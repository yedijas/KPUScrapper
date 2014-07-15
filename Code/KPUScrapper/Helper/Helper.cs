using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPUScrapper.Helper
{
    class Helper
    {
        public static string ValidateDir(string dir)
        {
            if (!dir.Last().Equals('\\'))
                return dir + '\\';
            else
                return dir;
        }
    }
}
