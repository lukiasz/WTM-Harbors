using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SrtHarbor.Model
{
    public class SSAFile 
    {
        public SSAScriptInfoSection ScriptInfo { get; set; }
        public SSAV4StylesSection V4Styles { get; set; }
        public SSAEventsSection Events { get; set; }
        public string NewLine { get; set; }

        public string Compile()
        {
            var sb = new StringBuilder();
            sb.Append(ScriptInfo.Compile(NewLine));
            sb.Append(NewLine);
            sb.Append(V4Styles.Compile(NewLine));
            sb.Append(NewLine);
            sb.Append(Events.Compile(NewLine));
            sb.Append(NewLine);
            return sb.ToString();
        }

        public IReadOnlyList<string> ContentErrors { get; set; }

        public int LearningUnits { get; set; }
    }
}
