using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SrtHarbor.Model
{
    public class LineEndings
    {
        public static readonly string Default = Environment.NewLine;
        public static readonly string Unix = "\n";
        public static readonly string Windows = "\r\n";
        public static readonly string Mac = "\r";
    }
}
