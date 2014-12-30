using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Should;
using SrtHarbor.Translating;
using SrtHarbor.Parsing;
using SrtHarbor.Model;
using SrtHarbor.Converting;

namespace Translators.Tests.Unit
{
    public class SSATranslatorTests
    {
        private readonly SSATranslator translator;

        public SSATranslatorTests()
        {
            translator = new SSATranslator("CCCCCC", "80ff00", "80ff00", new TimeSpan(0, 0, 3));
        }

        [Fact]
        public void basic_scenario1___single_translation_is_inserted_properly()
        {
            var srtText = @"1
00:00:44,335 --> 00:00:45,859
How about a ride Mister?

2
00:01:46,537 --> 00:01:51,875
Jennifer... oh man, aren't you a sight for sore eyes.

";
            var translations = new List<Translation>
            {
                new Translation
                {
                    // sight
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 1, EndPos = 38,
                        StartPos = 33
                    },
                    TranslatedText = "TLUMACZENIE_SIGHT"
                }
            };

            var translatedContent = Translate(srtText, translations, false);
            var result = translatedContent.Compile();

            var expectedResult = @"[Script Info]
ScriptType: v4.00
Collisions: Normal
Timer: 100.0000

[V4 Styles]
Format: Name, Fontname, Fontsize, PrimaryColour, SecondaryColour, TertiaryColour, BackColour, Bold, Italic, BorderStyle, Outline, Shadow, Alignment, MarginL, MarginR, MarginV, AlphaLevel, Encoding
Style: Default,Arial,22,HFFFFFF,0,HFFFFFF,0,0,0,0,1,0,2,20,20,10,0
Style: TranslationActive,Arial,22,H80ff00,0,16777215,0,0,0,0,1,0,6,20,20,10,0
Style: TranslationInactive,Arial,22,HCCCCCC,0,16777215,0,0,0,0,1,0,6,20,20,10,0

[Events]
Format: Marked, Start, End, Style, Name, MarginL, MarginR, MarginV, Effect, Text
Dialogue: Marked=0,0:00:44.33,0:00:45.85,Default,NTP,0000,0000,0000,!Effect,How about a ride Mister?
Dialogue: Marked=0,0:01:46.53,0:01:51.87,Default,NTP,0000,0000,0000,!Effect,Jennifer... oh man, aren't you a {\c&H80ff00&}sight{\c} for sore eyes.
Dialogue: Marked=0,0:01:43.53,0:01:46.53,TranslationInactive,NTP,0000,0000,0000,!Effect,sight - TLUMACZENIE_SIGHT
Dialogue: Marked=0,0:01:46.53,0:01:51.87,TranslationActive,NTP,0000,0000,0000,!Effect,sight - TLUMACZENIE_SIGHT
Dialogue: Marked=0,0:01:51.87,0:01:54.87,TranslationInactive,NTP,0000,0000,0000,!Effect,sight - TLUMACZENIE_SIGHT

";

            result.ShouldEqual(expectedResult);
        }


        [Fact]
        public void basic_scenario2___knownDialogsAreRemoved()
        {
            var srtText = @"1
00:00:44,335 --> 00:00:45,859
How about a ride Mister?

2
00:01:46,537 --> 00:01:51,875
Jennifer... oh man, aren't you a sight for sore eyes.

";
            var translations = new List<Translation>
            {
                new Translation
                {
                    // sight
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 1, EndPos = 38,
                        StartPos = 33
                    },
                    TranslatedText = "TLUMACZENIE_SIGHT"
                }
            };

