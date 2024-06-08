using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace mrrswpf.ViewModels
{
    /// <summary>
    /// Interaction logic for AddInspectorActivity.xaml
    /// </summary>
    public partial class AddInspectorActivity : Window
    {
        public bool CanClose { get; set; } = false;
        public AddInspectorActivity()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (!CanClose)
            {
                e.Cancel = true;
                this.Hide();
            }
        }
    }
}
