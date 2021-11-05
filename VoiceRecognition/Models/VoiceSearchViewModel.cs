using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VoiceRecognition.Models
{
    public class VoiceSearchViewModel
    {
        public string voiceRendered { get; set; }
    }

    public class Translation
    {
        public string translatedText { get; set; }
    }

    public class Data
    {
        public List<Translation> translations { get; set; }
    }

    public class Root
    {
        public Data data { get; set; }
    }
}