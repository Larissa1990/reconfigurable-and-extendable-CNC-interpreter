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
        private NonTerminal binOpExpression;
        private NonTerminal binOp;
        private NonTerminal specialExpression;

        private NonTerminal posX;
        private NonTerminal posY;
        private NonTerminal posZ;
        private NonTerminal posI;
        private NonTerminal posJ;
        private NonTerminal posK;
        private NonTerminal posR;
        private NonTerminal strpos;
        private NonTerminal cirpos;
        private NonTerminal strPos;
        private NonTerminal cirPos;
        private NonTerminal position;
        private NonTerminal linearInterpo;
        private NonTerminal circularInterpo;
        private NonTerminal motion;
        private NonTerminal toolChange;
        private NonTerminal toolSelection;
        private NonTerminal changeTool;
        private NonTerminal spindleMotion;
        private NonTerminal spindleRotation;
        private NonTerminal spindleSpeed;
        private NonTerminal spindleStop;
        private NonTerminal feedControl;
        private NonTerminal feedUnits;
        private NonTerminal feedValue;
        private NonTerminal geometry;
        private NonTerminal zeroOffset;
        private NonTerminal workPlane;
        private NonTerminal dimensions;
        private NonTerminal units;
        private NonTerminal compensation;
        private NonTerminal compensationMode;
        private NonTerminal compensationSelection;
        private NonTerminal coolant;
        private NonTerminal suppression;
        private NonTerminal nonMotion;
        private NonTerminal nonMotions;
        private NonTerminal order;
        private NonTerminal basicBlock;
        private NonTerminal ncBlock;
        private NonTerminal ncBlocks;

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

            #region Expression
            expression = new NonTerminal("Expression");
            unaryExpression = new NonTerminal("Unary Expression");
            primaryExpression = new NonTerminal("Primary Expression");
            parenthesizedExpression = new NonTerminal("Parenthesized Expression");
            binOpExpression = new NonTerminal("BinOpExpression");
            binOp = new NonTerminal("BinOp");
            specialExpression = new NonTerminal("Special Expression");

            expression.Rule = binOpExpression | primaryExpression;
            unaryExpression.Rule = ToTerm("-") + number;
            binOpExpression.Rule = expression + binOp + expression;
            parenthesizedExpression.Rule = lp + expression + rp;
            binOp.Rule = ToTerm("^") | "+" | "-" | "*" | "/";
            specialExpression.Rule = identifier + parenthesizedExpression;
            primaryExpression.Rule = number | unaryExpression | parenthesizedExpression | specialExpression;

            RegisterOperators(3, "^");
            RegisterOperators(1, "+", "-");
            RegisterOperators(2, "*", "/");

            #endregion
            #region Position
            posX = new NonTerminal("X");
            posY = new NonTerminal("Y");
            posZ = new NonTerminal("Z");
            posI = new NonTerminal("I");
            posJ = new NonTerminal("J");
            posK = new NonTerminal("K");
            posR = new NonTerminal("R");
            strpos = new NonTerminal("Target Position");
            cirpos = new NonTerminal("Target Position");
            strPos = new NonTerminal("Target Position");
            cirPos = new NonTerminal("Target Position");
            position = new NonTerminal("Target Position");

            posX.Rule = ToTerm("X") + expression | ToTerm("X") +"="+ expression;
            posY.Rule = ToTerm("Y") + expression | ToTerm("Y=") + "="+expression;
            posZ.Rule = ToTerm("Z") + expression | ToTerm("Z=") + "="+expression;
            posI.Rule = ToTerm("I") + expression | ToTerm("I=") + "="+expression;
            posJ.Rule = ToTerm("J") + expression | ToTerm("J=") + "="+expression;
            posK.Rule = ToTerm("K") + expression | ToTerm("K=") + "="+expression;
            posR.Rule = ToTerm("R") + expression | ToTerm("R=") + "="+expression;
            strpos.Rule = posX | posY | posZ;
            strPos.Rule = MakePlusRule(strPos, strpos);
            cirpos.Rule = posI | posJ | posK;
            cirPos.Rule = MakePlusRule(cirPos, cirpos) | posR;
            position.Rule = strPos | strPos + cirPos;
            #endregion
            #region Motion Command
            linearInterpo = new NonTerminal("Linear Interpolation");
            circularInterpo = new NonTerminal("Circular Interpolation");
            motion = new NonTerminal("Motion Command");

            linearInterpo.Rule = ToTerm("G01") | "G1" | "G00" | "G0";
            circularInterpo.Rule = ToTerm("G02") | "G2" | "G03" | "G3";
            motion.Rule = linearInterpo | circularInterpo;
            #endregion
            #region Non Motion Commands
            toolChange = new NonTerminal("Tool Change Command");
            toolSelection = new NonTerminal("Tool Selection");
            changeTool = new NonTerminal("Change Tool");
            spindleMotion = new NonTerminal("Spindle Motion Command");
            spindleRotation = new NonTerminal("Spindle Rotation");
            spindleSpeed = new NonTerminal("Spindle Speed");
            spindleStop = new NonTerminal("Spindle Stop");
            feedControl = new NonTerminal("Feed Control Command");
            feedUnits = new NonTerminal("Feed Units");
            feedValue = new NonTerminal("Feed Value");
            geometry = new NonTerminal("Geometry Setting Command");
            zeroOffset = new NonTerminal("Settable Zero Offset");
            workPlane = new NonTerminal("Woking Plane");
            dimensions = new NonTerminal("Diemensions");
            units = new NonTerminal("Units");
            compensation = new NonTerminal("Tool Radius Compensation Command");
            compensationMode = new NonTerminal("Compensation Mode");
            compensationSelection = new NonTerminal("Compensation Selection");
            coolant = new NonTerminal("Coolant Control");
            suppression = new NonTerminal("Suppression Zero Offset");
            nonMotion = new NonTerminal("Non Motion Command");
            nonMotions = new NonTerminal("Non Motion Commands");
            order = new NonTerminal("Order");
            basicBlock = new NonTerminal("Basic Block");
            ncBlock = new NonTerminal("NC Block");
            ncBlocks = new NonTerminal("NC Blocks");


            toolChange.Rule = toolSelection | changeTool;
            spindleMotion.Rule = spindleRotation | spindleSpeed | spindleStop;
            feedControl.Rule = feedUnits | feedValue;
            geometry.Rule = zeroOffset | workPlane | dimensions | units;
            compensation.Rule = compensationMode | compensationSelection;
            coolant.Rule = ToTerm("M7") | "M8" | "M9";
            suppression.Rule = ToTerm("G53");
            toolSelection.Rule = ToTerm("T") + number | ToTerm("T") + "=" + number;
            changeTool.Rule = ToTerm("M06") | "M6";
            spindleRotation.Rule = ToTerm("M3") | "M4" | "M5" | "M03" | "M04" | "M05";
            spindleSpeed.Rule = ToTerm("S") + number | ToTerm("S") +"="+ number;
            spindleStop.Rule = ToTerm("M2") | "M30";
            feedUnits.Rule = ToTerm("G94") | "G95";
            feedValue.Rule = ToTerm("F") + number | ToTerm("F") + "=" + number;
            zeroOffset.Rule = ToTerm("G54") | "G55" | "G56" | "G57";
            workPlane.Rule = ToTerm("G17") | "G18" | "G19";
            dimensions.Rule = ToTerm("G90") | "G91";
            units.Rule = ToTerm("G70") | "G71";
            compensationMode.Rule = ToTerm("G40") | "G41" | "G42";
            compensationSelection.Rule = ToTerm("D") + number | ToTerm("D") + "=" + number;

            nonMotion.Rule = toolChange | spindleMotion | feedControl | geometry | compensation | coolant | suppression;
            nonMotions.Rule = MakePlusRule(nonMotions, nonMotion) | Empty;

            order.Rule = ToTerm("N") + number | ToTerm("N") + "=" + number;
            basicBlock.Rule = nonMotions + linearInterpo + nonMotions + strPos + nonMotions
                | nonMotions + circularInterpo + nonMotions + cirPos + nonMotions
                | nonMotions + linearInterpo + nonMotions
                | nonMotions + strPos + nonMotions
                | nonMotions + circularInterpo + nonMotions
                | nonMotions + cirPos + nonMotions
                | nonMotions;

            ncBlock.Rule = order + basicBlock;
            ncBlocks.Rule = MakePlusRule(ncBlocks, semi, ncBlock);



            #endregion
            //this.Root = ncBlocks;
            this.Root = expression;
        }


    }
}
