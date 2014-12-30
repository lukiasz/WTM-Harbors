using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SrtHarbor.Harbor;

namespace CoreHarbors
{
    /// <summary>
    /// It's simple class which uses Mozilla's charset detection.
    /// </summary>
    public class MozillaEncodingDetector : IEncodingDetector
    {
        public string Read(byte[] data, out string detectedEncoding)
        {
            var cd = new Ude.CharsetDetector();
            cd.Feed(data, 0, data.Length);
            cd.DataEnd();

            //if (cd.Charset == null)
            //    throw new CharsetDetectionException<Messages>(
            //        Messages.Error_CharsetNotDetected);

            var charset = AlterEncoding(cd.Charset);
            var current = System.Text.Encoding.GetEncoding(charset);

            detectedEncoding = charset;
            return current.GetString(data);
        }

        public Language CurrentLanguage { get; set; }

        private string AlterEncoding(string recognizedEncoding)
        {
            // 1252 and 1250 are similar, but since we know language
            // we can assume it's 1250
            if ((recognizedEncoding == "windows-1252"
                || recognizedEncoding == null)
                && CurrentLanguage == Language.Polish)
            {
                return "windows-1250";
            }
            return recognizedEncoding;
        }
    }
}