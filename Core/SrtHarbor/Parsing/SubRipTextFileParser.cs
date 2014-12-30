using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Antlr4.Runtime;
using SrtHarbor.Model;
using System;
using NLog;
using System.Collections.Generic;

namespace SrtHarbor.Parsing
{
    public class SubRipTextFileParser 
    {
        private readonly SRTParserBaseVisitor<SRTFile> visitor;
        private string lineEndings = LineEndings.Windows;
        private readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public SubRipTextFileParser(SRTParserBaseVisitor<SRTFile> visitor)
        {
            this.visitor = visitor;
        }

        private string FixLineEndingsFor(string input)
        {
            // Fix for:
            //
            // Situation when last subtitle isn't ended correctly
            // It happens when there is no endline at the end of file and
            // no endline at the end of last subtitle.
            if (input.Contains(LineEndings.Windows))
            {
                lineEndings = LineEndings.Windows;
                if (input.EndsWith(LineEndings.Windows))
                    return input;
                return input + LineEndings.Windows;
            }

            if (input.Contains(LineEndings.Mac))
            {
                lineEndings = LineEndings.Mac;
                if (input.EndsWith(LineEndings.Mac))
                    return input;
                return input + LineEndings.Mac;
            }

            if (input.Contains(LineEndings.Unix))
            {
                lineEndings = LineEndings.Unix;
                if (input.EndsWith(LineEndings.Unix))
                    return input;
                return input + LineEndings.Unix;
            }
            return input;
        }

        public SRTFile Parse(string input, string fileName)
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            var task = Task<SRTFile>.Run(() =>
            {
                input = FixLineEndingsFor(input);

                var stream = new AntlrInputStream(input);
                var lexer = new SRTLexer(stream);
                var tokens = new CommonTokenStream(lexer);
                var parser = new SRTParser(tokens);
                var listener = new ErrorListener();

                parser.RemoveErrorListeners();
                parser.AddErrorListener(listener);

                var subtitles = visitor.VisitFile(parser.file());
                Logger.Debug("ParseErrors in file: " + fileName + ": " +
                    String.Join("\n", listener.ParseErrors.ToList()));

                return subtitles;
            }, token);

            if (!task.Wait(30000, token))
            {
                tokenSource.Cancel();
                throw new InvalidOperationException("Parsing took too much time!");
            }
            
            return task.Result;
        }
    }
}
