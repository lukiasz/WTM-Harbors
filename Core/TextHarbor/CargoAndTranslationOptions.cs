using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TextHarbor
{
    public class CargoAndTranslationOptions
    {
        public Cargo cargo { get; set; }
        public Dictionary<string, bool> translationOptions { get; set; }
    }
}