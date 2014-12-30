using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Should;
using Xunit;
using SrtHarbor.Translating;
using SrtHarbor.Model;
using SrtHarbor.SubRipText;
using SrtHarbor.Parsing;

namespace Tests
{
    public class AdvancedSrtTranslatorsTests
    {
        private readonly SubRipFileAdvancedTranslator translator;
            
        public AdvancedSrtTranslatorsTests()
        {
            translator =
                new SubRipFileAdvancedTranslator(Color.Wheat, "FONT", 3);

            translator.DesiredSubsDisplayTime = new TimeSpan(0, 0, 10);
            translator.MinBreakBetweenSubs = new TimeSpan(0, 0, 5);
        }


        [Fact]
        public void insert_before_and_inline___works()
        {
            translator.InsertBeforeAndInline = true;


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
                        CaptionPos = 1,
                        EndPos = 38,
                        StartPos = 33
                    },
                    TranslatedText = "TLUMACZENIE_SIGHT"
                }
            };

            var translatedContent = Translate(srtText, translations, false);
            var result = translatedContent.Compile();

            var expectedSrtText = @"1
00:00:44,335 --> 00:00:45,859
How about a ride Mister?

2
00:01:31,537 --> 00:01:41,537
<font color=""#F5DEB3"" face=""FONT"">sight - TLUMACZENIE_SIGHT</font>

3
00:01:46,537 --> 00:01:51,875
Jennifer... oh man, aren't you a sight<font color=""#F5DEB3"" face=""FONT""> (TLUMACZENIE_SIGHT)</font> for sore eyes.

";
            result.ShouldEqual(expectedSrtText);
        }

        [Fact]
        public void basic_scenario1___single_translation_is_inserted_into_break()
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
                        CaptionPos = 1,
                        EndPos = 38,
                        StartPos = 33
                    },
                    TranslatedText = "TLUMACZENIE_SIGHT"
                }
            };

            var translatedContent = Translate(srtText, translations, false);
            var result = translatedContent.Compile();

            var expectedSrtText = @"1
00:00:44,335 --> 00:00:45,859
How about a ride Mister?

2
00:01:31,537 --> 00:01:41,537
<font color=""#F5DEB3"" face=""FONT"">sight - TLUMACZENIE_SIGHT</font>

3
00:01:46,537 --> 00:01:51,875
Jennifer... oh man, aren't you a <font color=""#F5DEB3"" face=""FONT"">sight</font> for sore eyes.

";
            result.ShouldEqual(expectedSrtText);
        }

        [Fact]
        public void basic_scenario2___three_translations_are_inserted_into_two_breaks()
        {
            // first break is before first dialog

            // second is before third dialog

            // we expect that translations will be inserted before first dialog
            // and before third.

            var srtText = @"1
00:00:21,400 --> 00:00:23,675
- What’s your name?
- Som.

2
00:00:24,720 --> 00:00:27,075
- You been working here for long?
- No. I've just started.

3
00:01:30,760 --> 00:01:34,799
I haven’t gone out for years.

4
00:01:35,960 --> 00:01:39,191
About 10 years.
I became a family man.

";

            var translations = new List<Translation>
            {
                new Translation()
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 0, EndPos = 18,
                        StartPos = 14
                    },
                    TranslatedText = "TLUMACZENIE_NAME"
                },
                new Translation()
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 2, EndPos = 14,
                        StartPos = 10
                    },
                    TranslatedText = "TLUMACZENIE_GONE"
                },
                new Translation()
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 3, EndPos = 14,
                        StartPos = 9
                    },
                    TranslatedText = "TLUMACZENIE_YEARS"
                },
            };

            var translatedContent = Translate(srtText, translations, false);
            var result = translatedContent.Compile();

            var expectedSrtText = @"1
00:00:06,400 --> 00:00:16,400
<font color=""#F5DEB3"" face=""FONT"">name - TLUMACZENIE_NAME</font>

2
00:00:21,400 --> 00:00:23,675
- What’s your <font color=""#F5DEB3"" face=""FONT"">name</font>?
- Som.

3
00:00:24,720 --> 00:00:27,075
- You been working here for long?
- No. I've just started.

4
00:01:15,760 --> 00:01:25,760
<font color=""#F5DEB3"" face=""FONT"">gone - TLUMACZENIE_GONE</font>
<font color=""#F5DEB3"" face=""FONT"">years - TLUMACZENIE_YEARS</font>

5
00:01:30,760 --> 00:01:34,799
I haven’t <font color=""#F5DEB3"" face=""FONT"">gone</font> out for years.

