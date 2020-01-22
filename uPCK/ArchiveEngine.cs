using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace uPCK
{
    public delegate void CloseAfterFinish();

    public class ArchiveEngine : INotifyPropertyChanged
    {
        public Form1 form;

        private System.Timers.Timer timers = new System.Timers.Timer(7000);
        private CountdownEvent events = new CountdownEvent(0);

        short version;
        public int compressionLevel;

        public ArchiveEngine(Form1 form)
        {
            timers.Elapsed += (a, b) =>
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            };
            timers.Start();
            this.form = form;
            compressionLevel = int.Parse((form.Controls["cmbCompLvl"] as ComboBox).Text);
        }

        public int GetFilesCount(PCKStream stream)
        {
            stream.Seek(-8, SeekOrigin.End);
            return stream.ReadInt32();
        }

        public IEnumerable<PCKFileEntry> ReadFileV2(PCKStream stream)
        {
            stream.Seek(-8, SeekOrigin.End);
            int FilesCount = stream.ReadInt32();
            form.UpdateProgressBar(form.Controls["progBar"], "max", FilesCount);
            stream.Seek(-272, SeekOrigin.End);
            long FileTableOffset = (long)((ulong)((uint)(stream.ReadUInt32() ^ (ulong)stream.key.KEY_1)));
            stream.Seek(FileTableOffset, SeekOrigin.Begin);
            BinaryReader TableStream = new BinaryReader(new MemoryStream(stream.ReadBytes((int)(stream.GetLenght() - FileTableOffset - 280))));
            for (int i = 0; i < FilesCount; ++i)
            {
                int EntrySize = TableStream.ReadInt32() ^ stream.key.KEY_1;
                TableStream.ReadInt32();
                yield return new PCKFileEntry(TableStream.ReadBytes(EntrySize), version);
            }
        }

        public IEnumerable<PCKFileEntry> ReadFileV3(PCKStream stream)
        {
            stream.Seek(-8, SeekOrigin.End);
            int FilesCount = stream.ReadInt32();
            form.UpdateProgressBar(form.Controls["progBar"], "max", FilesCount);
            stream.Seek(-280, SeekOrigin.End);
            long FileTableOffset = stream.ReadInt64() ^ stream.key.KEY_1;
            stream.Seek(FileTableOffset, SeekOrigin.Begin);
            BinaryReader TableStream = new BinaryReader(new MemoryStream(stream.ReadBytes((int)(stream.GetLenght() - FileTableOffset - 288))));
            for (int i = 0; i < FilesCount; ++i)
            {
                int EntrySize = TableStream.ReadInt32() ^ stream.key.KEY_1;
                TableStream.ReadInt32();
                yield return new PCKFileEntry(TableStream.ReadBytes(EntrySize), version);
            }
        }

        public void Unpack(string path)
        {
            new Thread(() =>
            {
                try
                {
                    _Unpack(path);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex.Message}\n\n{ex.Source}\n\n{ex.StackTrace}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }).Start();
        }

        public void Compress(string path)
        {
            new Thread(() =>
            {
                try
                {
                    _Compress(path);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{ex.Message}\n\n{ex.Source}\n\n{ex.StackTrace}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }).Start();
        }

        private void _Unpack(string path)
        {
            string dir = $"{path}.files";
            if (Directory.Exists(dir))
            {
                form.UpdateProgress(form.Controls["lblProgress"], "Removing Existing Directory");
                Directory.Delete(dir, true);
            }
            Directory.CreateDirectory(dir);
            PCKStream stream = new PCKStream(path);
            stream.Seek(-4, SeekOrigin.End);
            version = stream.ReadInt16();
            events = new CountdownEvent(GetFilesCount(stream));
            IEnumerator<PCKFileEntry> enumerator;
            //Select wich version to read
            if (version == 3) enumerator = ReadFileV3(stream).GetEnumerator();
            else enumerator = ReadFileV2(stream).GetEnumerator();
            while (enumerator.MoveNext())
            {
                string p = Path.Combine(dir, Path.GetDirectoryName(enumerator.Current.Path));
                if (!Directory.Exists(p))
                {
                    Directory.CreateDirectory(p);
                }
                form.UpdateProgress(form.Controls["lblProgress"], 
                                                    $"Unpacking " +
                                                    $"{(form.Controls["progBar"] as ProgressBar).Value}" +
                                                    $"/{(form.Controls["progBar"] as ProgressBar).Maximum}: " +
                                                    $"{ enumerator.Current.Path }"); 
                stream.Seek(enumerator.Current.Offset, SeekOrigin.Begin);
                byte[] file = stream.ReadBytes(enumerator.Current.CompressedSize);
                ThreadPool.QueueUserWorkItem(x => {
                    byte[] buffer = (x as object[])[0] as byte[];
                    PCKFileEntry entry = (x as object[])[1] as PCKFileEntry;
                    File.WriteAllBytes(Path.Combine(dir, entry.Path), buffer.Length < entry.Size ? PCKZlib.Decompress(buffer, entry.Size) : buffer);
                    events.Signal();
                }, new object[] { file, enumerator.Current });
                form.UpdateProgressBar(form.Controls["progBar"], "value", ((ProgressBar)form.Controls["progBar"]).Value+1);
            }
            form.UpdateProgress(form.Controls["lblProgress"], "WaitThreads");
            events.Wait();
            stream.Dispose();
            form.UpdateProgressBar(form.Controls["progBar"], "value", 0);
            form.UpdateProgress(form.Controls["lblProgress"], "Ready");
        }

        public void _Compress(string dir)
        {
            string pck = dir.Replace(".files", "");
            if (File.Exists(pck))
                File.Delete(pck);
            if (File.Exists(pck.Replace(".pck", ".pkx")))
                File.Delete(pck.Replace(".pck", ".pkx"));
            form.UpdateProgress(form.Controls["lblProgress"], "FileList");
            string[] files = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
            PCKStream stream = new PCKStream(pck);
            stream.WriteInt32(stream.key.FSIG_1);
            stream.WriteInt32(0);
            stream.WriteInt32(stream.key.FSIG_2);
            form.UpdateProgressBar(form.Controls["progBar"], "max", files.Length);
            MemoryStream FileTable = new MemoryStream();
            events = new CountdownEvent(files.Length);
            for (form.UpdateProgressBar(form.Controls["progBar"], "value", 0);
                 (form.Controls["progBar"] as ProgressBar).Value <
                 (form.Controls["progBar"] as ProgressBar).Maximum;
                 form.UpdateProgressBar(form.Controls["progBar"], "value", ((ProgressBar)form.Controls["progBar"]).Value+1))
            {
                string file = files[(form.Controls["progBar"] as ProgressBar).Value].Replace(dir, "").Replace("/", "\\").Remove(0, 1);
                form.UpdateProgress(form.Controls["lblProgress"], $"Compressing " +
                                                    $"{(form.Controls["progBar"] as ProgressBar).Value}" +
                                                    $"/{(form.Controls["progBar"] as ProgressBar).Maximum}: {file}");
                byte[] decompressed = File.ReadAllBytes(Path.Combine(dir, files[(form.Controls["progBar"] as ProgressBar).Value]));
                byte[] compressed = PCKZlib.Compress(decompressed, compressionLevel);
                var entry = new PCKFileEntry()
                {
                    Path = file,
                    Offset = (uint)stream.Position,
                    Size = decompressed.Length,
                    CompressedSize = compressed.Length
                };
                stream.WriteBytes(compressed);
                    byte[] buffer = entry.Write(compressionLevel);
                    lock (FileTable)
                    {
                        FileTable.Write(BitConverter.GetBytes(buffer.Length ^ stream.key.KEY_1), 0, 4);
                        FileTable.Write(BitConverter.GetBytes(buffer.Length ^ stream.key.KEY_2), 0, 4);
                        FileTable.Write(buffer, 0, buffer.Length);
                    }
                    events.Signal();
            }
            events.Wait();
            long FileTableOffset = stream.Position;
            stream.WriteBytes(FileTable.ToArray());
            stream.WriteInt32(stream.key.ASIG_1);//4
            stream.WriteInt16(2);//2
            stream.WriteInt16(2);//2
            stream.WriteUInt32((uint)(FileTableOffset ^ stream.key.KEY_1));//4
            stream.WriteInt32(0);//4
            stream.WriteBytes(Encoding.Default.GetBytes("Angelica File Package, Perfect World."));//37
            byte[] nuller = new byte[215];
            stream.WriteBytes(nuller);//215 - 268
            stream.WriteInt32(stream.key.ASIG_2);//4
            stream.WriteInt32(files.Length);//4
            stream.WriteInt16(2);//2
            stream.WriteInt16(2);//2
            stream.Seek(4, SeekOrigin.Begin);
            stream.WriteUInt32((uint)stream.GetLenght());
            stream.Dispose();
            form.UpdateProgressBar(form.Controls["progBar"], "value", 0);
            form.UpdateProgress(form.Controls["lblProgress"], "Ready");
        }

        public string ParseBase64Path(string root_path, string path)
        {
            string output = root_path;
            string[] split = path.Split(char.Parse("\\"));
            foreach (string str in split)
            {
                try
                {
                    output += $"\\{Encoding.UTF8.GetString(Convert.FromBase64String(str))}";
                }
                catch
                {
                    output += str;
                }
            }
            return output;
        }

        public byte[] ReadFile(PCKStream stream, PCKFileEntry file)
        {
            stream.Seek(file.Offset, SeekOrigin.Begin);
            byte[] bytes = stream.ReadBytes(file.CompressedSize);
            return file.CompressedSize < file.Size ? PCKZlib.Decompress(bytes, file.Size) : bytes;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
