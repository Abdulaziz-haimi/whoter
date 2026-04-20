using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using water3.Models;

namespace water3.Services
{
    public class ReadingImportService
    {
        private static readonly Dictionary<string, string[]> HeaderAliases =
            new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
            {
                { "SubscriberID",  new[] { "SubscriberID", "SubscriberId", "رقم المشترك", "المشترك", "Subscriber" } },
                { "MeterID",       new[] { "MeterID", "MeterId", "رقم العداد", "العداد", "Meter" } },
                { "ReadingDate",   new[] { "ReadingDate", "Reading Date", "تاريخ القراءة", "التاريخ", "Date" } },
                { "CurrentReading",new[] { "CurrentReading", "Current Reading", "القراءة الحالية", "القراءة", "Reading" } },
                { "Notes",         new[] { "Notes", "ملاحظات", "ملاحظة", "Note" } }
            };

        public List<ReadingImportRow> ReadFromExcel(string filePath)
        {
            var result = new List<ReadingImportRow>();

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("مسار الملف غير صالح");

            if (!File.Exists(filePath))
                throw new FileNotFoundException("الملف غير موجود", filePath);

            using (var wb = new XLWorkbook(filePath))
            {
                var ws = wb.Worksheet(1);
                var lastRow = ws.LastRowUsed() != null ? ws.LastRowUsed().RowNumber() : 0;
                var lastCol = ws.LastColumnUsed() != null ? ws.LastColumnUsed().ColumnNumber() : 0;

                if (lastRow < 2 || lastCol == 0)
                    return result;

                var headers = BuildHeaderMap(ws, lastCol);

                RequireColumn(headers, "SubscriberID");
                RequireColumn(headers, "MeterID");
                RequireColumn(headers, "ReadingDate");
                RequireColumn(headers, "CurrentReading");

                for (int r = 2; r <= lastRow; r++)
                {
                    if (IsRowEmpty(ws, r, lastCol))
                        continue;

                    var row = new ReadingImportRow
                    {
                        RowNumber = r,
                        Notes = GetCellString(ws, r, headers, "Notes")
                    };

                    try
                    {
                        row.SubscriberID = GetCellInt(ws, r, headers, "SubscriberID");
                        row.MeterID = GetCellInt(ws, r, headers, "MeterID");
                        row.ReadingDate = GetCellDate(ws, r, headers, "ReadingDate");
                        row.CurrentReading = GetCellDecimal(ws, r, headers, "CurrentReading");

                        row.IsValid = true;
                        row.ErrorMessage = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        row.IsValid = false;
                        row.ErrorMessage = ex.Message;
                    }

                    result.Add(row);
                }
            }

            return result;
        }

        private Dictionary<string, int> BuildHeaderMap(IXLWorksheet ws, int lastCol)
        {
            var rawHeaders = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            for (int c = 1; c <= lastCol; c++)
            {
                string header = NormalizeHeader(ws.Cell(1, c).GetString());
                if (!string.IsNullOrWhiteSpace(header) && !rawHeaders.ContainsKey(header))
                    rawHeaders[header] = c;
            }

            var result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in HeaderAliases)
            {
                foreach (var alias in item.Value)
                {
                    string normalizedAlias = NormalizeHeader(alias);
                    if (rawHeaders.TryGetValue(normalizedAlias, out int columnIndex))
                    {
                        result[item.Key] = columnIndex;
                        break;
                    }
                }
            }

            return result;
        }

        private void RequireColumn(Dictionary<string, int> headers, string columnName)
        {
            if (!headers.ContainsKey(columnName))
                throw new Exception("العمود المطلوب غير موجود في الملف: " + columnName);
        }

        private bool IsRowEmpty(IXLWorksheet ws, int row, int lastCol)
        {
            for (int c = 1; c <= lastCol; c++)
            {
                if (!ws.Cell(row, c).IsEmpty())
                    return false;
            }
            return true;
        }

        private string GetCellString(IXLWorksheet ws, int row, Dictionary<string, int> headers, string columnName)
        {
            if (!headers.ContainsKey(columnName))
                return string.Empty;

            return NormalizeText(ws.Cell(row, headers[columnName]).GetFormattedString());
        }