6
00:01:35,960 --> 00:01:39,191
About 10 <font color=""#F5DEB3"" face=""FONT"">years</font>.
I became a family man.

";
            result.ShouldEqual(expectedSrtText);
        }


        [Fact]
        public void basic_scenario3___three_translations_inserted_into_break_one_inline()
        {
            var srtText = @"1
00:00:21,400 --> 00:00:23,675
- What’s your name?
- Som.

2
00:00:24,720 --> 00:00:27,075
- You been working here for long?
- No. I've just started.

3
00:01:30,760 --> 00:01:34,799
I haven’t gone out for years.

4
00:01:35,960 --> 00:01:39,191
About 10 years.
I became a family man.

";
            var translations = new List<Translation>
            {
                new Translation()
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 2, EndPos = 9,
                        StartPos = 2
                    },
                    TranslatedText = "TLUMACZENIE_HAVENT"
                },
                new Translation()
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 2, EndPos = 14,
                        StartPos = 10
                    },
                    TranslatedText = "TLUMACZENIE_GONE"
                },
                new Translation()
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 2, EndPos = 18,
                        StartPos = 15
                    },
                    TranslatedText = "TLUMACZENIE_OUT"
                },
                new Translation()
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 3, EndPos = 14,
                        StartPos = 9
                    },
                    TranslatedText = "TLUMACZENIE_YEARS"
                },
            };

            var translatedContent = Translate(srtText, translations, false);
            var result = translatedContent.Compile();

            var expectedSrtText = @"1
00:00:21,400 --> 00:00:23,675
- What’s your name?
- Som.

2
00:00:24,720 --> 00:00:27,075
- You been working here for long?
- No. I've just started.

3
00:01:15,760 --> 00:01:25,760
<font color=""#F5DEB3"" face=""FONT"">haven’t - TLUMACZENIE_HAVENT</font>
<font color=""#F5DEB3"" face=""FONT"">gone - TLUMACZENIE_GONE</font>
<font color=""#F5DEB3"" face=""FONT"">out - TLUMACZENIE_OUT</font>

4
00:01:30,760 --> 00:01:34,799
I <font color=""#F5DEB3"" face=""FONT"">haven’t</font> <font color=""#F5DEB3"" face=""FONT"">gone</font> <font color=""#F5DEB3"" face=""FONT"">out</font> for years.

5
00:01:35,960 --> 00:01:39,191
About 10 years<font color=""#F5DEB3"" face=""FONT""> (TLUMACZENIE_YEARS)</font>.
I became a family man.

";
            result.ShouldEqual(expectedSrtText);

        }


        // this tests checks nothing since in srt we're removing translations
        // in same captions.
        [Fact]
        public void duplicated_translations_in_same_subtitles_are_removed()
        {
            var srtText = @"1
00:00:21,400 --> 00:00:23,675
- What’s your name?
- Som.

2
00:00:24,720 --> 00:00:27,075
- You been working here for long?
- No. I've just started.

3
00:01:30,760 --> 00:01:34,799
I haven't gone haven't for years.

4
00:01:35,960 --> 00:01:39,191
About 10 years.
I became a family man.

";
            var translations = new List<Translation>
            {
                new Translation()
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 2, EndPos = 9,
                        StartPos = 2
                    },
                    TranslatedText = "TLUMACZENIE_HAVENT"
                },
                new Translation()
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 2, EndPos = 14,
                        StartPos = 10
                    },
                    TranslatedText = "TLUMACZENIE_GONE"
                },
                new Translation()
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 2, EndPos = 22,
                        StartPos = 15
                    },
                    TranslatedText = "TLUMACZENIE_HAVENT"
                },
                new Translation()
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 3, EndPos = 14,
                        StartPos = 9
                    },
                    TranslatedText = "TLUMACZENIE_YEARS"
                },
            };

            var translatedContent = Translate(srtText, translations, false);
            var result = translatedContent.Compile();

            var expectedSrtText = @"1
00:00:21,400 --> 00:00:23,675
- What’s your name?
- Som.

2
00:00:24,720 --> 00:00:27,075
- You been working here for long?
- No. I've just started.

3
00:01:15,760 --> 00:01:25,760
<font color=""#F5DEB3"" face=""FONT"">haven't - TLUMACZENIE_HAVENT</font>
<font color=""#F5DEB3"" face=""FONT"">gone - TLUMACZENIE_GONE</font>
<font color=""#F5DEB3"" face=""FONT"">years - TLUMACZENIE_YEARS</font>

