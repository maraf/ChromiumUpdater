using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ChromiumUpdater
{
    [Serializable]
    public class UpdaterException : Exception
    {
        public UpdaterException() { }
        public UpdaterException(string message) : base(message) { }
        public UpdaterException(string message, Exception inner) : base(message, inner) { }
        protected UpdaterException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
