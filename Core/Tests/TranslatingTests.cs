using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Should;
using TextHarbor;

namespace Tests
{
    public class TranslatingTests
    {
        [Fact]
        public void translate_single_block_single_word___works()
        {
            // Arrange
            var sentence = "Find an answer or ask a question in our community.";

            var sentenceAsBinary = UTF8Encoding.UTF8.GetBytes(sentence);
            var binaryBase64 = Convert.ToBase64String(sentenceAsBinary);

            var cargo = new Cargo
            {
                Blocks = new List<Block>
                {
                    new Block
                    {
                        HarborData = new HarborData
                        {
                            BlockIndex = 0,
                            BlockLength = sentence.Length
                        },
                        LearningData = new LearningData
                        {
                            Words = new List<Word>
                            {
                                new Word
                                {
                                    Knowledge = Knowledge.NotKnown,
                                    Length = "answer".Length,
                                    ShortTranslation = "odpowiedz",
                                    StartIndex = sentence.IndexOf("answer")
                                }
                            }
                        },
                        Text = sentence
                    }
                },
                BinaryBase64 = binaryBase64
            };

            var translationOptions = new Dictionary<string, bool>
            {
                { "InsertTranslationsBeforeOriginalWords", false },
                { "SkipIdenticalTranslations", false }
            };
           
            // Act
            cargo.Translate(sentence, translationOptions);

            // Assert
            var binary = Convert.FromBase64String(cargo.BinaryBase64);
            var str = UTF8Encoding.UTF8.GetString(binary);
            str.ShouldEqual("Find an answer (odpowiedz) or ask a question in our community.");
        }
    }
}
