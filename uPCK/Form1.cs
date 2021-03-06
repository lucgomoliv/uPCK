﻿using System;
using System.Reflection;
using System.Windows.Forms;
using WK.Libraries.BetterFolderBrowserNS;
using zlib;

namespace uPCK
{
    public partial class Form1 : Form
    {
        public ArchiveEngine archive; 

        public Form1()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string resourceName = new AssemblyName(args.Name).Name + ".dll";
                string resource = Array.Find(this.GetType().Assembly.GetManifestResourceNames(), element => element.EndsWith(resourceName));

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
                {
                    Byte[] assemblyData = new Byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };
            InitializeComponent();
            cmbCompLvl.SelectedIndex = 9;
            archive = new ArchiveEngine(this);
        }

        private void btnUnpack_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Angelica Engine|*.pck|All Files|*.*"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                archive.compressionLevel = int.Parse(cmbCompLvl.Text);
                lblFile.Text = "File: " + ofd.FileName;
                archive.Unpack(ofd.FileName);
            }
        }
        private void btnCompress_Click(object sender, EventArgs e)
        {
            BetterFolderBrowser fbd = new BetterFolderBrowser();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                archive.compressionLevel = int.Parse(cmbCompLvl.Text);
                archive.Compress(fbd.SelectedPath);
            }
        }

        public delegate void UpdateProgressDelegate(Control ctrl, string text);
        public void UpdateProgress(Control ctrl, string text)
        {
            if (ctrl.InvokeRequired)
            {
                UpdateProgressDelegate del = new UpdateProgressDelegate(UpdateProgress);
                ctrl.Invoke(del, ctrl, text);
            }
            else ctrl.Text = text;
        }

        public delegate void UpdateProgressBarDelegate(Control ctrl, string prop, int value);
        public void UpdateProgressBar(Control ctrl, string prop, int value)
        {
            if (ctrl.InvokeRequired)
            {
                UpdateProgressBarDelegate del = new UpdateProgressBarDelegate(UpdateProgressBar);
                ctrl.Invoke(del, ctrl, prop, value);
            }
            else
            {
                switch (prop)
                {
                    case "max": 
                        ((ProgressBar) ctrl).Maximum = value;
                    break;
                    case "min":
                        ((ProgressBar)ctrl).Minimum = value;
                    break;
                    case "value":
                        ((ProgressBar)ctrl).Value = value;
                    break;
                }
            }
        }
    }
}