4
00:01:30,760 --> 00:01:34,799
I <font color=""#F5DEB3"" face=""FONT"">haven't</font> <font color=""#F5DEB3"" face=""FONT"">gone</font> haven't for years.

5
00:01:35,960 --> 00:01:39,191
About 10 <font color=""#F5DEB3"" face=""FONT"">years</font>.
I became a family man.

";
            result.ShouldEqual(expectedSrtText);

        }

        [Fact]
        public void basic_scenario4___no_break_four_translations_inline()
        {
            var srtText = @"1
00:00:00,400 --> 00:00:01,675
- What’s your name?
- Som.

2
00:00:02,720 --> 00:00:03,075
- You been working here for long?
- No. I've just started.

3
00:00:04,760 --> 00:00:05,799
I haven’t gone out for years.

4
00:00:06,960 --> 00:00:07,191
About 10 years.
I became a family man.

";
            var translations = new List<Translation>
            {
                new Translation()
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 2, EndPos = 9,
                        StartPos = 2
                    },
                    TranslatedText = "TLUMACZENIE_HAVENT"
                },
                new Translation()
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 2, EndPos = 14,
                        StartPos = 10
                    },
                    TranslatedText = "TLUMACZENIE_GONE"
                },
                new Translation()
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 2, EndPos = 18,
                        StartPos = 15
                    },
                    TranslatedText = "TLUMACZENIE_OUT"
                },
                new Translation()
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 3, EndPos = 14,
                        StartPos = 9
                    },
                    TranslatedText = "TLUMACZENIE_YEARS"
                },
            };

            var translatedContent = Translate(srtText, translations, false);
            var result = translatedContent.Compile();

            var expectedSrtText = @"1
00:00:00,400 --> 00:00:01,675
- What’s your name?
- Som.

2
00:00:02,720 --> 00:00:03,075
- You been working here for long?
- No. I've just started.

3
00:00:04,760 --> 00:00:05,799
I haven’t<font color=""#F5DEB3"" face=""FONT""> (TLUMACZENIE_HAVENT)</font> gone<font color=""#F5DEB3"" face=""FONT""> (TLUMACZENIE_GONE)</font> out<font color=""#F5DEB3"" face=""FONT""> (TLUMACZENIE_OUT)</font> for years.

4
00:00:06,960 --> 00:00:07,191
About 10 years<font color=""#F5DEB3"" face=""FONT""> (TLUMACZENIE_YEARS)</font>.
I became a family man.

";
            result.ShouldEqual(expectedSrtText);

        }


        [Fact]
        public void basic_scenario5___allow_for_three_duplicated_translations_for_exactly_same_words()
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
00:00:04,760 --> 00:00:05,799
I haven’t gone out for years.

4
00:00:06,960 --> 00:00:07,191
About 10 years.
I became a family man.

";
            var translations = new List<Translation>
            {
                new Translation()
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 0, EndPos = 19,
                        StartPos = 14
                    },
                    TranslatedText = "TLUMACZENIE_YEARS"
                },
                new Translation()
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 1, EndPos = 16,
                        StartPos = 11
                    },
                    TranslatedText = "TLUMACZENIE_YEARS"
                },
                new Translation()
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 2, EndPos = 28,
                        StartPos = 23
                    },
                    TranslatedText = "TLUMACZENIE_YEARS"
                },
                new Translation()
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 3, EndPos = 14,
                        StartPos = 9
                    },
                    TranslatedText = "TLUMACZENIE_YEARS"
                },
            };

            var translatedContent = Translate(srtText, translations, false);
            var result = translatedContent.Compile();

            var expectedSrtText = @"1
00:00:00,400 --> 00:00:01,675
- What’s your years<font color=""#F5DEB3"" face=""FONT""> (TLUMACZENIE_YEARS)</font>?
- Som.

2
00:00:02,720 --> 00:00:03,075
- You been years<font color=""#F5DEB3"" face=""FONT""> (TLUMACZENIE_YEARS)</font> here for long?
- No. I've just started.

3
00:00:04,760 --> 00:00:05,799
I haven’t gone out for years<font color=""#F5DEB3"" face=""FONT""> (TLUMACZENIE_YEARS)</font>.

4
00:00:06,960 --> 00:00:07,191
About 10 years.
I became a family man.

