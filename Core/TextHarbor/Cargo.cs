using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace TextHarbor
{
    public class Cargo
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        public List<Block> Blocks { get; set; }
        public string BinaryBase64 { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string ContentType { get; set; }

        public void Translate(string decodedData,
            Dictionary<string, bool> translationOptions)
        {
            if (!AreCorrect(translationOptions))
                throw new InvalidOperationException("Translations options are not correct!");

            var skipIdenticalTranslations = (bool)translationOptions["SkipIdenticalTranslations"];
            var insertTranslationsBeforeOriginalWords 
                = (bool)translationOptions["InsertTranslationsBeforeOriginalWords"];

            var blocksFromLast = Blocks
                .OrderByDescending(c => c.HarborData.BlockIndex);

            foreach (var block in blocksFromLast)
            {
                var wordsFromLast = block.LearningData.Words
                    .OrderByDescending(w => w.StartIndex);

                foreach (var word in wordsFromLast)
                {
                    if (skipIdenticalTranslations &&
                        word.ShortTranslation.ToUpper() == decodedData
                            .Substring(word.StartIndex + block.HarborData.BlockIndex, word.Length).ToUpper())
                        continue;

                    if (insertTranslationsBeforeOriginalWords)
                    {
                        var translationPos = word.StartIndex + block.HarborData.BlockIndex;
                        decodedData = decodedData.Insert(translationPos, "(" + word.ShortTranslation + ") ");
                    }
                    else
                    {
                        var translationPos = word.StartIndex + block.HarborData.BlockIndex +
                           word.Length;

                        decodedData = decodedData.Insert(translationPos, " (" + word.ShortTranslation + ")");
                    }
                }
            }

            var binary = UTF8Encoding.UTF8.GetBytes(decodedData);
            BinaryBase64 = Convert.ToBase64String(binary);
        }

        public void Parse(string decodedData)
        {
            Blocks = decodedData.GetBlocks();
        }

        public bool AreCorrect(Dictionary<string, bool> translationOptions)
        {
            if (translationOptions == null)
            {
                Logger.Error("TranslateOptions are null for cargo: " + this.ToString());
                return false;
            }

            string[] keys =
            {
                "SkipIdenticalTranslations",
                "InsertTranslationsBeforeOriginalWords"
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

        public override string ToString()
        {
            return Name + "( " + Type + ", " + ContentType + ") ";
        }
    }
}