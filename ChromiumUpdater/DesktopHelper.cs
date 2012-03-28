using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace ChromiumUpdater
{
    public class DesktopHelper
    {
        public static void CreateStartupShortcut()
        {
            IWshRuntimeLibrary.IWshShell_Class wshShell = new IWshRuntimeLibrary.IWshShell_Class();
            IWshRuntimeLibrary.IWshShortcut shortcut;
            string startUpFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

            // Create the shortcut
            shortcut = (IWshRuntimeLibrary.IWshShortcut)wshShell.CreateShortcut(startUpFolderPath + "\\ChromiumUpdater.lnk");

            shortcut.TargetPath = Process.GetCurrentProcess().MainModule.FileName;
            shortcut.WorkingDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            shortcut.Description = "Launch Chromium updater";
            shortcut.IconLocation =  Environment.CurrentDirectory + @"\Refresh2.ico";
            shortcut.Save();
        }

        public static void DeleteStartupShortcut()
        {
            string startUpFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

            DirectoryInfo di = new DirectoryInfo(startUpFolderPath);
            FileInfo[] files = di.GetFiles("*.lnk");

            foreach (FileInfo fi in files)
            {
                //string shortcutTargetFile = GetShortcutTargetFile(fi.FullName);
                if (fi.Name.Equals("ChormiumUpdater.lnk", StringComparison.InvariantCultureIgnoreCase))
                {
                    System.IO.File.Delete(fi.FullName);
                }
            }
        }
    }
}
