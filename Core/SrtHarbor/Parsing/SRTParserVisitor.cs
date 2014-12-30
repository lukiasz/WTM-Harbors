using SrtHarbor.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SrtHarbor.Parsing
{
    public class SRTParserVisitor : SRTParserBaseVisitor<SRTFile>
    {
        private readonly string lineEndings;

        public SRTParserVisitor()
        {
            this.lineEndings = LineEndings.Windows;
        }

        public override SRTFile VisitFile(SRTParser.FileContext context)
        {
            var subtitles = context
                .children
                .OfType<SRTParser.SubtitleContext>()
                .ToList()
                .Select(child => VisitSubtitle(child)).ToList();

            var totalTime = subtitles[subtitles.Count - 1].End;
            var learningUnits = totalTime.TotalMinutes / 15;
            learningUnits = (int)Math.Ceiling(learningUnits);

            return new SRTFile(subtitles, lineEndings, (int)learningUnits);
        }

        private new SRTCaption VisitSubtitle(SRTParser.SubtitleContext context)
        {
            var id = Convert.ToInt32(context.children[0].GetText());
            var sh = Convert.ToInt32(context.children[2]
                .GetText().Substring(0, 2));
            var sm = Convert.ToInt32(context.children[2]
                .GetText().Substring(3, 2));
            var ss = Convert.ToInt32(context.children[2]
                .GetText().Substring(6, 2));
            var sms = Convert.ToInt32(context.children[2]
                .GetText().Substring(9, 3));

            var eh = Convert.ToInt32(context.children[2]
                .GetText().Substring(17, 2));
            var em = Convert.ToInt32(context.children[2]
                .GetText().Substring(20, 2));
            var es = Convert.ToInt32(context.children[2]
                .GetText().Substring(23, 2));
            var ems = Convert.ToInt32(context.children[2]
                .GetText().Substring(26, 3));

            var text = context.children[3].GetText();
            
            if (text[text.Length-2] == '\r')
                text = text.Substring(0, text.Length - 2);
            else
                text = text.Substring(0, text.Length - 1);

            var c = new SRTCaption
            {
                Id = id,
                Start = new TimeSpan(0, sh, sm, ss, sms),
                End = new TimeSpan(0, eh, em, es, ems),
                Text = text,
                //CR: It's not perfect
            };
            return c;
        }

    }
}
