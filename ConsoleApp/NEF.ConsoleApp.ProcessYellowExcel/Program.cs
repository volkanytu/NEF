using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NEF.Library.Business;
using NEF.Library.Utility;

namespace NEF.ConsoleApp.ProcessYellowExcel
{
    class Program
    {
        static void Main(string[] args)
        {
            MsCrmResult result = ProcessRecords.Process();
        }
    }
}
