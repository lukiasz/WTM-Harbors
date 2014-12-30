using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using SrtHarbor.Model;

namespace SrtHarbor.Translating
{
    public class SSATranslator
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();
        HashSet<string> notFoundTranslations = new HashSet<string>();
        readonly string translationActiveStyle;
        readonly string translationInactiveStyle;
        TimeSpan translationInactiveDisplayTime;
        readonly string activeInlineColorInHex;

        public SSATranslator(string inactiveColor,
            string activeColor, string activeInlineColorInHex,
            TimeSpan translationInactiveDisplayTime)
        {
            translationActiveStyle = String.Format("TranslationActive,Arial,22,H{0},0,16777215,0,0,0,0,1,0,6,20,20,10,0",
                activeColor);

            translationInactiveStyle = String.Format("TranslationInactive,Arial,22,H{0},0,16777215,0,0,0,0,1,0,6,20,20,10,0",
                inactiveColor);

            this.activeInlineColorInHex = activeInlineColorInHex;
            this.translationInactiveDisplayTime = translationInactiveDisplayTime;
        }

        public void Translate(SSAFile content, IList<Translation> translations,
            bool removeKnownDialogs)
        {
            // insert additional styles
            content.V4Styles.Styles.Add(translationActiveStyle);
            content.V4Styles.Styles.Add(translationInactiveStyle);

            // SSA does not care about ordering, but we do because
            // sometimes we need to color words that occur in same sentence.
            // counting from end allows to perform insertions into sentences without
            // any checking if indices have changed.
            Func<Translation, int>
                    orderbyPredicate = t =>
                        100 * t.Position.CaptionPos +
                        t.Position.StartPos;

            var deserializedTranslations = translations
                .OrderBy(orderbyPredicate).ToList().RemoveDuplicatedTranslations(3);


            if (removeKnownDialogs)
            {
                content.Events.Dialogues
                    .RemoveDialogsWithoutAnyTranslations(deserializedTranslations);
            }

            var insertedCaptions = new List<SSACaption>();

            // for each translation, insert:
            // inactive translation 3 sec before,
            // active translation at caption displaying time
            // inactive translation 3 sec after
            for (var i = deserializedTranslations.Count - 1; i >= 0; --i)
            {
                var captions = content.Events.Dialogues;
                var t = deserializedTranslations[i];

                var currentCaption = captions[t.Position.CaptionPos];
                var word = captions.GetRelatedWord(t);

                if (t.TranslatedText == null || t.TranslatedText == "-")
                {
                    LogWordWithoutTranslation(word);
                }
                else
                {
                    currentCaption.ColorWord(t, word, activeInlineColorInHex);
                    var currentTranslationSubs = CreateTranslationsSubs(t,
                    currentCaption, word);

                    insertedCaptions.AddRange(currentTranslationSubs);
                }
            }

            insertedCaptions.MergeTranslationsSubs();
            content.Events.Dialogues.AddRange(insertedCaptions);
            FlushNotFoundTranslationsLog();
        }

        private List<SSACaption> CreateTranslationsSubs(Translation t,
            SSACaption relatedCaption, string foundWord)
        {
            var list = new List<SSACaption>(3);
            var translationContent = foundWord + " - " + t.TranslatedText;

            // prev inactive
            list.Add(new SSACaption
            {
                // caption may come very soon in movie and in that case we'll
                // have negative timespan.
                Start = (relatedCaption.Start - translationInactiveDisplayTime)
                        < new TimeSpan(0, 0, 0) ?
                    new TimeSpan(0, 0, 0) 
                    : relatedCaption.Start - translationInactiveDisplayTime,

                End = relatedCaption.Start,
                StyleName = "TranslationInactive",
                Text = translationContent
            });

            // active
            list.Add(new SSACaption
            {
                Start = relatedCaption.Start,
                End = relatedCaption.End,
                StyleName = "TranslationActive",
                Text = translationContent
            });

            // after inactive
            list.Add(new SSACaption
            {
                Start = relatedCaption.End,
                End = relatedCaption.End + translationInactiveDisplayTime,
                StyleName = "TranslationInactive",
                Text = translationContent
            });

            return list;
        }


        void LogWordWithoutTranslation(string word)
        {
            notFoundTranslations.Add(word);
        }

        void FlushNotFoundTranslationsLog()
        {
            foreach (var word in notFoundTranslations)
            {
                logger.Warn(word + ": translation not found");
            }
            notFoundTranslations.Clear();
        }
    }
}
