using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SrtHarbor.Model
{
    public class SRTFile
    {
        public SRTFile(IList<SRTCaption> subtitles, string newLine, int learningUnits)
        {
            this.Subtitles = subtitles;

            this.NewLine = newLine ?? LineEndings.Windows;
            this.LearningUnits = learningUnits;
        }

        public IList<SRTCaption> Subtitles { get; set; }

        public string NewLine { get; private set; }

        public string Compile()
        {
            var sb = new StringBuilder();
            var lastId = 0;
            Subtitles.ToList()
                .ForEach(c => sb.Append(CompileSubtitle(c, ++lastId))
                                .Append(NewLine));

            return sb.ToString();
        }

        private string CompileSubtitle(SRTCaption c, int lastId)
        {
            var sb = new StringBuilder(lastId.ToString())
            .Append(NewLine)
                .Append(c.Start.ToString("hh\\:mm\\:ss\\,fff"))
                .Append(" --> ")
                .Append(c.End.ToString("hh\\:mm\\:ss\\,fff"))
                .Append(NewLine)
                .Append(c.Text.ToArray())
                .Append(NewLine);
            return sb.ToString();
            throw new NotImplementedException();
        }

        /// <summary>
        /// One learning unit = 15 minutes
        /// </summary>
        public int LearningUnits
        {
            get;
            private set;
        }
    }
}
