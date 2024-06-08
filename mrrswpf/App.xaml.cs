using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace mrrswpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            var conf = (Models.Configs)this.Resources["Zoe"];
            conf.DbPath = "I love Zoe Flood!";
        }
    }
}
