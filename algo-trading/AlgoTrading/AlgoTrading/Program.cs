using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using AlgoTrading.Calculator;
using ConsoleTables;

namespace AlgoTrading {
    class Program {
        static void Main(string[] args) {

            Console.Write("Enter a year - ");
            string yearChosen = Console.ReadLine();

            if (yearChosen == string.Empty) return;
            string path = @"E:\E Drive\Projects\algo-trading\AlgoTrading\AlgoTrading\msft.csv";
            DataView dvData = ConvertToDataView(yearChosen, ConvertCSVtoDataTable(path));

            int count = dvData.Count;
            decimal maxProfit = 0;


            var nineDaySMACalculator = new SimpleMovingAverage(k: 9);
            var eighteenDaySMACalculator = new SimpleMovingAverage(k: 18);
            var table = new ConsoleTable("Date", "Open Price", "Close Price", "Max Profit", "9 Day SMA", "18 day SMA", "BUY/SELL");


            for (int i = count - 1; i > 0; i--) {
                decimal open = Decimal.Parse(dvData[i]["Open"].ToString(), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, new CultureInfo("en-US"));
                decimal close = Decimal.Parse(dvData[i]["Close/Last"].ToString(), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, new CultureInfo("en-US"));
                decimal nextDayOpen = Decimal.Parse(dvData[i - 1]["Close/Last"].ToString(), NumberStyles.AllowCurrencySymbol | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, new CultureInfo("en-US"));
                // lets say if the stock opens at $5 and closes at $6, buy it at $6 and sell it next day
                // lets say if the stock opens at $5 and closes at $4 then do nothing
                if (open <= close) {
                    maxProfit += (close - nextDayOpen);
                }
                else {
                    maxProfit += (nextDayOpen - close);
                }
                decimal nineDaySMA = Math.Round(nineDaySMACalculator.Update(open), 3);
                decimal eighteenDaySMA = Math.Round(eighteenDaySMACalculator.Update(open), 3);
                bool isBuy = nineDaySMA > eighteenDaySMA;
                table.AddRow(dvData[i]["Date"].ToString(), dvData[i]["Open"].ToString(), dvData[i]["Close/Last"].ToString(), maxProfit.ToString("C") , nineDaySMA, eighteenDaySMA, isBuy ? "BUY" : "SELL");
            }

            // Console.Write("Max Profit: " + maxProfit.ToString("C"));
            Console.Write(table);

        }


        private static DataView ConvertToDataView(string yearChosen, DataTable dataTable) {
            dataTable = dataTable.Rows
                                    .Cast<DataRow>()
                                    .Where(row => !row.ItemArray.All(field => field is DBNull ||
                                                                     string.IsNullOrWhiteSpace(field as string)))
                                    .CopyToDataTable();

            var dvData = new DataView(dataTable) {
                RowFilter = "(Convert(Date,System.String) like '%" + yearChosen + "%')",
                Sort = "Date"
            };
            return dvData;
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
