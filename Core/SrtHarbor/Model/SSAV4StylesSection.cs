using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SrtHarbor.Model
{
    public class SSAV4StylesSection
    {
        public string Format { get; set; }
        public List<string> Styles { get; set; }

        public string Compile(string newline)
        {
            var sb = new StringBuilder();
            sb.Append("[V4 Styles]");
            sb.Append(newline);
            sb.Append("Format: ");
            sb.Append(Format);
            sb.Append(newline);
            foreach (var style in Styles)
            {
                sb.Append("Style: ");
                sb.Append(style);
                sb.Append(newline);
            }
            return sb.ToString();
        }
    }
}
