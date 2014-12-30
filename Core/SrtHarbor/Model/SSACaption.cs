using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SrtHarbor.Model
{
    public class SSACaption
    {
        public int Id { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public string Text { get; set; }
        public string StyleName { get; set; }

        public string Compile()
        {
            var text = Text;
            // this should and will be done at TextParsedContent level
            text = text.Replace("\r\n", "\\N");
            text = text.Replace("\r", "\\N");
            text = text.Replace("\n", "\\N");

            // this might be done at upload time
            text = text.Replace("<i>", "{\\i1}")
                .Replace("</i>", "{\\i0}");

            text = text.Replace("<b>", "{\\b1}")
                .Replace("</b>", "{\\b0}");

            var stringFormat = "Dialogue: Marked=0,{0},{1},{2},NTP,0000,0000,0000,!Effect,{3}";
            return String.Format(stringFormat, Start.ToString("h\\:mm\\:ss\\.ff"),
                End.ToString("h\\:mm\\:ss\\.ff"), StyleName, text);
        }
    }
}
