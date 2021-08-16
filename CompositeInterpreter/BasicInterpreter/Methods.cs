using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;

namespace BasicInterpreter
{
    public static class Methods
    {
        private static List<string> singleWords = new List<string>()
        {
            "N","T","X","Y","Z","I","J","K","S","D","F"
        };
        public static string ConvertProgram(List<string>program)
        {
            string _program = null;
            foreach (var item in program)
                _program = _program + ConvertEachBlock(item) + "\r\n";
            return _program;
        }

        private static string ConvertEachBlock(string block)
        {
            string[] terms = block.Split(' ');
            List<string> _terms = new List<string>();

            foreach(var item in terms)
            {
                if (item.Length == 1)
                {
                    if(singleWords.Contains(item))
                        _terms.Add(item + "=");
                    else
                    {
                        // 这里需要百度一下，判断字符串是否为数字的方法，使用正则表达式
                        Func<string, bool> isNumber = (word) =>
                        {
                            return true;
                        };
                        if (isNumber(item))
                            _terms.Add(item);
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

        public static List<TreeNode>GenerateTreeNode(string expression)
        {
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
                ParseTree parseTree = _parser.Parse(expression);
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

            // 这里我感觉可以用类似于node.js那种匿名方法的写法
            void getNodes(int pid, ParseTreeNode node)
            {
                TreeNode newNode = new TreeNode();
                newNode.id = id;
                id++;
                newNode.pid = pid;
                newNode.name = node.ToString();
                nodes.Add(newNode);
                foreach (var item in node.ChildNodes)
                    getNodes(newNode.id, item);
            }

            List<TreeNode>getChildNodes(int pid, List<TreeNode> nodes)
            {
                List<TreeNode> mainNodes = nodes.Where(x => x.pid == pid).ToList();
                List<TreeNode> otherNodes = nodes.Where(x => x.pid != pid).ToList();
                foreach (var node in mainNodes)
                    node.childNodes = getChildNodes(node.id, otherNodes);
                return mainNodes;
            }
        }



    }
}
