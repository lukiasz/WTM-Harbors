using SrtHarbor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SrtHarbor.Converting
{
    public static class SRTToSSA
    {
        public static SSAFile 
            ToSSA(this SRTFile srtContent)
        {
            var scriptInfo = new SSAScriptInfoSection
            {
                ScriptType = "v4.00",
                Collisions = "Normal",
                Timer = "100.0000"
            };

            var v4Styles = new SSAV4StylesSection
            {
                Format = "Name, Fontname, Fontsize, PrimaryColour, SecondaryColour, TertiaryColour, BackColour, Bold, Italic, BorderStyle, Outline, Shadow, Alignment, MarginL, MarginR, MarginV, AlphaLevel, Encoding",
                Styles = new List<string>
                {
                    "Default,Arial,22,HFFFFFF,0,HFFFFFF,0,0,0,0,1,0,2,20,20,10,0",
                }
            };

            var events = new SSAEventsSection
            {
                Format = "Marked, Start, End, Style, Name, MarginL, MarginR, MarginV, Effect, Text",
                Dialogues = new List<SSACaption>()
            };

            foreach (var caption in srtContent.Subtitles)
            {
                events.Dialogues.Add(new SSACaption
                {
                    End = caption.End,
                    Id = caption.Id,
                    Start = caption.Start,
                    StyleName = "Default",
                    Text = caption.Text
                });
            }

            var ssaFile = new SSAFile
            {
                Events = events,
                LearningUnits = srtContent.LearningUnits,
                ScriptInfo = scriptInfo,
                V4Styles = v4Styles,
                NewLine = srtContent.NewLine
            };

            return ssaFile;
        }
    }
}
