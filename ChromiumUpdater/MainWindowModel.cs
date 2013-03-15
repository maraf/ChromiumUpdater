using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DesktopCore;

namespace ChromiumUpdater
{
    public class MainWindowModel : DesktopCore.Data.IComponentModel<DataModel>
    {
        public DataModel Model { get; set; }

        public Dictionary<string, object> Settings { get; set; }

        public MainWindowModel()
        {
            Model = new DataModel();
            Settings = new Dictionary<string, object>();
        }
    }

    public class DataModel : NotifyPropertyChanged
    {
        private string installationFolder = "";
        private string lastBuildUrl = "http://commondatastorage.googleapis.com/chromium-browser-continuous/Win/LAST_CHANGE";
        private string downloadBuildUrl = "http://commondatastorage.googleapis.com/chromium-browser-continuous/Win/{0}/";
        private string downloadFileName = "chrome-win32.zip";
        private string downloadChangeLog = "changelog.xml";
        private string localVersion;
        private bool autoCheckAndDownload = false;
        private bool autoRun = false;
        private bool autoRunChromium = false;
        private bool autoCloseUpdater = false;

        private int status = 0;

        public string InstallationFolder
        {
            get { return installationFolder; }
            set
            {
                installationFolder = value;
                FirePropertyChanged("InstallationFolder");
            }
        }

        public string LastBuildUrl
        {
            get { return lastBuildUrl; }
            set
            {
                lastBuildUrl = value;
                FirePropertyChanged("LastBuildUrl");
            }
        }

        public string DownloadBuildUrl
        {
            get { return downloadBuildUrl; }
            set
            {
                downloadBuildUrl = value;
                FirePropertyChanged("DownloadBuildUrl");
            }
        }

        public string DownloadFileName
        {
            get { return downloadFileName; }
            set
            {
                downloadFileName = value;
                FirePropertyChanged("DownloadFileName");
            }
        }

        public string DownloadChangeLog
        {
            get { return downloadChangeLog; }
            set
            {
                downloadChangeLog = value;
                FirePropertyChanged("DownloadChangeLog");
            }
        }

        public string LocalVersion
        {
            get { return localVersion; }
            set
            {
                localVersion = value;
                FirePropertyChanged("LocalVersion");
            }
        }

        public bool AutoCheckAndDownload
        {
            get { return autoCheckAndDownload; }
            set
            {
                autoCheckAndDownload = value;
                FirePropertyChanged("AutoCheckAndDownload");
            }
        }

        public bool AutoRun
        {
            get { return autoRun; }
            set
            {
                autoRun = value;
                FirePropertyChanged("AutoRun");
            }
        }

        public bool AutoRunChromium
        {
            get { return autoRunChromium; }
            set
            {
                autoRunChromium = value;
                FirePropertyChanged("AutoRunChromium");
            }
        }

        public bool AutoCloseUpdater
        {
            get { return autoCloseUpdater; }
            set
            {
                autoCloseUpdater = value;
                FirePropertyChanged("AutoCloseUpdater");
            }
        }


        public int Status
        {
            get { return status; }
            set
            {
                status = value;
                FirePropertyChanged("Status");
            }
        }
    }
}
