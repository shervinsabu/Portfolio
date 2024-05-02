using System;
namespace ExampleProject.Data
{
	public class StockPrice
	{
        public DateOnly stockDate { get; set; }
        public decimal openValue { get; set; }
        public decimal highValue { get; set; }
        public decimal lowValue { get; set; }
        public decimal closeValue { get; set; }
        public decimal adjCloseValue { get; set; }
        public long volume { get; set; }
    }
}

