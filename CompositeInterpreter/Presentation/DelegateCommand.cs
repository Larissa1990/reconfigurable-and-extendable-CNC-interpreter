using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Presentation
{
    public class DelegateCommand : ICommand
    {
        // 定义一个名为SimpleEventHandler的委托，两个参数，一个object类，一个是自定义的DelegateCommandEventArgs类
        public delegate void SimpleEventHandler(object sender, DelegateCommandEventArgs e);
        // handler是方法，别忘了，委托是用于定义方法的类
        private SimpleEventHandler handler;
        private bool isEnabled = true;

        public DelegateCommand(SimpleEventHandler handler)
        {
            this.handler = handler;
        }

        public void Execute(object parameter)
        {
            this.handler(this, new DelegateCommandEventArgs(parameter));
        }
        public bool CanExecute(object parameter)
        {
            return this.isEnabled;
        }
        public event EventHandler CanExecuteChanged;
        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set
            {
                this.isEnabled = value;
                this.OnCanExecuteChanged();
            }
        }
        private void OnCanExecuteChanged()
        {
            if (this.CanExecuteChanged != null)
                this.CanExecuteChanged(this, EventArgs.Empty);
        }
    }
}
