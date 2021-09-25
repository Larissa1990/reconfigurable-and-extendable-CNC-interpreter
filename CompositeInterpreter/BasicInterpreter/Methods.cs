using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using System.Text.RegularExpressions;
using System.IO;
using InterpreterInterface;

namespace BasicInterpreter
{
    public static class Methods
    {
        private static List<string> singleWords = new List<string>()
        {
            "N","T","X","Y","Z","I","J","K","S","D","F"
        };
        public static string ConvertProgram(List<string> programs)
        {

            string _program = null;
            string line = null;
            foreach (var item in programs)
            {
                line = ConvertEachBlock(item);
                if (line.Contains("Error"))
                    return line;
                else
                {
                    if (programs.IndexOf(item) != programs.Count - 1)
                        _program = _program + line + "\r\n";
                    else
                        _program = _program + line.Substring(0, line.Length - 1);
                }

            }

            return _program;
        }

        private static string ConvertEachBlock(string block)
        {
            string[] terms = block.Split(' ');
            List<string> _terms = new List<string>();

            foreach (var item in terms)
            {
                if (item.Length == 1)
                {
                    if (singleWords.Contains(item))
                        _terms.Add(item + "=");
                    else
                    {
                        Func<string, bool> isNumber = (word) =>
                        {
                            return Regex.Match(word, "^(-?\\d+)(\\.\\d+)?").Success;
                        };
                        if (isNumber(item))
                            _terms.Add(item);
                        else
                            return "Error Message";
                    }
                }
                else
                {
                    if (singleWords.Contains(item.Substring(0, 1)))
                    {
                        if (item.Substring(1, 1) == "=")
                            _terms.Add(item);
                        else
                        {
                            _terms.Add(item.Substring(0, 1) + "=" + item.Substring(1));
                        }
                    }
                    else
                        _terms.Add(item);
                }
            }

            string line = null;
            foreach (var item in _terms)
                line = line + item + " ";
            return line.TrimEnd() + ";";
        }

        public static List<TreeNode> GenerateTreeNode(string program)
        {
            List<string> programs = program.Split("\r\n").ToList();
            string _program = ConvertProgram(programs);
            Grammar _grammar = new LexicalAndSyntaxAnalysis();
            LanguageData _language = new LanguageData(_grammar);
            Parser _parser = new Parser(_language);
            List<TreeNode> nodes = new List<TreeNode>();
            int id = 1;
            if (_parser == null || !_parser.Language.CanParse())
            {
                Console.WriteLine("File Damaged");
            }
            else
            {
                ParseTree parseTree = _parser.Parse(_program);
                if (parseTree.ParserMessages.Count != 0)
                {
                    foreach (var item in parseTree.ParserMessages)
                        Console.WriteLine(item);
                }
                else
                {

                    foreach (var item in parseTree.Root.ChildNodes)
                        getNodes(0, item);
                    List<TreeNode> _nodes = getChildNodes(0, nodes);
                    return _nodes;
                }
            }
            return new List<TreeNode>();

            void getNodes(int pid, ParseTreeNode node)
            {
                string term = node.Term.ToString();
                if(term.ToLower()=="non motion commands")
                {
                    foreach (var item in node.ChildNodes)
                        getNodes(pid, item);
                }
                else
                {
                    TreeNode newNode = new TreeNode();
                    newNode.id = id;
                    id++;
                    newNode.pid = pid;
                    if (term.ToLower() == "expression")
                    {
                        newNode.name = ComputeExpression(node).ToString();
                        nodes.Add(newNode);
                    }
                    else
                    {
                        newNode.name = node.ToString().Contains("(") ? node.FindTokenAndGetText() : term;
                        nodes.Add(newNode);
                        foreach (var item in node.ChildNodes)
                            getNodes(newNode.id, item);
                    }
                }
            }

            List<TreeNode> getChildNodes(int pid, List<TreeNode> nodes)
            {
                List<TreeNode> mainNodes = nodes.Where(x => x.pid == pid).ToList();
                List<TreeNode> otherNodes = nodes.Where(x => x.pid != pid).ToList();
                foreach (var node in mainNodes)
                    node.childNodes = getChildNodes(node.id, otherNodes);
                return mainNodes;
            }
        }

