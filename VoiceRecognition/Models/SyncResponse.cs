using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VoiceRecognition.Models
{
    public class SyncResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public long FromId { get; set; }

        public long ToId { get; set; }

        public DateTime DepurtureDate{ get; set; }
    }
}