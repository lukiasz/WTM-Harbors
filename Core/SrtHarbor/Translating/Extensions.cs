using SrtHarbor.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SrtHarbor.Translating
{
    public static class Extensions
    {
        /// <summary>
        /// Removes translations when they occurs too many times.
        /// Also, it removes duplicated translations of duplicated words in same caption.
        /// </summary>
        /// <param name="translations"></param>
        /// <param name="content"></param>
        /// <param name="maxTranslationOccurences"></param>
        /// <returns></returns>
        internal static List<Translation>
            RemoveDuplicatedTranslationsWithCaptionCheck(
            this List<Translation> translations,
            int maxTranslationOccurences)    
        {
            var clearedTranslations = new
                    List<Translation>();

            // I assume they're ordered chronologically
            var translationsCount = new Dictionary<string, int>();

            var lastOccurrences = new Dictionary<string, int>();
            foreach (var t in translations)
            {
                var translationText = t.TranslatedText != null
                    ? t.TranslatedText.ToLower()
                    : "null";
  
                int wordCount = 0;
                int lastWordCaptionPos = int.MinValue;
                
                // Count in whole file
                if (translationsCount.TryGetValue(translationText, out wordCount))
                    ++translationsCount[translationText];
                else
                    translationsCount[translationText] = 1;

                // translation was inserted not so many times
                if (translationsCount[translationText] <= maxTranslationOccurences)
                {
                    // check in which caption was the last occurrence of that translation
                    if (lastOccurrences.TryGetValue(translationText, out lastWordCaptionPos))
                    {
                        int diffInCaptions = t.Position.CaptionPos - lastWordCaptionPos;
                        if (diffInCaptions > 0)
                        {
                            lastOccurrences[translationText] = t.Position.CaptionPos;
                            clearedTranslations.Add(t);
                        }
                        else
                        {
                            // if difference between same translation occurence
                            // is lower than two captions, there is no sense
                            // in displaying that translation.
                            --translationsCount[translationText];
                        }

                    }
                    else
                    {
                        translationsCount[translationText] = 1;
                        lastOccurrences[translationText] = t.Position.CaptionPos;
                        clearedTranslations.Add(t);
                    }
                }
                else
                {
                    // translation was inserted too many times, do nothing
                }
            }

            return clearedTranslations;
        }

        /// <summary>
        /// Removes translations when they occurs too many times.
        /// </summary>
        /// <param name="translations"></param>
        /// <param name="content"></param>
        /// <param name="maxTranslationOccurences"></param>
        /// <returns></returns>
        internal static List<Translation>
            RemoveDuplicatedTranslations(
            this List<Translation> translations, int maxTranslationOccurences)
        {
            var clearedTranslations = new
                    List<Translation>();

            // I assume they're ordered chronologically
            var translationsCount = new Dictionary<string, int>();
            foreach (var t in translations)
            {
                var translationText = t.TranslatedText != null 
                    ? t.TranslatedText.ToLower() 
                    : "null";

                int wordCount = 0;
                if (translationsCount.TryGetValue(translationText, out wordCount))
                    ++translationsCount[translationText];
                else
                    translationsCount[translationText] = 1;

                if (translationsCount[translationText] <= maxTranslationOccurences)
                    clearedTranslations.Add(t);
            }

            return clearedTranslations;
        }

        /// <summary>
        /// Removes all dialogs which don't have translations provided
        /// </summary>
        /// <param name="subtitles"></param>
        /// <param name="translations"></param>
        internal static void RemoveDialogsWithoutAnyTranslations(
            this IList<SRTCaption> subtitles,
            IList<Translation> translations)
        {
            int prevCaptionPos = 0;
            int i = 0;

            // I assume translations are ordered chronologically
            for (i = 0; i < translations.Count; ++i)
            {
                var t = translations[i];
                var captionPos = t.Position.CaptionPos;

                // remove all subs from prevCaptionPos to captionPos
                // that number might be less than zero in case when we're have
                // multiple translations in single caption.
                var subsToRemove = Math.Max(captionPos - prevCaptionPos, 0);
                subtitles.RemoveRange(prevCaptionPos, subsToRemove);

                // fix CaptionPos indices in translations
                for (var j = i; j < translations.Count; ++j)
                {
                    translations[j].Position.CaptionPos -= subsToRemove;
                }
                
                // we're left one caption because it has translation.
                // increment position
                ++prevCaptionPos;
            }

            // remove subs from end
            var subsToRemove2 = subtitles.Count - prevCaptionPos;
            subtitles.RemoveRange(prevCaptionPos, subsToRemove2);
        }

        /// <summary>
        /// RemoveRange extension for IList. Not optimized at all.
        /// </summary>
        /// <param name="subtitles"></param>
        /// <param name="index"></param>
        /// <param name="count"></param>
        internal static void RemoveRange<T>(this IList<T> elements,
            int index, int count)
        {
            for (var k = 0; k < count; ++k)
            {
                elements.RemoveAt(index);
            }
        }

        /// <summary>
        /// Get word for which translation is provided
        /// </summary>
        /// <param name="subtitles"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        internal static string GetRelatedWord(this IList<SRTCaption> subtitles,
            Translation t)
        {
            var word = subtitles[t.Position.CaptionPos].Text
                .Substring(t.Position.StartPos,
                t.Position.EndPos - t.Position.StartPos);
            return word;
        }

        /// <summary>
        /// Insert single translation inline (with "()" braces)
        /// </summary>
        /// <param name="subtitles"></param>
        /// <param name="t"></param>
        /// <param name="format"></param>
        internal static void InsertTranslation(this IList<SRTCaption> subtitles,
            Translation t, string format)
        {
            subtitles[t.Position.CaptionPos].Text
                =
                subtitles[t.Position.CaptionPos].Text
                .Insert(t.Position.EndPos,
                    String.Format(format, t.TranslatedText));
        }

        /// <summary>
        /// Color word for which translation is provided
        /// </summary>
        /// <param name="subtitles"></param>
        /// <param name="t"></param>
        /// <param name="format"></param>
        internal static void ColorWord(this IList<SRTCaption> subtitles,
            Translation t, string format)
        {
            var word = subtitles.GetRelatedWord(t);

            // remove word
            subtitles[t.Position.CaptionPos].Text
            =
            subtitles[t.Position.CaptionPos].Text.Remove(t.Position.StartPos,
                    t.Position.EndPos - t.Position.StartPos);

            // insert formatted word
            var insertedFormattedWord = String.Format(format, word);

            subtitles[t.Position.CaptionPos].Text
            =
            subtitles[t.Position.CaptionPos].Text
            .Insert(t.Position.StartPos, insertedFormattedWord);
        }

        /// <summary>
        /// Finds id of first caption after nearest preceding break.
        /// If there is no break at all, it'll return -1
        /// </summary>
        /// <param name="subs"></param>
        /// <param name="minBreakLength"></param>
        /// <param name="currentSubId"></param>
        /// <returns></returns>
        internal static int FindIdOfFirstCaptionAfterNearestPrecedingBreak(this IList<SRTCaption> subs,
            TimeSpan minBreakLength, int currentSubId)
        {
            for (var i = currentSubId; i > 0; --i)
            {
                var currentSubs = subs[i];
                var prevSubs = subs[i - 1];
                var timeDiff = currentSubs.Start - prevSubs.End;
                if (timeDiff > minBreakLength)
                    return i;
            }
            // now we're dealing with situation when we're at beginning
            var timeBeforeFirstSubs = subs[0].Start;
            if (timeBeforeFirstSubs > minBreakLength)
                return 0;
            return -1;
        }
    }
}
