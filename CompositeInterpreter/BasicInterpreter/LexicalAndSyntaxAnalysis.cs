using System;
using Irony.Parsing;

namespace BasicInterpreter
{
    [Language("G code basic commands","1.0","Lexical and syntactic analysis")]
    public class LexicalAndSyntaxAnalysis: Grammar
    {
        #region Private fields
        private NumberLiteral number;
        private IdentifierTerminal identifier;
        private KeyTerm colon;
        private KeyTerm semi;
        private KeyTerm comma;
        private KeyTerm lp;
        private KeyTerm rp;
        private NonTerminal expression;
        private NonTerminal unaryExpression;
        private NonTerminal primaryExpression;
        private NonTerminal parenthesizedExpression;
        private NonTerminal literal;
        private NonTerminal binOpExpression;
        private NonTerminal binOp;
        private NonTerminal memberAccess;
        private NonTerminal memberAccessSegment;


        #endregion

        public LexicalAndSyntaxAnalysis()
        {
            number = TerminalFactory.CreateCSharpNumber("number");
            identifier = TerminalFactory.CreateCSharpIdentifier("identifier");
            colon = ToTerm(":", "colon");
            semi = ToTerm(";", "semi");
            comma = ToTerm(",", "comma");
            lp = ToTerm("(");
            rp = ToTerm(")");

            CommentTerminal SingleLineComment = new CommentTerminal("SingleLineComment", "//");
            CommentTerminal DelimitedCommand = new CommentTerminal("DelimitedComment", "/*", "*/");
            NonGrammarTerminals.Add(SingleLineComment);
            NonGrammarTerminals.Add(DelimitedCommand);

            expression = new NonTerminal("expression");
            unaryExpression = new NonTerminal("unaryExpression");
            primaryExpression = new NonTerminal("primaryExpression");
            parenthesizedExpression = new NonTerminal("parenthesizedExpression");
            literal = new NonTerminal("literal");
            binOpExpression = new NonTerminal("binOpExpression");
            binOp = new NonTerminal("binOp");
            memberAccess = new NonTerminal("memberAccess");
            memberAccessSegment = new NonTerminal("memberAccessSegment");

            expression.Rule = binOpExpression | primaryExpression;
            unaryExpression.Rule = ToTerm("-") + number;
            binOpExpression.Rule = expression + binOp + expression;
            parenthesizedExpression.Rule = lp + expression + rp;
            binOp.Rule = ToTerm("^") | "+" | "-" | "*" | "/";
            primaryExpression.Rule = number | unaryExpression | parenthesizedExpression;
            //primaryExpression.Rule = number|unaryExpression|parenthesizedExpression

            RegisterOperators(3, "^");
            RegisterOperators(1, "+", "-");
            RegisterOperators(2, "*", "/");

            this.Root = expression;

        }
    }
}
