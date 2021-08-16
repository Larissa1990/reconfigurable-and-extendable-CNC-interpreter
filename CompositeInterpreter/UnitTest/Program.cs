using System;
using Irony.Parsing;
using BasicInterpreter;
using System.Collections.Generic;
using System.IO;


namespace UnitTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"C:\Users\Larissa\Desktop\Components1119\programs\milling#1.txt";
            using(StreamReader sr = new StreamReader(path))
            {
                List<string> program = new List<string>();
                string line = sr.ReadLine();
                while (line != null)
                {
                    program.Add(line);
                    line = sr.ReadLine();
                }

                string _program = BasicInterpreter.Methods.ConvertProgram(program);
                Console.WriteLine(_program);

            }
            /*
            Grammar _grammar = new LexicalAndSyntaxAnalysis();
            LanguageData _language = new LanguageData(_grammar);
            Parser _parser = new Parser(_language);
            if (_parser == null || !_parser.Language.CanParse())
            {
                Console.WriteLine("File Damaged");
            }
            else
            {
                Console.WriteLine("Please inut an expression:");
                string inputExpression = Console.ReadLine().ToString();
                ParseTree parseTree = _parser.Parse(inputExpression);
                if(parseTree.ParserMessages.Count !=0)
                {
                    foreach (var item in parseTree.ParserMessages)
                        Console.WriteLine(item);
                }
                else
                {
                    WriteNode(parseTree.Root,"");
                }
            }*/
            Console.Read();
        }

        static void WriteNode(ParseTreeNode node, string space)
        {
            Console.WriteLine(space+node.ToString());
            if(node.ChildNodes.Count != 0)
            {
                space = space + "  ";
                foreach (var item in node.ChildNodes)
                    WriteNode(item,space);
            }
            else
            {
                space = space.Substring(0, space.Length - 2);
            }

        }

    }
}