            var translatedContent = Translate(srtText, translations, true);
            var result = translatedContent.Compile();
            var expectedResult = @"[Script Info]
ScriptType: v4.00
Collisions: Normal
Timer: 100.0000

[V4 Styles]
Format: Name, Fontname, Fontsize, PrimaryColour, SecondaryColour, TertiaryColour, BackColour, Bold, Italic, BorderStyle, Outline, Shadow, Alignment, MarginL, MarginR, MarginV, AlphaLevel, Encoding
Style: Default,Arial,22,HFFFFFF,0,HFFFFFF,0,0,0,0,1,0,2,20,20,10,0
Style: TranslationActive,Arial,22,H80ff00,0,16777215,0,0,0,0,1,0,6,20,20,10,0
Style: TranslationInactive,Arial,22,HCCCCCC,0,16777215,0,0,0,0,1,0,6,20,20,10,0

[Events]
Format: Marked, Start, End, Style, Name, MarginL, MarginR, MarginV, Effect, Text
Dialogue: Marked=0,0:01:46.53,0:01:51.87,Default,NTP,0000,0000,0000,!Effect,Jennifer... oh man, aren't you a {\c&H80ff00&}sight{\c} for sore eyes.
Dialogue: Marked=0,0:01:43.53,0:01:46.53,TranslationInactive,NTP,0000,0000,0000,!Effect,sight - TLUMACZENIE_SIGHT
Dialogue: Marked=0,0:01:46.53,0:01:51.87,TranslationActive,NTP,0000,0000,0000,!Effect,sight - TLUMACZENIE_SIGHT
Dialogue: Marked=0,0:01:51.87,0:01:54.87,TranslationInactive,NTP,0000,0000,0000,!Effect,sight - TLUMACZENIE_SIGHT

";

            result.ShouldEqual(expectedResult);
        }

        [Fact]
        public void basic_scenario3___multiple_translations_into_single_sentence_are_inserted_correctly()
        {
            var srtText = @"1
00:00:44,335 --> 00:00:45,859
How about a ride Mister?

2
00:01:46,537 --> 00:01:51,875
Jennifer... oh man, aren't you a sight for sore eyes.

";
            var translations = new List<Translation>
            {
                new Translation
                {
                    // sight
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 1, EndPos = 38,
                        StartPos = 33
                    },
                    TranslatedText = "TLUMACZENIE_SIGHT"
                },
                new Translation
                {
                    // sore
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 1, EndPos = 47,
                        StartPos = 43
                    },
                    TranslatedText = "TLUMACZENIE_SORE"
                }
            };
      

            var translatedContent = Translate(srtText, translations, true);
            var result = translatedContent.Compile();
            var expectedResult = @"[Script Info]
ScriptType: v4.00
Collisions: Normal
Timer: 100.0000

[V4 Styles]
Format: Name, Fontname, Fontsize, PrimaryColour, SecondaryColour, TertiaryColour, BackColour, Bold, Italic, BorderStyle, Outline, Shadow, Alignment, MarginL, MarginR, MarginV, AlphaLevel, Encoding
Style: Default,Arial,22,HFFFFFF,0,HFFFFFF,0,0,0,0,1,0,2,20,20,10,0
Style: TranslationActive,Arial,22,H80ff00,0,16777215,0,0,0,0,1,0,6,20,20,10,0
Style: TranslationInactive,Arial,22,HCCCCCC,0,16777215,0,0,0,0,1,0,6,20,20,10,0

[Events]
Format: Marked, Start, End, Style, Name, MarginL, MarginR, MarginV, Effect, Text
Dialogue: Marked=0,0:01:46.53,0:01:51.87,Default,NTP,0000,0000,0000,!Effect,Jennifer... oh man, aren't you a {\c&H80ff00&}sight{\c} for {\c&H80ff00&}sore{\c} eyes.
Dialogue: Marked=0,0:01:43.53,0:01:46.53,TranslationInactive,NTP,0000,0000,0000,!Effect,sore - TLUMACZENIE_SORE
Dialogue: Marked=0,0:01:43.53,0:01:46.53,TranslationInactive,NTP,0000,0000,0000,!Effect,sight - TLUMACZENIE_SIGHT
Dialogue: Marked=0,0:01:46.53,0:01:51.87,TranslationActive,NTP,0000,0000,0000,!Effect,sore - TLUMACZENIE_SORE
Dialogue: Marked=0,0:01:46.53,0:01:51.87,TranslationActive,NTP,0000,0000,0000,!Effect,sight - TLUMACZENIE_SIGHT
Dialogue: Marked=0,0:01:51.87,0:01:54.87,TranslationInactive,NTP,0000,0000,0000,!Effect,sore - TLUMACZENIE_SORE
Dialogue: Marked=0,0:01:51.87,0:01:54.87,TranslationInactive,NTP,0000,0000,0000,!Effect,sight - TLUMACZENIE_SIGHT

