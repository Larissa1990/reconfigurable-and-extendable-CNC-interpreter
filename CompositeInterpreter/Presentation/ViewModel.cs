using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using BasicInterpreter;

namespace Presentation
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<TreeNode> nodes = new ObservableCollection<TreeNode>();

        public ObservableCollection<TreeNode> Nodes
        {
            get
            {
                return nodes;
            }
            set
            {
                nodes = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Nodes"));
            }
        }

        public DelegateCommand GenerateCommand { get; }

        public ViewModel()
        {
            GenerateCommand = new DelegateCommand(GenerateCommandHandler);
        }

        private void GenerateCommandHandler(object sender, DelegateCommandEventArgs e)
        {
            string expression = e.Parameter.ToString();
            expression.Trim();
            nodes.Clear();
            List<TreeNode> _nodes = BasicInterpreter.Methods.GenerateTreeNode(expression);
            foreach (var item in _nodes)
                nodes.Add(item);
        }


    }
}
