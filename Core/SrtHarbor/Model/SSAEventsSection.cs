using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SrtHarbor.Model
{
    public class SSAEventsSection
    {
        public string Format { get; set; }
        public List<SSACaption> Dialogues { get; set; }

        public string Compile(string newline)
        {
            var sb = new StringBuilder();
            sb.Append("[Events]");
            sb.Append(newline);
            sb.Append("Format: ");
            sb.Append(Format);
            sb.Append(newline);
            foreach (var caption in Dialogues)
            {
                sb.Append(caption.Compile());
                sb.Append(newline);
            }
            return sb.ToString();
        }
    }
}
