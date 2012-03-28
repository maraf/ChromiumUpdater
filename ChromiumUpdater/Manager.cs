using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.IsolatedStorage;
using System.IO;
using System.Xml.Serialization;
using System.Net;
using System.ComponentModel;
using Ionic.Zip;
using System.Threading;
using System.Windows.Threading;

namespace ChromiumUpdater
{
    public class Manager
    {
        public static readonly string FileName = "ChromuimUpdater.dat";

        public MainWindowModel Model { get; set; }

        public void Save()
        {
            IsolatedStorageFile f = IsolatedStorageFile.GetUserStoreForAssembly();
            using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(FileName, FileMode.Create, f))
            {
                XmlSerializer xs = new XmlSerializer(typeof(DataModel));
                xs.Serialize(stream, Model.Model);
            }
        }

        public void Load()
        {
            IsolatedStorageFile f = IsolatedStorageFile.GetUserStoreForAssembly();
            if (f.FileExists(FileName))
            {
                try
                {
                    using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(FileName, FileMode.OpenOrCreate, f))
                    {
                        XmlSerializer xs = new XmlSerializer(typeof(DataModel));
                        Model = new MainWindowModel();
                        Model.Model = xs.Deserialize(stream) as DataModel;
                    }
                }
                finally
                {
                    if (Model == null)
                        Model = new MainWindowModel();
                    else if (Model.Model == null)
                        Model.Model = new DataModel();
                }
            }
            else
            {
                Model = new MainWindowModel();
            }

            Model.Model.Status = 0;
            if (String.IsNullOrEmpty(Model.Model.InstallationFolder) || String.IsNullOrWhiteSpace(Model.Model.InstallationFolder))
                Model.Model.InstallationFolder = Environment.CurrentDirectory;

            if (Model.Model.LocalVersion == null)
                Model.Model.LocalVersion = "0";
        }

        public void Delete()
        {
            IsolatedStorageFile f = IsolatedStorageFile.GetUserStoreForAssembly();
            if (f.FileExists(FileName))
                f.DeleteFile(FileName);
        }

        public string GetLastRevision()
        {
            try
            {
                WebClient client = new WebClient();
                byte[] data = client.DownloadData(Model.Model.LastBuildUrl);
                return Encoding.UTF8.GetString(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                throw new UpdaterException("Updater exception", ex);
            }
        }

        public WebClient DownloadVersion(string revision, LogDelegate log, Dispatcher dispatcher)
        {
            string downloadUrl = Path.Combine(String.Format(Model.Model.DownloadBuildUrl, revision), Model.Model.DownloadFileName);
            string localPath = Path.Combine(Model.Model.InstallationFolder, Model.Model.DownloadFileName);
            string extractPath = Model.Model.InstallationFolder;

            try
            {
                WebClient client = new WebClient();
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(delegate(object sender, DownloadProgressChangedEventArgs e)
                {
                    Model.Model.Status = e.ProgressPercentage;
                });
                client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(delegate(object sender, AsyncCompletedEventArgs e)
                {
                    if (e.Error == null && e.Cancelled == false)
                    {
                        log(LogType.FileDownloaded);
                        ZipFile zip = new ZipFile(localPath);

                            Action<string, ExtractExistingFileAction> extractAll = zip.ExtractAll;
                            Action finished = () =>
                            {
                                Model.Model.LocalVersion = revision;
                                Model.Model.Status = 0;
                                log(LogType.FileExtracted);
                                
                                zip.Dispose();
                                File.Delete(localPath);
                            };

                            Model.Model.Status = 0;
                            zip.ExtractProgress += new EventHandler<ExtractProgressEventArgs>((s, p) =>
                            {
                                if (p.EntriesTotal != 0)
                                    Model.Model.Status = (int)((double)p.EntriesExtracted / p.EntriesTotal * 100);
                            });

                            extractAll.BeginInvoke(extractPath, ExtractExistingFileAction.OverwriteSilently, (ar) =>
                            {
                                Action<string, ExtractExistingFileAction> a = (ar as System.Runtime.Remoting.Messaging.AsyncResult).AsyncDelegate as Action<string, ExtractExistingFileAction>;
                                try
                                {
                                    if (a != null)
                                        a.EndInvoke(ar);

                                    ThreadStart start = delegate()
                                    {
                                        DispatcherOperation op = dispatcher.BeginInvoke(
                                            finished,
                                            DispatcherPriority.Normal
                                        );

                                        DispatcherOperationStatus status = op.Status;
                                        while (status != DispatcherOperationStatus.Completed)
                                        {
                                            status = op.Wait(TimeSpan.FromMilliseconds(1000));
                                            if (status == DispatcherOperationStatus.Aborted)
                                                throw new UpdaterException("Dispatcher operation aborted!");
                                        }
                                    };

                                    new Thread(start).Start();
                                }
                                catch (Exception ex)
                                {
                                    throw new UpdaterException("Updater exception", ex);
                                }
                            }, null);

                    }
                    else if (e.Error != null)
                    {
                        log(LogType.FileDownloadError, e.Error);
                    }
                    else if (e.Cancelled)
                    {
                        log(LogType.FileDownloadCanceled);
                    }
                });
                client.DownloadFileAsync(new Uri(downloadUrl), localPath);
                return client;
            }
            catch (Exception ex)
            {
                throw new UpdaterException("Updater exception", ex);
            }
        }
    }
}
