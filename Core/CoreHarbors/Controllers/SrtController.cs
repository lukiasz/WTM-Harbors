using SrtHarbor;
using SrtHarbor.Harbor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace CoreHarbors.Controllers
{
    public class SrtController: ApiController
    {
        [Route("Srt/Metadata")]
        public object Get()
        {
            return new
            {
                Name = "Srt file harbor",
                Description = "Can handle srt files adding translations to them inline," +
                    "in separate dialogs or at top of screen after converting subtitles to ass format.",
                Parse = new string[] { ".SRT", ".srt", "SubRip Text"},
                Translate = new string[] { ".SRT", ".srt", "SubRip Text" },
                TranslationOptionsDefinitions = new object[]
                {
                    new { PropertyName = "RemoveKnownDialogs", Description = "Remove dialogs where all words are known" },
                    new { PropertyName = "SrtOutput", Description = "Output in basic srt file" }
                }
            };
        }

        [Route("Srt/Parse")]
        public Cargo Post(Cargo cargo)
        {
            var encodingDetector = new MozillaEncodingDetector();
            encodingDetector.CurrentLanguage = Language.Polish;
            var binary = Convert.FromBase64String(cargo.BinaryBase64);
            string detectedEncoding;
            var decodedData = encodingDetector.Read(binary, out detectedEncoding);

            var harbor = new Harbor();
            harbor.Parse(cargo, decodedData);

            cargo.Type = "SubRip Text";
            return cargo;
        }

        [Route("Srt/Translate")]
        public Cargo Post(CargoAndTranslationOptions request)
        {
            var harbor = new Harbor();
            harbor.Translate(request.cargo, request.translationOptions);

            return request.cargo;
        }
    }
}