using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChromiumUpdater
{
    public delegate void LogDelegate(LogType type, Exception error = null);

    public delegate void ProgressDelegate(int currentPercentage);

    public enum LogType
    {
        FileDownloaded, FileDownloadError, FileDownloadCanceled, FileExtracted
    }
}
