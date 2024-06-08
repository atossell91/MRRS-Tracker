using mrrswpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mrrswpf.Commands
{
    internal class OpenAddInspectorActivityWindow : CommandBase
    {
        AddInspectorActivity _dialog;
        public OpenAddInspectorActivityWindow(AddInspectorActivity dialog)
        {
            _dialog = dialog;
        }
        public override void Execute(object parameter)
        {
            _dialog.ShowDialog();
        }
    }
}
