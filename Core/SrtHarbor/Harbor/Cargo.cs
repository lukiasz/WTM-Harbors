using NLog;
using SrtHarbor.Model;
using SrtHarbor.SubRipText;
using SrtHarbor.Translating;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using SrtHarbor.Parsing;
using SrtHarbor.Converting;

namespace SrtHarbor.Harbor
{
    public class Cargo
    {
        public List<Block> Blocks { get; set; }
        public string BinaryBase64 { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string ContentType { get; set; }
        public GlobalHarborData HarborData { get; set; }

        public override string ToString()
        {
            return Name + "( " + Type + ", " + ContentType + ") ";
        }
    }
}