using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime;

namespace SrtHarbor.Parsing
{
    internal class ErrorListener : BaseErrorListener
    {
        private List<string> errors = new List<string>(); 

        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            var sb = new StringBuilder();
            var invStack = String.Join("<-", ((Parser)(recognizer)).GetRuleInvocationStack());
            sb.AppendFormat("Syntax error at rule: '{0}'. Offending symbol: {1}, Exception: {2}, Message: {3}", invStack, offendingSymbol.ToString(), e, msg);
            errors.Add(sb.ToString());
        }

        public override void ReportAmbiguity(Parser recognizer, Antlr4.Runtime.Dfa.DFA dfa, int startIndex, int stopIndex, bool exact, Antlr4.Runtime.Sharpen.BitSet ambigAlts, Antlr4.Runtime.Atn.ATNConfigSet configs)
        {
            var sb = new StringBuilder();
            var invStack = String.Join("<-", ((Parser)(recognizer)).GetRuleInvocationStack());
            sb.AppendFormat("Ambiguity at rule: '{0}'. Start index: {1}, Stop index: {2}", invStack, startIndex, stopIndex);
        }

        public override void ReportAttemptingFullContext(Parser recognizer, Antlr4.Runtime.Dfa.DFA dfa, int startIndex, int stopIndex, Antlr4.Runtime.Sharpen.BitSet conflictingAlts, Antlr4.Runtime.Atn.SimulatorState conflictState)
        {
            var sb = new StringBuilder();
            var invStack = String.Join("<-", ((Parser)(recognizer)).GetRuleInvocationStack());
            sb.AppendFormat("AttemptingFullContext at rule: '{0}'. Start index: {1}, Stop index: {2}", invStack, startIndex, stopIndex);
        }

        public override void ReportContextSensitivity(Parser recognizer, Antlr4.Runtime.Dfa.DFA dfa, int startIndex, int stopIndex, int prediction, Antlr4.Runtime.Atn.SimulatorState acceptState)
        {
            var sb = new StringBuilder();
            var invStack = String.Join("<-", ((Parser)(recognizer)).GetRuleInvocationStack());
            sb.AppendFormat("ContextSensitivity at rule: '{0}'. Start index: {1}, Stop index: {2}", invStack, startIndex, stopIndex);
        }

        public IReadOnlyCollection<string> ParseErrors 
        {
            get
            {
                return errors.AsReadOnly();
            }
        }
    }
}
