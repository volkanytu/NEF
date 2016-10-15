using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NEF.Library.Utility;
using NEF.Library.Business;

namespace NEF.ConsoleApp.LogoIntegration
{
    class Program
    {
        static void Main(string[] args)
        {
            MsCrmResult resultExpenseCenter = ExpenseCenterProcess.Process();

            if (resultExpenseCenter.Success)
            {
                Console.SetCursorPosition(0, 5);
                Console.WriteLine(resultExpenseCenter.Result);
            }

            MsCrmResult resultSales = SalesProcess.Process();

            if (resultSales.Success)
            {
                Console.SetCursorPosition(0, 6);
                Console.WriteLine(resultSales.Result);
            }

        }
    }
}