";

            result.ShouldEqual(expectedSrtText);
        }

        [Fact]
        public void remove_known_dialogs___is_set___known_dialogs_are_removed()
        {
            var srtText = @"1
00:00:19,000 --> 00:00:22,913
[Man] Back in the '70s,
every cop wanted out of the city.

2
00:00:23,040 --> 00:00:26,919
But the only cops allowed to live
outside New York were transit cops.

3
00:00:27,040 --> 00:00:31,556
Because the Transit Authority
was also run by Jersey and Connecticut.

4
00:00:31,680 --> 00:00:34,240
So these guys I knew
at the 37...

5
00:00:34,360 --> 00:00:36,476
they started pulling overtime
at subway stations,

6
00:00:36,600 --> 00:00:40,991
and got the city to declare them
""auxiliary transit cops.""

7
00:00:41,120 --> 00:00:45,238
They bought some land in Jersey. Got
some cheap loans from people they knew.

8
00:00:45,360 --> 00:00:48,716
They made themselves a place
where the shit couldn't touch 'em.

9
00:00:48,840 --> 00:00:51,308
That's what they thought anyway.

10
00:01:02,040 --> 00:01:04,270
Every precinct
has its ""cop bar,""

";

            var translations = new List<Translation>
            {
                new Translation()
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 1, EndPos = 12,
                        StartPos = 8
                    },
                    TranslatedText = "TLUMACZENIE_ONLY"
                },
                new Translation()
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 4, EndPos = 4,
                        StartPos = 0
                    },
                    TranslatedText = "TLUMACZENIE_THEY"
                },
                new Translation()
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 6, EndPos = 11,
                        StartPos = 5
                    },
                    TranslatedText = "TLUMACZENIE_BOUGHT"
                },
                new Translation()
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 7, EndPos = 9,
                        StartPos = 5
                    },
                    TranslatedText = "TLUMACZENIE_MADE"
                }
            };


            // translations are provided to captions 
            // nr: 2, 5, 7, 8 (starting from 1). Everything else
            // should be removed.


            var translatedContent = Translate(srtText, translations, true);
            var result = translatedContent.Compile();

            var expectedSrtText = @"1
00:00:08,040 --> 00:00:18,040
<font color=""#F5DEB3"" face=""FONT"">only - TLUMACZENIE_ONLY</font>
<font color=""#F5DEB3"" face=""FONT"">they - TLUMACZENIE_THEY</font>
<font color=""#F5DEB3"" face=""FONT"">bought - TLUMACZENIE_BOUGHT</font>

2
00:00:23,040 --> 00:00:26,919
But the <font color=""#F5DEB3"" face=""FONT"">only</font> cops allowed to live
outside New York were transit cops.

3
00:00:34,360 --> 00:00:36,476
<font color=""#F5DEB3"" face=""FONT"">they</font> started pulling overtime
at subway stations,

4
00:00:41,120 --> 00:00:45,238
They <font color=""#F5DEB3"" face=""FONT"">bought</font> some land in Jersey. Got
some cheap loans from people they knew.

5
00:00:45,360 --> 00:00:48,716
They made<font color=""#F5DEB3"" face=""FONT""> (TLUMACZENIE_MADE)</font> themselves a place
where the shit couldn't touch 'em.

";

            result.ShouldEqual(expectedSrtText);

        }


        [Fact]
        public void remove_known_dialogs___is_set___known_dialogs_are_removed2()
        {
            var srtText = @"1
00:03:36,049 --> 00:03:39,212
You've sold us a bill of goods,
Gordon.

2
00:03:39,385 --> 00:03:42,946
Promised new drugs to fight everything
from the common cold to cancer.

3
00:03:43,122 --> 00:03:47,991
-We'll deliver everything we promised.
-I'm sorry, but the adventure is over.

4
00:03:49,495 --> 00:03:51,622
We're exercising our option
to liquidate...

5
00:03:51,798 --> 00:03:54,028
...Byron & Mitchell Research Limited.

6
00:03:55,802 --> 00:03:59,329
Jack, show these people how filthy
rich we're gonna make them, please.

7
00:03:59,505 --> 00:04:01,905
I don't think you understand
what I just said.

";

            var translations = new List<Translation>
            {
                new Translation()
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 0, EndPos = 11,
                        StartPos = 7
                    },
                    TranslatedText = "TLUMACZENIE_SOLD"
                },
                new Translation()
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 2, EndPos = 14,
                        StartPos = 7
                    },
                    TranslatedText = "TLUMACZENIE_DELIVER"
                },
                new Translation()
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 3, EndPos = 16,
                        StartPos = 6
                    },
                    TranslatedText = "TLUMACZENIE_EXERCISING"
                },
                new Translation()
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 3, EndPos = 27,
                        StartPos = 21
                    },
                    TranslatedText = "TLUMACZENIE_OPTION"
                },
                new Translation()
                {
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 6, EndPos = 13,
                        StartPos = 8
                    },
                    TranslatedText = "TLUMACZENIE_THINK"
                }
            };


            // translations are provided to captions 
            // nr: 1, 3, 4 (two times), 7


            var translatedContent = Translate(srtText, translations, true);
            var result = translatedContent.Compile();

            var expectedSrtText = @"1