";

            result.ShouldEqual(expectedResult);

        }

        [Fact]
        public void translation___is_null_or_equal_to_minus_sign___translation_is_not_inserted()
        {
            var srtText = @"1
00:00:00,400 --> 00:00:01,675
- What’s your years?
- Som.

2
00:00:02,720 --> 00:00:03,075
- You been years here for long?
- No. I've just started.

3
00:01:04,760 --> 00:01:05,799
I haven’t gone out for years.

4
00:01:06,960 --> 00:01:07,191
About 10 years.
I became a family man.

";
            var translations = new List<Translation>
            {
                new Translation()
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 0, EndPos = 8,
                        StartPos = 2
                    },
                    TranslatedText = "-"
                },
                new Translation()
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 1, EndPos = 16,
                        StartPos = 11
                    },
                    TranslatedText = "-"
                },
                new Translation()
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 2, EndPos = 28,
                        StartPos = 23
                    },
                    TranslatedText = null
                },
                new Translation()
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 3, EndPos = 14,
                        StartPos = 9
                    },
                    TranslatedText = null
                },
            };

            var translatedContent = Translate(srtText, translations, false);
            var result = translatedContent.Compile();

            var expectedSrtText = @"[Script Info]
ScriptType: v4.00
Collisions: Normal
Timer: 100.0000

[V4 Styles]
Format: Name, Fontname, Fontsize, PrimaryColour, SecondaryColour, TertiaryColour, BackColour, Bold, Italic, BorderStyle, Outline, Shadow, Alignment, MarginL, MarginR, MarginV, AlphaLevel, Encoding
Style: Default,Arial,22,HFFFFFF,0,HFFFFFF,0,0,0,0,1,0,2,20,20,10,0
Style: TranslationActive,Arial,22,H80ff00,0,16777215,0,0,0,0,1,0,6,20,20,10,0
Style: TranslationInactive,Arial,22,HCCCCCC,0,16777215,0,0,0,0,1,0,6,20,20,10,0

[Events]
Format: Marked, Start, End, Style, Name, MarginL, MarginR, MarginV, Effect, Text
Dialogue: Marked=0,0:00:00.40,0:00:01.67,Default,NTP,0000,0000,0000,!Effect,- What’s your years?\N- Som.
Dialogue: Marked=0,0:00:02.72,0:00:03.07,Default,NTP,0000,0000,0000,!Effect,- You been years here for long?\N- No. I've just started.
Dialogue: Marked=0,0:01:04.76,0:01:05.79,Default,NTP,0000,0000,0000,!Effect,I haven’t gone out for years.
Dialogue: Marked=0,0:01:06.96,0:01:07.19,Default,NTP,0000,0000,0000,!Effect,About 10 years.\NI became a family man.

";

            result.ShouldEqual(expectedSrtText);
        }


        [Fact]
        public void multiple_subtitles_are_equal___subtitles_are_merged()
        {
            var srtText = @"1
00:00:44,335 --> 00:00:45,859
How about a ride Mister?

2
00:01:46,537 --> 00:01:51,875
Jennifer... oh man, aren't you a eyes for sore eyes.

";
            var translations = new List<Translation>
            {
                new Translation
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 1, EndPos = 37,
                        StartPos = 33
                    },
                    TranslatedText = "TLUMACZENIE_EYES"
                },
                new Translation
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 1, EndPos = 51,
                        StartPos = 47
                    },
                    TranslatedText = "TLUMACZENIE_EYES"
                }
            };

            var translatedContent = Translate(srtText, translations, false);
            var result = translatedContent.Compile();

            var expectedResult = @"[Script Info]
ScriptType: v4.00
Collisions: Normal
Timer: 100.0000

[V4 Styles]
Format: Name, Fontname, Fontsize, PrimaryColour, SecondaryColour, TertiaryColour, BackColour, Bold, Italic, BorderStyle, Outline, Shadow, Alignment, MarginL, MarginR, MarginV, AlphaLevel, Encoding
Style: Default,Arial,22,HFFFFFF,0,HFFFFFF,0,0,0,0,1,0,2,20,20,10,0
Style: TranslationActive,Arial,22,H80ff00,0,16777215,0,0,0,0,1,0,6,20,20,10,0
Style: TranslationInactive,Arial,22,HCCCCCC,0,16777215,0,0,0,0,1,0,6,20,20,10,0

