using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;

namespace AlgoTrading {
    class Program {
        static void Main(string[] args) {

            Console.Write("Starting Process");
            string path = @"E:\E Drive\Projects\algo-trading\AlgoTrading\AlgoTrading\data.csv";
            DataTable dataTable = ConvertCSVtoDataTable(path);
            dataTable = dataTable.Rows
                        .Cast<DataRow>()
                        .Where(row => !row.ItemArray.All(field => field is DBNull ||
                                                         string.IsNullOrWhiteSpace(field as string)))
                        .CopyToDataTable();

            var dvData = new DataView(dataTable);
            int count = dvData.Count;

            decimal maxProfit = 0;

            for (int i = count - 1; i > 0; i--) {
                
                decimal close = Decimal.Parse(dvData[i]["Close/Last"].ToString(), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, new CultureInfo("en-US"));
                decimal opening = Decimal.Parse(dvData[i - 1]["Close/Last"].ToString(), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, new CultureInfo("en-US"));

                if (opening < close) {
                    maxProfit += (close - opening);
                }
            }

            Console.Write("Max Profit: " + maxProfit.ToString("C"));

        }

        public static DataTable ConvertCSVtoDataTable(string strFilePath) {
            var dt = new DataTable();
            using (var sr = new StreamReader(strFilePath)) {
                string[] headers = sr.ReadLine().Split(',');
                foreach (string header in headers) {
                    dt.Columns.Add(header);
                }
                while (!sr.EndOfStream) {
                    string[] rows = sr.ReadLine().Split(',');
                    DataRow dr = dt.NewRow();
                    for (int i = 0; i < headers.Length; i++) {
                        dr[i] = rows[i];
                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;
        }
    }
}
