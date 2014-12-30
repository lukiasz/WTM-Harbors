using NLog;
using SrtHarbor.Model;
using SrtHarbor.Translating;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SrtHarbor.Converting;
using SrtHarbor.Parsing;

namespace SrtHarbor.Harbor
{
    public class Harbor
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        public void Translate(Cargo cargo, Dictionary<string, bool> translationOptions)
        {
            if (!AreCorrect(translationOptions))
                throw new InvalidOperationException("Translations options are not correct!");

            var srtOutput = (bool)translationOptions["SrtOutput"];
            var removeKnownDialogs = (bool)translationOptions["RemoveKnownDialogs"];

            var subtitles = cargo.Blocks.Select(b => new SrtHarbor.Model.SRTCaption
            {
                Id = b.Id,
                Start = b.HarborData.Start,
                End = b.HarborData.End,
                Text = b.Text
            }).ToList();

            var subRipTextFile = new SRTFile(subtitles,
                cargo.HarborData.LineEnding, cargo.HarborData.LearningUnits);

            var translations = from b in cargo.Blocks
                               from w in b.LearningData.Words
                               where w.Knowledge == Knowledge.NotKnown
                               select new Translation
                               {
                                   TranslatedText = w.ShortTranslation,
                                   Position = new WordPosInSRTFile
                                   {
                                       CaptionPos = b.Id,
                                       EndPos = w.StartIndex + w.Length,
                                       StartPos = w.StartIndex
                                   }
                               };

            if (srtOutput)
            {
                var translator = new SubRipFileAdvancedTranslator(Color.CornflowerBlue, "Trebuchet MS", 3, true);
                translator.Translate(subRipTextFile, translations.ToList(), removeKnownDialogs);
                var translatedContent = subRipTextFile.Compile();
                var binary = UTF8Encoding.UTF8.GetBytes(translatedContent);
                cargo.BinaryBase64 = Convert.ToBase64String(binary);
            }
            else
            {
                var ssaContent = subRipTextFile.ToSSA();

                var translator = new SSATranslator("CCCCCC", "80ff00", "80ff00", new TimeSpan(0, 0, 4));
                translator.Translate(ssaContent, translations.ToList(), removeKnownDialogs);
                var translatedContent = ssaContent.Compile();
                var binary = UTF8Encoding.UTF8.GetBytes(translatedContent);
                cargo.BinaryBase64 = Convert.ToBase64String(binary);

                ChangeExtensionToAss(cargo);
            }
        }

        public void Parse(Cargo cargo, string decodedData)
        {
            var parser = new SubRipTextFileParser(new SRTParserVisitor());
            var result = parser.Parse(decodedData, cargo.Name);

            int blockId = 0;
            cargo.HarborData = new GlobalHarborData
            {
                LineEnding = result.NewLine,
                LearningUnits = result.LearningUnits
            };
            cargo.Blocks = result.Subtitles.Select(s => s.ToBlockWithId(blockId++)).ToList();
        }

        private void ChangeExtensionToAss(Cargo cargo)
        {
            var indexOfExtension = cargo.Name.ToLower().IndexOf(".srt");
            if (indexOfExtension > -1)
            {
                cargo.Name = cargo.Name.Substring(0, indexOfExtension) + ".ass";
            }
        }

        private bool AreCorrect(Dictionary<string, bool> translationOptions)
        {
            if (translationOptions == null)
            {
                Logger.Error("TranslateOptions are null for cargo: " + this.ToString());
                return false;
            }

            string[] keys =
            {
                "SrtOutput",
                "RemoveKnownDialogs"
            };

            var correct = true;
            foreach (var key in keys)
            {
                if (!translationOptions.ContainsKey(key))
                {
                    Logger.Error("TranslateOptions don't contain " + key);
                    correct = false;
                }
            }
            return correct;
        }
    }
}
