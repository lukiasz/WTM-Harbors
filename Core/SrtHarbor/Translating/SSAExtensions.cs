using SrtHarbor.Model;
using SrtHarbor.Translating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SrtHarbor.Translating
{
    public static class SSAExtensions
    {
        internal static string GetRelatedWord(this IList<SSACaption> subtitles,
            Translation t)
        {
            var word = subtitles[t.Position.CaptionPos].Text
                .Substring(t.Position.StartPos,
                t.Position.EndPos - t.Position.StartPos);

            return word;
        }

        internal static void ColorWord(this SSACaption originalCaption,
            Translation t,
            string word, string activeInlineColorInHex)
        {
            var sentenceText = originalCaption.Text;

            // remove word
            sentenceText = sentenceText
                .Remove(t.Position.StartPos, t.Position.EndPos - t.Position.StartPos);

            var coloredWordFormat = "{{\\c&H{0}&}}{1}{{\\c}}";
            var coloredWord = String.Format(coloredWordFormat,
                activeInlineColorInHex, word);

            sentenceText = sentenceText.Insert(t.Position.StartPos, coloredWord);

            originalCaption.Text = sentenceText;
        }

        internal static void RemoveDialogsWithoutAnyTranslations(
           this List<SSACaption> subtitles,
           List<Translation> translations)
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
            var subsToRemove2 = Math.Max(subtitles.Count - prevCaptionPos, 0);

            subtitles.RemoveRange(prevCaptionPos-1, subsToRemove2);
        }

        internal static void MergeTranslationsSubs(this List<SSACaption> subs)
        {
            subs.MergeSameStyleSubs();
            subs.MergeDifferentStyleSubs();
        }

        // Overlaping translations subtitles should be merged.  
        private static void MergeSameStyleSubs(this List<SSACaption> subs)
        {
            var ordered = subs.OrderBy(s => s.Start).ToList();
            for (int i = ordered.Count - 1; i >= 0; --i)
            {
                var currentCaption = ordered[i];
                for (var j = i + 1; j < ordered.Count; ++j)
                {
                    // when we go from end, there might be only one overlapping
                    // subs.
                    var overlappingCaption = ordered[j];
                    if (overlappingCaption.Text ==
                        currentCaption.Text &&
                        overlappingCaption.StyleName == currentCaption.StyleName &&
                        overlappingCaption.Start >= currentCaption.Start &&
                        overlappingCaption.Start < currentCaption.End)
                    {
                        var laterEnd = overlappingCaption.End > currentCaption.End ?
                            overlappingCaption.End : currentCaption.End;

                        currentCaption.End = laterEnd;
                        
                        // since main loop is going backwards, we can
                        // safely do that
                        ordered.RemoveAt(j);

                        // there shouldn't be any more overlapping subs
                        break;

                    }
                    else if (j > 10)
                    {
                        // since subtitles are ordered chronologically,
                        // there is no point in further search. Subtitles won't
                        // be placed so closely to each other. Even so, they
                        // won't be readable at all in that case.
                        break;
                    }
                }
            }

            subs.Clear();
            subs.InsertRange(0, ordered);
        }

        private static void MergeDifferentStyleSubs(this List<SSACaption> subs)
        {
            var ordered = subs.OrderBy(s => s.Start).ToList();
            for (int i = ordered.Count - 1; i >= 0; --i)
            {
                var currentCaption = ordered[i];
                for (var j = i + 1; j < ordered.Count; ++j)
                {
                    // when we go from end, there might be only one overlapping
                    // subs.
                    var overlappingCaption = ordered[j];
                    if (overlappingCaption.Text ==
                        currentCaption.Text &&
                        overlappingCaption.StyleName != currentCaption.StyleName &&
                        overlappingCaption.Start >= currentCaption.Start &&
                        overlappingCaption.Start < currentCaption.End)
                    {

                        if (currentCaption.StyleName == "TranslationInactive")
                        {
                            var captionActive = overlappingCaption;
                            if (captionActive.End >= currentCaption.End)
                                currentCaption.End = captionActive.Start;
                            else
                            {
                                var newCaption = new SSACaption
                                {
                                    End = currentCaption.End,
                                    Text = currentCaption.Text,
                                    Start = captionActive.End,
                                    StyleName = "TranslationInactive"
                                };
                                ordered.Insert(i, newCaption);
                                i += 1;
                                currentCaption.End = captionActive.Start;
                            }

                        }
                        else // if caption.StyleName == "TranslationActive"
                        {
                            var captionInactive = overlappingCaption;
                            if (captionInactive.End >= currentCaption.End)
                                captionInactive.Start = currentCaption.End;
                            else
                            {
                                // situation when "CaptionActive" totally 
                                // covers "CaptionInactive"

                                // remove "CaptionInactive"
                                ordered.RemoveAt(j);
                            }
                        }

                        // there shouldn't be any more overlapping subs
                        break;

                    }
                    else if (j > 10)
                    {
                        // since subtitles are ordered chronologically,
                        // there is no point in further search. Subtitles won't
                        // be placed so closely to each other. Even so, they
                        // won't be readable at all in that case.
                        break;
                    }
                }
            }
            subs.Clear();
            subs.AddRange(ordered);
        }
    }
}