00:03:21,049 --> 00:03:31,049
<font color=""#F5DEB3"" face=""FONT"">sold - TLUMACZENIE_SOLD</font>
<font color=""#F5DEB3"" face=""FONT"">deliver - TLUMACZENIE_DELIVER</font>
<font color=""#F5DEB3"" face=""FONT"">exercising - TLUMACZENIE_EXERCISING</font>

2
00:03:36,049 --> 00:03:39,212
You've <font color=""#F5DEB3"" face=""FONT"">sold</font> us a bill of goods,
Gordon.

3
00:03:43,122 --> 00:03:47,991
-We'll <font color=""#F5DEB3"" face=""FONT"">deliver</font> everything we promised.
-I'm sorry, but the adventure is over.

4
00:03:49,495 --> 00:03:51,622
We're <font color=""#F5DEB3"" face=""FONT"">exercising</font> our option<font color=""#F5DEB3"" face=""FONT""> (TLUMACZENIE_OPTION)</font>
to liquidate...

5
00:03:51,798 --> 00:03:54,028
...Byron & Mitchell Research Limited.

6
00:03:59,505 --> 00:04:01,905
I don't think<font color=""#F5DEB3"" face=""FONT""> (TLUMACZENIE_THINK)</font> you understand
what I just said.

";

            result.ShouldEqual(expectedSrtText);

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

            var expectedSrtText = @"1
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

            result.ShouldEqual(expectedSrtText);
        }


        // it only is done for .srt. In .ass we've got only colored words (and translations
        // at top are merged properly), so it's fine to have them colored
        [Fact]
        public void if_unknown_occurs_multiple_times_in_single_dialog___duplications_are_removed()
        {
            translator.InsertBeforeAndInline = true;


            // word checking is case-insensitive, so there will be no two translations
            // for "Vibrate" and "vibrate"
            var srtText = @"1
00:01:04,643 --> 00:01:06,827
Excellent. Now hum. Mmm. Mmm.

2
00:01:06,847 --> 00:01:08,551
Vibrate, vibrate, vibrate, vibrate.

";
            var translations = new List<Translation>
            {
                new Translation
                {
                    // Vibrate
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 1,
                        EndPos = 7,
                        StartPos = 0
                    },
                    TranslatedText = "TLUMACZENIE_VIBRATE"
                },
                new Translation
                {
                    // Vibrate
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 1,
                        EndPos = 16,
                        StartPos = 9
                    },
                    TranslatedText = "TLUMACZENIE_VIBRATE"
                },
                new Translation
                {
                    // Vibrate
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 1,
                        EndPos = 25,
                        StartPos = 18
                    },
                    TranslatedText = "TLUMACZENIE_VIBRATE"
                },
                new Translation
                {
                    // Vibrate
                    Position = new WordPosInSRTFile
                    {
                        CaptionPos = 1,
                        EndPos = 34,
                        StartPos = 27
                    },
                    TranslatedText = "TLUMACZENIE_VIBRATE"
                }
            };

            var translatedContent = Translate(srtText, translations, false);
            var result = translatedContent.Compile();

            var expectedSrtText = @"1
00:00:49,643 --> 00:00:59,643
<font color=""#F5DEB3"" face=""FONT"">Vibrate - TLUMACZENIE_VIBRATE</font>

2
00:01:04,643 --> 00:01:06,827
Excellent. Now hum. Mmm. Mmm.

3
00:01:06,847 --> 00:01:08,551
Vibrate<font color=""#F5DEB3"" face=""FONT""> (TLUMACZENIE_VIBRATE)</font>, vibrate, vibrate, vibrate.

";
            result.ShouldEqual(expectedSrtText);
        }


        private SRTFile Translate(string srtText,
            List<Translation> translations, bool removeKnownDialogs)
        {
            var parser = new SubRipTextFileParser(new SRTParserVisitor());
            var content = parser.Parse(srtText, "fakeFile.srt");
            
            translator.Translate(content, translations, removeKnownDialogs);
            return content;
        }



    }
}
