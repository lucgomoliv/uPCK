using System;
using System.IO;
using System.Text;

namespace uPCK
{
	public class PCKFileEntry
	{
		public string Path { get; set; }
		public long Offset { get; set; }
		public int Size { get; set; }
		public int CompressedSize { get; set; }

		public PCKFileEntry(){ }

		public PCKFileEntry(byte[] bytes, short version)
		{
            if (version == 3) ReadV3(bytes);
            else ReadV2(bytes);
        }

        public void ReadV2(byte[] bytes)
        {
            if (bytes.Length < 276)
            {
                bytes = PCKZlib.Decompress(bytes, 276);
            }
            BinaryReader br = new BinaryReader(new MemoryStream(bytes));
            Path = Encoding.GetEncoding(936).GetString(br.ReadBytes(260)).Split(new string[] { "\0" }, StringSplitOptions.RemoveEmptyEntries)[0].Replace("/", "\\");
            Offset = br.ReadUInt32();
            Size = br.ReadInt32();
            CompressedSize = br.ReadInt32();
            br.Close();
        }

        public void ReadV3(byte[] bytes)
		{
			if (bytes.Length < 288)
			{
				bytes = PCKZlib.Decompress(bytes, 288);
			}
			BinaryReader br = new BinaryReader(new MemoryStream(bytes));
			Path = Encoding.GetEncoding(936).GetString(br.ReadBytes(260)).Split(new string[] { "\0" }, StringSplitOptions.RemoveEmptyEntries)[0].Replace("/", "\\");
            br.ReadInt32();
            Offset = br.ReadInt64();
			Size = br.ReadInt32();
			CompressedSize = br.ReadInt32();
			br.Close();
		}

		public byte[] Write(int CompressionLevel)
		{
            byte[] buffer = new byte[276];
            MemoryStream msb = new MemoryStream(buffer);
            BinaryWriter bw = new BinaryWriter(msb);
            bw.Write(Encoding.GetEncoding("GB2312").GetBytes(Path.Replace("/", "\\")));
            bw.BaseStream.Seek(260, SeekOrigin.Begin);
            bw.Write((uint)Offset);
            bw.Write(Size);
            bw.Write(CompressedSize);
            bw.Write(0);
            bw.BaseStream.Seek(0, SeekOrigin.Begin);
            bw.Close();
            byte[] compressed = PCKZlib.Compress(buffer, CompressionLevel);
            return compressed.Length < 276 ? compressed : buffer;
        }
	}
}
