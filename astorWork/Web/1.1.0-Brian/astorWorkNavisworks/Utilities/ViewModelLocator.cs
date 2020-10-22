using astorWork_Navisworks.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astorWork_Navisworks.Utilities
{
    public class ViewModelLocator
    {
        public static SyncPageVM syncPageVM = new SyncPageVM();
        public static SyncResultPageVM syncResultPageVM = new SyncResultPageVM();
    }
}
