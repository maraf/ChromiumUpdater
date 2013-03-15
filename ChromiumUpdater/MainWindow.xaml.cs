using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Diagnostics;
using Path = System.IO.Path;

namespace ChromiumUpdater
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Manager Manager { get; protected set; }
        private WebClient client = null;
        private string lastRevision = null;

        public MainWindow()
        {
            InitializeComponent();

            Manager = new Manager();
            Manager.Load();

            DataContext = Manager.Model;

            //If not cofigured yet, configure it
            if (Manager.Model.Model.LocalVersion == "0")
                dpnMain.Visibility = Visibility.Collapsed;

            //Auto download
            if (Manager.Model.Model.AutoCheckAndDownload)
            {
                btnCheckVersion_Click(null, null);
                if (Manager.Model.Model.LocalVersion != lastRevision)
                    btnStartDownload_Click(null, null);
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            DesktopCore.WindowHelper.ExtendGlassFrame(this, new Thickness(-1));
        }

        private void Log(string log, params object[] parameters)
        {
            tbxLog.Text += String.Format("{0}{1}", String.Format(log, parameters), Environment.NewLine);
        }

        private void LogException(UpdaterException ex)
        {
            if (ex.InnerException != null)
                Log(ex.InnerException.Message);
            else
                Log(ex.Message);
        }

        private void winMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Manager.Save();
        }

        private void btnConfig_Click(object sender, RoutedEventArgs e)
        {
            dpnMain.Visibility = Visibility.Collapsed;
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            dpnMain.Visibility = Visibility.Visible;
            if (Manager.Model.Model.AutoRun)
                DesktopHelper.CreateStartupShortcut();
            else
                DesktopHelper.DeleteStartupShortcut();
        }

        private void btnRestoreConfig_Click(object sender, RoutedEventArgs e)
        {
            Manager.Delete();
            Manager.Load();
            DataContext = Manager.Model;
        }

        private void btnCheckVersion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                lastRevision = Manager.GetLastRevision();
                Log("Local revision number is {0}, last revision number is {1}.", Manager.Model.Model.LocalVersion, lastRevision);
            }
            catch (UpdaterException ex)
            {
                LogException(ex);
            }
        }

        private void btnStartDownload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (lastRevision == null)
                    btnCheckVersion_Click(sender, e);

                Log("Downloading.");
                btnStartDownload.IsEnabled = false;
                btnStopDownload.IsEnabled = true;
                client = Manager.DownloadVersion(lastRevision, (type, error) => 
                {
                    switch (type)
                    {
                        case LogType.FileDownloaded:
                            Log("File downloaded, extracting.");
                            break;
                        case LogType.FileDownloadError:
                            Log("Error occured in download.{1}{0}", error.Message, Environment.NewLine);
                            break;
                        case LogType.FileDownloadCanceled:
                            Log("Download canceled.");
                            break;
                        case LogType.FileExtracted:
                            Log("New version applied.");

                            if (Manager.Model.Model.AutoRunChromium)
                                Process.Start(Path.Combine(Manager.Model.Model.InstallationFolder, Path.GetFileNameWithoutExtension(Manager.Model.Model.DownloadFileName), "chrome.exe"));

                            if (Manager.Model.Model.AutoCloseUpdater)
                                Close();

                            break;
                    }
                    btnStartDownload.IsEnabled = true;
                    btnStopDownload.IsEnabled = false;
                    client = null;
                }, Dispatcher);
            }
            catch (UpdaterException ex)
            {
                LogException(ex);
            }
        }

        private void btnStopDownload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (client != null)
                {
                    client.CancelAsync();
                    btnStopDownload.IsEnabled = false;
                    btnStartDownload.IsEnabled = true;
                }
            }
            catch (UpdaterException ex)
            {
                LogException(ex);
            }
        }
    }
}
