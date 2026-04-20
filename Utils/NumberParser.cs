using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Text;

namespace water3.Utils
{
    
  
    
         public static class NumberParser
         {
             public static char NormalizeArabicDigit(char c)
             {
                 if (c >= '٠' && c <= '٩') return (char)('0' + (c - '٠'));
                 if (c >= '۰' && c <= '۹') return (char)('0' + (c - '۰'));
                 return c;
             }

             public static string NormalizeNumberString(string input)
             {
                 if (string.IsNullOrWhiteSpace(input)) return "";

                 var sb = new StringBuilder();
                 bool dotAdded = false;

                 foreach (char c0 in input.Trim())
                 {
                     char c = NormalizeArabicDigit(c0);

                     if (char.IsDigit(c))
                         sb.Append(c);
                     else if ((c == '.' || c == ',') && !dotAdded)
                     {
                         sb.Append('.');
                         dotAdded = true;
                     }
                 }
                 return sb.ToString();
             }

             public static bool TryParseDecimal(string s, out decimal value)
             {
                 return decimal.TryParse(
                     s,
                     NumberStyles.Number | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                     CultureInfo.InvariantCulture,
                     out value
                 );
             }
         }
     }
 
