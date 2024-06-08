using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace mrrswpf.Commands
{
    internal class CmdOpenDialog : CommandBase
    {
        public event EventHandler WindowHidden;
        Window _window;
        public CmdOpenDialog(Window window)
        {
            _window = window;
        }

        public override void Execute(object parameter)
        {
            _window.Show();
        }
    }
}