        private int GetCellInt(IXLWorksheet ws, int row, Dictionary<string, int> headers, string columnName)
        {
            var cell = ws.Cell(row, headers[columnName]);

            if (cell.IsEmpty())
                throw new Exception("العمود " + columnName + " فارغ");

            if (cell.TryGetValue<int>(out int intValue))
                return intValue;

            if (cell.TryGetValue<double>(out double doubleValue))
                return Convert.ToInt32(doubleValue);

            string raw = NormalizeText(cell.GetFormattedString());
            if (string.IsNullOrWhiteSpace(raw))
                throw new Exception("العمود " + columnName + " فارغ");

            if (decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal decimalValue))
                return Convert.ToInt32(decimalValue);

            if (decimal.TryParse(raw, NumberStyles.Any, CultureInfo.CurrentCulture, out decimalValue))
                return Convert.ToInt32(decimalValue);

            throw new Exception("القيمة في " + columnName + " غير صحيحة");
        }

        private decimal GetCellDecimal(IXLWorksheet ws, int row, Dictionary<string, int> headers, string columnName)
        {
            var cell = ws.Cell(row, headers[columnName]);

            if (cell.IsEmpty())
                throw new Exception("العمود " + columnName + " فارغ");

            if (cell.TryGetValue<decimal>(out decimal decValue))
                return decValue;

            if (cell.TryGetValue<double>(out double doubleValue))
                return Convert.ToDecimal(doubleValue);

            string raw = NormalizeText(cell.GetFormattedString());
            if (string.IsNullOrWhiteSpace(raw))
                throw new Exception("العمود " + columnName + " فارغ");

            raw = raw.Replace(",", ".");

            if (decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out decValue))
                return decValue;

            if (decimal.TryParse(raw, NumberStyles.Any, CultureInfo.CurrentCulture, out decValue))
                return decValue;

            throw new Exception("القيمة في " + columnName + " غير صحيحة");
        }

        private DateTime GetCellDate(IXLWorksheet ws, int row, Dictionary<string, int> headers, string columnName)
        {
            var cell = ws.Cell(row, headers[columnName]);

            if (cell.IsEmpty())
                throw new Exception("العمود " + columnName + " فارغ");

            if (cell.TryGetValue<DateTime>(out DateTime dt))
                return dt.Date;

            if (cell.TryGetValue<double>(out double oaValue))
            {
                try
                {
                    return DateTime.FromOADate(oaValue).Date;
                }
                catch
                {
                }
            }

            string raw = NormalizeText(cell.GetFormattedString());
            if (string.IsNullOrWhiteSpace(raw))
                throw new Exception("العمود " + columnName + " فارغ");

            string[] formats =
            {
                "yyyy-MM-dd",
                "dd/MM/yyyy",
                "d/M/yyyy",
                "MM/dd/yyyy",
                "M/d/yyyy",
                "dd-MM-yyyy",
                "d-M-yyyy"
            };

            if (DateTime.TryParseExact(raw, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return dt.Date;

            if (DateTime.TryParse(raw, CultureInfo.CurrentCulture, DateTimeStyles.None, out dt))
                return dt.Date;

            if (DateTime.TryParse(raw, out dt))
                return dt.Date;

            throw new Exception("القيمة في " + columnName + " ليست تاريخًا صحيحًا");
        }

        private string NormalizeHeader(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            string s = input.Trim();
            s = s.Replace("_", "").Replace(" ", "").Replace("-", "");
            s = s.Replace("أ", "ا").Replace("إ", "ا").Replace("آ", "ا");
            s = s.Replace("ة", "ه").Replace("ى", "ي");

            return s.ToLowerInvariant();
        }

        private string NormalizeText(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            char[] chars = input.Trim().ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                switch (chars[i])
                {
                    case '٠': chars[i] = '0'; break;
                    case '١': chars[i] = '1'; break;
                    case '٢': chars[i] = '2'; break;
                    case '٣': chars[i] = '3'; break;
                    case '٤': chars[i] = '4'; break;
                    case '٥': chars[i] = '5'; break;
                    case '٦': chars[i] = '6'; break;
                    case '٧': chars[i] = '7'; break;
                    case '٨': chars[i] = '8'; break;
                    case '٩': chars[i] = '9'; break;
                    case '٫': chars[i] = '.'; break;
                    case '٬': chars[i] = ','; break;
                }
            }

            return new string(chars);
        }
    }
}