using System;
using System.Windows.Forms;
using System.Reflection;

namespace uPCK
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string resource1 = "uPCK.BetterFolderBrowser.dll";
            string resource2 = "uPCK.zlib.net.dll";
            EmbeddedAssembly.Load(resource1, "System.Windows.Forms.Ribbon35.dll");
            EmbeddedAssembly.Load(resource2, "System.Data.SQLite.dll");

            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            return EmbeddedAssembly.Get(args.Name);
        }
    }
}
