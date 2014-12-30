using SrtHarbor.Harbor;
using System;

namespace SrtHarbor.Model
{
    public class SRTCaption
    {
        public int Id { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public string Text { get; set; }

        public Block ToBlockWithId(int id)
        {
            return new Block
            {
                Id = id,
                HarborData = new CaptionData
                {
                    End = End,
                    Start = Start
                },
                Text = Text
            };
        }
    }
}