[Events]
Format: Marked, Start, End, Style, Name, MarginL, MarginR, MarginV, Effect, Text
Dialogue: Marked=0,0:00:44.33,0:00:45.85,Default,NTP,0000,0000,0000,!Effect,How about a ride Mister?
Dialogue: Marked=0,0:01:46.53,0:01:51.87,Default,NTP,0000,0000,0000,!Effect,Jennifer... oh man, aren't you a {\c&H80ff00&}eyes{\c} for sore {\c&H80ff00&}eyes{\c}.
Dialogue: Marked=0,0:01:43.53,0:01:46.53,TranslationInactive,NTP,0000,0000,0000,!Effect,eyes - TLUMACZENIE_EYES
Dialogue: Marked=0,0:01:46.53,0:01:51.87,TranslationActive,NTP,0000,0000,0000,!Effect,eyes - TLUMACZENIE_EYES
Dialogue: Marked=0,0:01:51.87,0:01:54.87,TranslationInactive,NTP,0000,0000,0000,!Effect,eyes - TLUMACZENIE_EYES

";

            result.ShouldEqual(expectedResult);
        }


        [Fact]
        public void multiple_translations_are_overlapping___translations_are_merged()
        {
            var srtText = @"1
00:00:44,335 --> 00:00:45,859
How about a sight Mister?

2
00:00:46,537 --> 00:00:51,875
Jennifer... oh man, aren't you a sight for sore eyes.

";
            var translations = new List<Translation>
            {
                new Translation
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 0, EndPos = 17,
                        StartPos = 12
                    },
                    TranslatedText = "TLUMACZENIE_SIGHT"
                },
                new Translation
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 1, EndPos = 38,
                        StartPos = 33
                    },
                    TranslatedText = "TLUMACZENIE_SIGHT"
                }
            };


            var translatedContent = Translate(srtText, translations, true);
            var result = translatedContent.Compile();
            var expectedResult = @"[Script Info]
ScriptType: v4.00
Collisions: Normal
Timer: 100.0000

[V4 Styles]
Format: Name, Fontname, Fontsize, PrimaryColour, SecondaryColour, TertiaryColour, BackColour, Bold, Italic, BorderStyle, Outline, Shadow, Alignment, MarginL, MarginR, MarginV, AlphaLevel, Encoding
Style: Default,Arial,22,HFFFFFF,0,HFFFFFF,0,0,0,0,1,0,2,20,20,10,0
Style: TranslationActive,Arial,22,H80ff00,0,16777215,0,0,0,0,1,0,6,20,20,10,0
Style: TranslationInactive,Arial,22,HCCCCCC,0,16777215,0,0,0,0,1,0,6,20,20,10,0

[Events]
Format: Marked, Start, End, Style, Name, MarginL, MarginR, MarginV, Effect, Text
Dialogue: Marked=0,0:00:44.33,0:00:45.85,Default,NTP,0000,0000,0000,!Effect,How about a {\c&H80ff00&}sight{\c} Mister?
Dialogue: Marked=0,0:00:46.53,0:00:51.87,Default,NTP,0000,0000,0000,!Effect,Jennifer... oh man, aren't you a {\c&H80ff00&}sight{\c} for sore eyes.
Dialogue: Marked=0,0:00:45.85,0:00:46.53,TranslationInactive,NTP,0000,0000,0000,!Effect,sight - TLUMACZENIE_SIGHT
Dialogue: Marked=0,0:00:41.33,0:00:44.33,TranslationInactive,NTP,0000,0000,0000,!Effect,sight - TLUMACZENIE_SIGHT
Dialogue: Marked=0,0:00:44.33,0:00:45.85,TranslationActive,NTP,0000,0000,0000,!Effect,sight - TLUMACZENIE_SIGHT
Dialogue: Marked=0,0:00:46.53,0:00:51.87,TranslationActive,NTP,0000,0000,0000,!Effect,sight - TLUMACZENIE_SIGHT
Dialogue: Marked=0,0:00:51.87,0:00:54.87,TranslationInactive,NTP,0000,0000,0000,!Effect,sight - TLUMACZENIE_SIGHT

";

            result.ShouldEqual(expectedResult);

        }



        private SSAFile Translate(string srtText,
           List<Translation> translations, bool removeKnownDialogs)
        {
            var parser = new SubRipTextFileParser(new SRTParserVisitor());
            var content = parser.Parse(srtText, "fakeFile.srt");
            var ssaContent = content.ToSSA();
            translator.Translate(ssaContent, translations, removeKnownDialogs);
            return ssaContent;
        }
    }
}
