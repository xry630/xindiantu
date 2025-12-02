using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    public class FastCmdsCfg
    {
        public ObservableCollection<FastCmdModel> FastCmds { get; set; } = new ObservableCollection<FastCmdModel>();
    }

}
