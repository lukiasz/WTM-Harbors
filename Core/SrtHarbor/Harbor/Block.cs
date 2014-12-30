using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SrtHarbor.Harbor
{
    public class Block
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public LearningData LearningData { get; set; }
        public CaptionData HarborData { get; set; }
    }
}