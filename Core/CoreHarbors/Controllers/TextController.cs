using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using TextHarbor;

namespace CoreHarbors.Controllers
{
    public class TextController : ApiController
    {
        [Route("Text/Metadata")]
        public object Get()
        {
            return new
            {
                Name = "Text file harbor",
                Description = "A text file harbor which adds translations after or before words.",
                Parse = new string[] { ".txt", ".TXT", "Plain Text" },
                Translate = new string[] { ".txt", ".TXT", "Plain Text" },
                TranslationOptionsDefinitions = new[]
                {
                    new { PropertyName = "SkipIdenticalTranslations", Description = "Skip translations which are identical to original words" },
                    new { PropertyName = "InsertTranslationsBeforeOriginalWords", Description = "Insert translations before original words" }
                }
            };
        }

        [Route("Text/Parse")]
        public Cargo Post(Cargo cargo)
        {
            var encodingDetector = new MozillaEncodingDetector();
            encodingDetector.CurrentLanguage = Language.Polish;
            var binary = Convert.FromBase64String(cargo.BinaryBase64);
            string detectedEncoding;
            var decodedData = encodingDetector.Read(binary, out detectedEncoding);
            cargo.Parse(decodedData);
            cargo.Type = "Plain Text";
            return cargo;
        }

        [Route("Text/Translate")]
        public Cargo Post(CargoAndTranslationOptions request)
        {
            var encodingDetector = new MozillaEncodingDetector();
            encodingDetector.CurrentLanguage = Language.Polish;
            var binary = Convert.FromBase64String(request.cargo.BinaryBase64);
            string detectedEncoding;
            var decodedData = encodingDetector.Read(binary, out detectedEncoding);
            request.cargo.Translate(decodedData, request.translationOptions);
            return request.cargo;
        }

        // A handy method to look for a raw http request's content
        private string ReadRawRequest()
        {
            using (var stream = new MemoryStream())
            {
                var context = (HttpContextBase)Request.Properties["MS_HttpContext"];
                context.Request.InputStream.Seek(0, SeekOrigin.Begin);
                context.Request.InputStream.CopyTo(stream);
                string requestBody = Encoding.UTF8.GetString(stream.ToArray());
                return requestBody;
            }
        }
    }
}