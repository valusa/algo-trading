using System;

namespace AlgoTrading.POCO {
    public class StockMetric {
        public decimal OpenPrice { get; set; }
        public decimal ClosePrice { get; set; }
        public decimal Volume { get; set; }
        public DateTime Date { get; set; }

    }
}
