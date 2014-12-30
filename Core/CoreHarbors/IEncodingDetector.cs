using SrtHarbor.Harbor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreHarbors
{
    public interface IEncodingDetector
    {
        string Read(byte[] data, out string detectedEncoding);
        Language CurrentLanguage { get; set; }
    }
}
