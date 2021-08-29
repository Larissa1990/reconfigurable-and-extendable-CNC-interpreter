using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterpreterInterface;
using Irony.Parsing;

namespace BasicInterpreter
{
    public class BasicInterpreter: IInterpreter
    {
        public Dictionary<string, Command> modalTable { get; set; }
        public Dictionary<string, Command> nonModalTable { get; set; }

        public Position LastPosition { get; set; }

        public List<string> Errors
        {
            get { return errors; }
        }
        public List<string> Warns
        {
            get { return warns; }
        }

        private Grammar _grammer;
        private LanguageData _language;
        private Parser _parser;
        private List<string> errors;
        private List<string> warns;
        private int id;

        public BasicInterpreter()
        {
            _grammer = new LexicalAndSyntaxAnalysis();
            _language = new LanguageData(_grammer);
            _parser = new Parser(_language);
            errors = new List<string>();
            warns = new List<string>();
            
        }
       
        public void Interpret(string program, ref int nodeId)
        {
            id = nodeId;
            List<TreeNode> syntaxTree = LexicalSyntaxAnalysis(program);
            if(errors.Count == 0)
            {
                nodeId = id;
            }

        }

        private List<TreeNode>LexicalSyntaxAnalysis(string program)
        {
            List<string> programs = program.Split("\r\n").ToList();
            string _program = Methods.ConvertProgram(programs);
            List<TreeNode> nodes = new List<TreeNode>();

            if (_parser == null || !_parser.Language.CanParse())
            {
                errors.Add("File Damaged");
            }
            else
            {
                ParseTree parseTree = _parser.Parse(_program);
                if (parseTree.ParserMessages.Count != 0)
                {
                    foreach (var item in parseTree.ParserMessages)
                        errors.Add(item.Message);
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
                if (term.ToLower() == "non motion commands")
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
                        newNode.name = Methods.ComputeExpression(node).ToString();
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
    }
}
