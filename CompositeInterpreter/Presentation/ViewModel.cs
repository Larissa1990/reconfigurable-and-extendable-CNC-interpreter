using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using BasicInterpreter;
using Microsoft.Win32;
using System.IO;
using InterpreterInterface;

namespace Presentation
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<TreeNode> nodes = new ObservableCollection<TreeNode>();
        private string programText;

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

        public string ProgramText
        {
            get { return programText; }
            set
            {
                programText = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ProgramText"));
            }
        }

        public DelegateCommand GenerateCommand { get; }

        public DelegateCommand OpenFileCommand { get; }

        public ViewModel()
        {
            GenerateCommand = new DelegateCommand(GenerateCommandHandler);
            OpenFileCommand = new DelegateCommand(OpenFileCommandHandler);
        }

        private void GenerateCommandHandler(object sender, EventArgs e)
        {
            nodes.Clear();
            List<TreeNode> _nodes = BasicInterpreter.Methods.GenerateTreeNode(ProgramText);
            foreach (var item in _nodes)
                nodes.Add(item);
        }
        private void OpenFileCommandHandler(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "Load an NC program";
            if (fileDialog.ShowDialog() == true)
            {
                string name = fileDialog.FileName;
                using (StreamReader sr = new StreamReader(name))
                {
                    ProgramText = sr.ReadToEnd();
                }
            }
        }

    }
}
