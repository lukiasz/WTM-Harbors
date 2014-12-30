using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SrtHarbor.Model
{
    public class SSAScriptInfoSection
    {
        public string Title { get; set; }
        public string OriginalScript { get; set; }
        public string OriginalTranslation { get; set; }
        public string OriginalEditing { get; set; }
        public string OriginalTiming { get; set; }
        public string SynchPoint { get; set; }
        public string ScriptUpdatedBy { get; set; }
        public string UpdateDetails { get; set; }
        public string ScriptType { get; set; }
        public string Collisions { get; set; }
        public string PlayResY { get; set; }
        public string PlayResX { get; set; }
        public string PlayDepth { get; set; }
        public string Timer { get; set; }
        public string WrapStyle { get; set; }

        public string Compile(string newline)
        {
            var sb = new StringBuilder();
            sb.Append("[Script Info]");
            sb.Append(newline);
            sb.Append("ScriptType: ");
            sb.Append(ScriptType);
            sb.Append(newline);
            sb.Append("Collisions: ");
            sb.Append(Collisions);
            sb.Append(newline);
            sb.Append("Timer: ");
            sb.Append(Timer);
            sb.Append(newline);
            return sb.ToString();
        }
    }
}
