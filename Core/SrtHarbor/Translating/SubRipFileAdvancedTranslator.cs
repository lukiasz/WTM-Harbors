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
    /// <summary>
    /// Advanced SRT Translator.
    /// 
    /// Rules:
    /// 1. it shouldn't translate every word infinitely times
    /// 2. translations inline are not convenient, so if possible it
    ///    displays translations earlier
    /// 3. known dialogs may be removed
    /// </summary>
    public class SubRipFileAdvancedTranslator 
    {
        static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public TimeSpan MinBreakBetweenSubs = new TimeSpan(0, 0, 3);
        public TimeSpan DesiredSubsDisplayTime = new TimeSpan(0, 0, 15);

        public const int maxTranslationsPerSubs = 3;

        readonly string translationHTMLFormat;
        readonly string colorMarkHTMLFormat;

        int maxTranslationOccurences { get; set; }
        public bool InsertBeforeAndInline { get; set; }

        HashSet<string> notFoundTranslations = new HashSet<string>();

        public SubRipFileAdvancedTranslator(Color color, string fontName,
            int maxTranslationsOccurence, bool insertBeforeAndInline = false)
        {
            this.maxTranslationOccurences = maxTranslationsOccurence;
            var colorStr = String.Format("#{0:X2}{1:X2}{2:X2}",
                color.R, color.G, color.B);

            translationHTMLFormat = String
                .Format("<font color=\"{0}\" face=\"{1}\"> ({2})</font>",
                colorStr, fontName, "{0}");

            colorMarkHTMLFormat = String
                .Format("<font color=\"{0}\" face=\"{1}\">{2}</font>",
                colorStr, fontName, "{0}");

            this.InsertBeforeAndInline = insertBeforeAndInline;
        }

        public void Translate(SRTFile content,
            List<Translation> deserializedTranslations, bool removeKnownDialogs)
        {
            // orderby should take care of caption position, sentence position
            // and word position
            Func<Translation, int>
                orderbyPredicate = t =>
                    100 * t.Position.CaptionPos +
                    t.Position.StartPos;


            if (removeKnownDialogs)
            {
                content.Subtitles
                    .RemoveDialogsWithoutAnyTranslations(deserializedTranslations);
            }

            // first, let's count duplicated translations and remove those
            // which occur too many times.
            var translationsWithoutDups = deserializedTranslations
                .RemoveDuplicatedTranslationsWithCaptionCheck(maxTranslationOccurences);

            var i = translationsWithoutDups.Count - 1; 
            while (i >= 0)
            {
                // calculate how big break we need. By default is 3x3 seconds
                var minimumBreak = MinBreakBetweenSubs
                    + MinBreakBetweenSubs
                    + MinBreakBetweenSubs;

                var currentSubId = translationsWithoutDups[i].Position.CaptionPos;

                // find nearest preceding break
                var afterBreakFirstSubId = content.Subtitles
                    .FindIdOfFirstCaptionAfterNearestPrecedingBreak(minimumBreak,
                    currentSubId);

                
                bool insertAllOnlyInline = false;

                // it means that there is no break at all
                // in that case we'll need to insert all subs inline
                if (afterBreakFirstSubId == -1)
                    insertAllOnlyInline = true;

                // get all translations from subs[id] to subs [currentSubsId]
                var localTranslations = translationsWithoutDups
                    .Where(t => t.Position.CaptionPos >= afterBreakFirstSubId &&
                                t.Position.CaptionPos <= currentSubId)
                                .OrderBy(orderbyPredicate).ToList();

                               
                // caption with translations. Create but not insert yet
                SRTCaption translationsCaption = null;
                if (!insertAllOnlyInline)
                    translationsCaption = CreateTranslationsSubtitle(content,
                        afterBreakFirstSubId, localTranslations);

                // so, we'll put inline translations when j > maxTranslationsPerSubs,
                // otherwise we'll just color word
                for (var j = localTranslations.Count - 1; j >= 0; --j)
                {
                    var t = localTranslations[j];
                    
                    // if there is no translation for this word, log it 
                    if (t.TranslatedText == null || t.TranslatedText == "-")
                    {
                        var word = content.Subtitles.GetRelatedWord(t);
                        LogWordWithoutTranslation(word);
                        continue;
                    }
                    else if (j > maxTranslationsPerSubs - 1 || insertAllOnlyInline)
                    {
                        
                        content.Subtitles
                            .InsertTranslation(t, translationHTMLFormat);
                    }
                    else
                    {
                        if (InsertBeforeAndInline)
                        {
                            // let's try to show translations either before subs 
                            // and inline
                            content.Subtitles
                                .InsertTranslation(t, translationHTMLFormat);
                        }
                        else
                        {
                            content.Subtitles
                            .ColorWord(t, colorMarkHTMLFormat);
                        }
                    }
                }
                
                // insert translation caption
                if (!insertAllOnlyInline && translationsCaption != null)
                    content.Subtitles.Insert(afterBreakFirstSubId, translationsCaption);

                i -= localTranslations.Count;
            }


            FlushNotFoundTranslationsLog();
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

        SRTCaption CreateTranslationsSubtitle(SRTFile content,
            int afterBreakSubsId,
            List<Translation> localTranslations)
        {
            // this subtitle should be visible 
            // three seconds after previous subtitles:
            var timeAfterPrevSub = afterBreakSubsId > 0 ?
                content.Subtitles[afterBreakSubsId - 1].End + MinBreakBetweenSubs
                : MinBreakBetweenSubs;

            // or three seconds before next subtitles:
            var timeBeforePrevSub = content.Subtitles[afterBreakSubsId].Start
                - MinBreakBetweenSubs - DesiredSubsDisplayTime;

            // we choose timespan that is bigger (occurs later)
            var newSubsStartTime = timeAfterPrevSub > timeBeforePrevSub ?
                timeAfterPrevSub : timeBeforePrevSub;

            var newSubsEndTime = content.Subtitles[afterBreakSubsId].Start
                - MinBreakBetweenSubs;

            var newSubsContent = GetNewSubsContent(localTranslations, content);

            // in case when we've got unknown words but don't have translations :(
            if (newSubsContent.Count() == 0)
                return null;

            var newSubs = new SRTCaption
            {
                End = newSubsEndTime,
                Start = newSubsStartTime,
                Text = newSubsContent 
            };
            return newSubs;
        }

        private string GetNewSubsContent(
            List<Translation> translations, SRTFile content)
        {
            var newLineAppended = false;
            var sb = new StringBuilder();
            var appendedLines = new List<string>();
            for (var i = 0; i < translations.Count && i < 3; ++i)
            {
                var t = translations[i];
                var word = content.Subtitles.GetRelatedWord(t);

                if (t.TranslatedText == null || t.TranslatedText == "-")
                {
                    LogWordWithoutTranslation(word);
                    continue;
                }
                var translation = ColorText(String.Format("{0} - {1}",
                    word, t.TranslatedText));


                appendedLines.Add(translation);

                sb.AppendFormat(translation);
                sb.Append(content.NewLine);
                newLineAppended = true;
            }
            if (newLineAppended)
                sb.Remove(sb.Length - content.NewLine.Length, content.NewLine.Length);
            return sb.ToString();
        }

        private string ColorText(string text)
        {
            return String.Format(colorMarkHTMLFormat, text);
        }
    }
}

