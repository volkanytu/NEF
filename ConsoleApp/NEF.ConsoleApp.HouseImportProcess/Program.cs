using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NEF.Library.Utility;
using NEF.Library.Business;

namespace NEF.ConsoleApp.HouseImportProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Ürün Import uygulaması çalışıyor...");

            MsCrmResult result = ImportProduct.Process();
           

            Console.SetCursorPosition(0, 8);
            Console.WriteLine(result.Result);

            Console.SetCursorPosition(0, 9);
            Console.WriteLine("Çıkış için bir tuşa basınız...");

            //Console.ReadKey();
        }
    }
}