        public static double ComputeExpressionTest(string expression)
        {
            Grammar _grammar = new LexicalAndSyntaxAnalysis();
            LanguageData _language = new LanguageData(_grammar);
            Parser _parser = new Parser(_language);

            if (_parser == null || !_parser.Language.CanParse())
            {
                Console.WriteLine("File Damaged");
            }
            else
            {
                ParseTree parseTree = _parser.Parse(expression);
                if (parseTree.ParserMessages.Count != 0)
                {
                    foreach (var item in parseTree.ParserMessages)
                        Console.WriteLine(item);
                }
                else
                {
                    return ComputeExpression(parseTree.Root);
                }
            }
            return 0;
        }

        public static double ComputeExpression(ParseTreeNode node)
        {
            Func<ParseTreeNode, double> UnaryExpression = (numNode) =>
            {
                string number = numNode.ChildNodes[1].FindTokenAndGetText();
                return Convert.ToDouble("-" + number);
            };

            string expType = node.Term.ToString().ToLower();

            if (expType == "binopexpression")
            {
                string sympol = node.ChildNodes[1].FindTokenAndGetText();
                switch (sympol)
                {
                    case "+":
                        return ComputeExpression(node.ChildNodes[0]) + ComputeExpression(node.ChildNodes[2]);
                    case "-":
                        return ComputeExpression(node.ChildNodes[0]) - ComputeExpression(node.ChildNodes[2]);
                    case "*":
                        return ComputeExpression(node.ChildNodes[0]) * ComputeExpression(node.ChildNodes[2]);
                    case "/":
                        return ComputeExpression(node.ChildNodes[0]) / ComputeExpression(node.ChildNodes[2]);
                    case "^":
                        return Math.Pow(ComputeExpression(node.ChildNodes[0]), ComputeExpression(node.ChildNodes[2]));
                }
            }
            else if (expType == "primaryexpression")
            {
                if (node.ChildNodes[0].Term.ToString() == "number")
                    return Convert.ToDouble(node.ChildNodes[0].FindTokenAndGetText());
                else
                    return ComputeExpression(node.ChildNodes[0]);
            }
            else if (expType == "unaryexpression")
            {
                return UnaryExpression(node);
            }
            else if (expType == "parenthesizedexpression")
            {
                return ComputeExpression(node.ChildNodes[0]);
            }
            else if (expType == "specialexpression")
            {
                string symbol = node.ChildNodes[0].FindTokenAndGetText();
                switch (symbol)
                {
                    case "cos":
                        return Math.Cos(ComputeExpression(node.ChildNodes[1]));
                    case "sin":
                        return Math.Sin(ComputeExpression(node.ChildNodes[1]));
                    case "tan":
                        return Math.Tan(ComputeExpression(node.ChildNodes[1]));
                }
            }
            else if (expType == "expression")
            {
                return ComputeExpression(node.ChildNodes[0]);
            }
            return 0;
        }

        public static double ComputeAngle(Position first, Position second, Position third, string compensationMode, string workPlane)
        {
            double angle1 = polarCoor(first, second, workPlane);
            double angle2 = polarCoor(second, third, workPlane);
            double det = angle2 - angle1;
            if (compensationMode == "G41")
                return Math.PI - det;
            else
                return Math.PI + det;
        }

        public static double polarCoor(Position start, Position end, string workPlane)
        {
            double a = 0, b = 0;
            switch (workPlane)
            {
                case "G17":
                    a = end.x - start.x;
                    b = end.y - start.y;
                    break;
                case "G18":
                    a = end.x - start.x;
                    b = end.z - start.z;
                    break;
                case "G19":
                    a = end.y - start.y;
                    b = end.z - start.z;
                    break;
            }
            double r = Math.Sqrt(a * a + b * b);
            double angle = Math.Asin(b / r);
            if (Math.Cos(angle) * (a / r) < 0)
            {
                angle = -(angle + Math.PI);
            }
            return angle;
        }



    }
}
