using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NEF.Library.Business
{
    public class TextToTranslateMoney
    {
        private TextToTranslateMoney()
        {
        }

        public static string ParaYaziya(Decimal para)
        {
            return TextToTranslateMoney.ParaYaziya(para, TextToTranslateMoney.ParaYaziyaGosterimTipi.Varsayilan, "", "");
        }

        public static string ParaYaziya(Decimal para, TextToTranslateMoney.ParaYaziyaGosterimTipi gosterimTipi)
        {
            return TextToTranslateMoney.ParaYaziya(para, gosterimTipi, "", "");
        }

        public static string ParaYaziya(Decimal para, TextToTranslateMoney.ParaYaziyaGosterimTipi gosterimTipi, string paraBirimi, string kurusBirimi)
        {
            string[,] strArray1 = new string[4, 10]
            {
        {
          "Sıfır ",
          "Bir ",
          "İki ",
          "Üç ",
          "Dört ",
          "Beş ",
          "Altı ",
          "Yedi ",
          "Sekiz ",
          "Dokuz "
        },
        {
          "",
          "On ",
          "Yirmi ",
          "Otuz ",
          "Kırk ",
          "Elli ",
          "Altmış ",
          "Yetmiş ",
          "Seksen ",
          "Doksan "
        },
        {
          "",
          "Yüz ",
          "İkiyüz ",
          "Üçyüz ",
          "Dörtyüz ",
          "Beşyüz ",
          "Altıyüz ",
          "Yediyüz ",
          "Sekizyüz ",
          "Dokuzyüz "
        },
        {
          "",
          "_Bin_",
          "_Milyon_",
          "_Milyar_",
          "_Trilyon_",
          "_Katrilyon_",
          "_Kentrilyon_",
          "_Bin_Kentrilyon_",
          "_Milyon_Kentrilyon_",
          "_Milyar_Kentrilyon_"
        }
            };
            string[] strArray2 = new string[4]
            {
        "TL",
        "KURUŞ",
        "YTL",
        "YKR"
            };
            string str1 = strArray1[0, 0];
            if (paraBirimi == null)
                paraBirimi = "";
            if (kurusBirimi == null)
                kurusBirimi = "";
            if (paraBirimi != "" || kurusBirimi != "")
            {
                strArray2[2] = paraBirimi.Trim().ToUpper();
                strArray2[3] = kurusBirimi.Trim().ToUpper();
                gosterimTipi |= TextToTranslateMoney.ParaYaziyaGosterimTipi.BirimOlsun | TextToTranslateMoney.ParaYaziyaGosterimTipi.YTL;
            }
            if (para != new Decimal(0, 0, 0, false, (byte)1))
                para = Convert.ToDecimal(para.ToString(".00"));
            string str2 = Decimal.Subtract(para, Decimal.Truncate(para)).ToString();
            string str3 = str2.Length > 1 ? str2.Substring(2) : "";
            string[] strArray3 = new string[4]
            {
        "",
        "",
        Decimal.Truncate(para).ToString(),
        !(str3 == "") || (gosterimTipi & TextToTranslateMoney.ParaYaziyaGosterimTipi.KurusDaimaGozuksun) == (TextToTranslateMoney.ParaYaziyaGosterimTipi) 0 ? str3 : "0"
            };
            for (int index = 0; index < 2; ++index)
            {
                strArray1[0, 0] = strArray3[2 + index] == "0" ? str1 : "";
                string str4 = ("00" + strArray3[index + 2]).Substring((strArray3[index + 2].Length + 2) % 3);
                int startIndex = str4.Length - 3;
                while (startIndex >= 0)
                {
                    strArray3[index] = strArray1[2, Convert.ToInt32(str4.Substring(startIndex, 1))] + strArray1[1, Convert.ToInt32(str4.Substring(startIndex + 1, 1))] + (!(str4.Substring(startIndex, 3) == "001") || startIndex != 0 || str4.Length != 6 ? strArray1[0, Convert.ToInt32(str4.Substring(startIndex + 2, 1))] : "") + (str4.Substring(startIndex, 3) == "000" ? "" : strArray1[3, (str4.Length - startIndex) / 3 - 1]) + strArray3[index];
                    startIndex -= 3;
                }
                strArray3[index] = ((gosterimTipi & TextToTranslateMoney.ParaYaziyaGosterimTipi.BasHarflerBuyuk) != (TextToTranslateMoney.ParaYaziyaGosterimTipi)0 ? strArray3[index] : ((gosterimTipi & TextToTranslateMoney.ParaYaziyaGosterimTipi.BuyukHarf) != (TextToTranslateMoney.ParaYaziyaGosterimTipi)0 ? strArray3[index].ToUpper() : strArray3[index].ToLower())).Replace(" ", (gosterimTipi & TextToTranslateMoney.ParaYaziyaGosterimTipi.AralardaBoslukVar) != (TextToTranslateMoney.ParaYaziyaGosterimTipi)0 ? " " : "").Replace("_", (gosterimTipi & (TextToTranslateMoney.ParaYaziyaGosterimTipi.AralardaBoslukVar | TextToTranslateMoney.ParaYaziyaGosterimTipi.UcerliGrupBoslugu)) != (TextToTranslateMoney.ParaYaziyaGosterimTipi)0 ? " " : "").Trim().Replace("  ", " ");
            }
            return strArray3[0] + (strArray3[0].Length <= 0 || (gosterimTipi & TextToTranslateMoney.ParaYaziyaGosterimTipi.BirimOlsun) == (TextToTranslateMoney.ParaYaziyaGosterimTipi)0 && strArray3[1].Length <= 0 ? "" : " " + strArray2[(int)(gosterimTipi & TextToTranslateMoney.ParaYaziyaGosterimTipi.YTL) >> 4]) + (strArray3[1].Length > 0 ? (strArray3[0].Length > 0 ? " " : "") + strArray3[1] + " " + strArray2[((int)(gosterimTipi & TextToTranslateMoney.ParaYaziyaGosterimTipi.YTL) >> 4) + 1] : "");
        }

        public static string ParaYaziyaEng(Decimal para, TextToTranslateMoney.ParaYaziyaGosterimTipi gosterimTipi, string paraBirimi, string kurusBirimi)
        {
            string[,] strArray1 = new string[4, 10]
            {
        {
          "Zero ",
          "One ",
          "Two ",
          "Three ",
          "Four ",
          "Five ",
          "Six ",
          "Seven ",
          "Eight ",
          "Nine "
        },
        {
          "",
          "Ten ",
          "Twenty ",
          "Thirty ",
          "Fourty ",
          "Fifty ",
          "Sixty ",
          "Seventy ",
          "Eighty ",
          "Ninety "
        },
        {
          "",
          "Hundred ",
          "TwoHunderd ",
          "ThreeHundred ",
          "FourHundred ",
          "FiveHundred ",
          "SixHundred ",
          "SevenHundred ",
          "EightHundred ",
          "NineHundred "
        },
        {
          "",
          "_Thousand_",
          "_Million_",
          "_Billion_",
          "_Trillion_",
          "_Quadrillion_",
          "_Quintillion_",
          "_Thousand_Quintillion_",
          "_Million_Quintillion_",
          "_Billion_Quintillion_"
        }
            };
            string[] strArray2 = new string[4]
            {
        "TL",
        "KURUŞ",
        "YTL",
        "YKR"
            };
            string str1 = strArray1[0, 0];
            if (paraBirimi == null)
                paraBirimi = "";
            if (kurusBirimi == null)
                kurusBirimi = "";
            if (paraBirimi != "" || kurusBirimi != "")
            {
                strArray2[2] = paraBirimi.Trim().ToUpper();
                strArray2[3] = kurusBirimi.Trim().ToUpper();
                gosterimTipi |= TextToTranslateMoney.ParaYaziyaGosterimTipi.BirimOlsun | TextToTranslateMoney.ParaYaziyaGosterimTipi.YTL;
            }
            if (para != new Decimal(0, 0, 0, false, (byte)1))
                para = Convert.ToDecimal(para.ToString(".00"));
            string str2 = Decimal.Subtract(para, Decimal.Truncate(para)).ToString();
            string str3 = str2.Length > 1 ? str2.Substring(2) : "";
            string[] strArray3 = new string[4]
            {
        "",
        "",
        Decimal.Truncate(para).ToString(),
        !(str3 == "") || (gosterimTipi & TextToTranslateMoney.ParaYaziyaGosterimTipi.KurusDaimaGozuksun) == (TextToTranslateMoney.ParaYaziyaGosterimTipi) 0 ? str3 : "0"
            };
            for (int index = 0; index < 2; ++index)
            {
                strArray1[0, 0] = strArray3[2 + index] == "0" ? str1 : "";
                string str4 = ("00" + strArray3[index + 2]).Substring((strArray3[index + 2].Length + 2) % 3);
                int startIndex = str4.Length - 3;
                while (startIndex >= 0)
                {
                    strArray3[index] = strArray1[2, Convert.ToInt32(str4.Substring(startIndex, 1))] + strArray1[1, Convert.ToInt32(str4.Substring(startIndex + 1, 1))] + (!(str4.Substring(startIndex, 3) == "001") || startIndex != 0 || str4.Length != 6 ? strArray1[0, Convert.ToInt32(str4.Substring(startIndex + 2, 1))] : "") + (str4.Substring(startIndex, 3) == "000" ? "" : strArray1[3, (str4.Length - startIndex) / 3 - 1]) + strArray3[index];
                    startIndex -= 3;
                }
                strArray3[index] = ((gosterimTipi & TextToTranslateMoney.ParaYaziyaGosterimTipi.BasHarflerBuyuk) != (TextToTranslateMoney.ParaYaziyaGosterimTipi)0 ? strArray3[index] : ((gosterimTipi & TextToTranslateMoney.ParaYaziyaGosterimTipi.BuyukHarf) != (TextToTranslateMoney.ParaYaziyaGosterimTipi)0 ? strArray3[index].ToUpper() : strArray3[index].ToLower())).Replace(" ", (gosterimTipi & TextToTranslateMoney.ParaYaziyaGosterimTipi.AralardaBoslukVar) != (TextToTranslateMoney.ParaYaziyaGosterimTipi)0 ? " " : "").Replace("_", (gosterimTipi & (TextToTranslateMoney.ParaYaziyaGosterimTipi.AralardaBoslukVar | TextToTranslateMoney.ParaYaziyaGosterimTipi.UcerliGrupBoslugu)) != (TextToTranslateMoney.ParaYaziyaGosterimTipi)0 ? " " : "").Trim().Replace("  ", " ");
            }
            return strArray3[0] + (strArray3[0].Length <= 0 || (gosterimTipi & TextToTranslateMoney.ParaYaziyaGosterimTipi.BirimOlsun) == (TextToTranslateMoney.ParaYaziyaGosterimTipi)0 && strArray3[1].Length <= 0 ? "" : " " + strArray2[(int)(gosterimTipi & TextToTranslateMoney.ParaYaziyaGosterimTipi.YTL) >> 4]) + (strArray3[1].Length > 0 ? (strArray3[0].Length > 0 ? " " : "") + strArray3[1] + " " + strArray2[((int)(gosterimTipi & TextToTranslateMoney.ParaYaziyaGosterimTipi.YTL) >> 4) + 1] : "");
        }

        [Flags]
        public enum ParaYaziyaGosterimTipi : byte
        {
            BuyukHarf = (byte)1,
            BasHarflerBuyuk = (byte)2,
            AralardaBoslukVar = (byte)4,
            UcerliGrupBoslugu = (byte)8,
            BirimOlsun = (byte)16,
            YTL = (byte)32,
            KurusDaimaGozuksun = (byte)64,
            Varsayilan = KurusDaimaGozuksun | YTL | BirimOlsun | BuyukHarf,
        }
    }
}
