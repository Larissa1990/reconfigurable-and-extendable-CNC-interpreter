using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterpreterInterface
{
    public class TreeNode
    {
        public int id { get; set; }
        public int pid { get; set; }
        public string name { get; set; }
        public List<TreeNode> childNodes { get; set; }

        public TreeNode()
        {
            childNodes = new List<TreeNode>();
        }
    }
}
