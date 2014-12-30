using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SrtHarbor.Harbor
{
    public class Word
    {
        public int Id { get; set; }
        public int StartIndex { get; set; }
        public int Length { get; set; }
        public string ShortTranslation { get; set; }
        public Knowledge Knowledge { get; set; }
    }
}