using System;
namespace ExampleProject.Data
{
	public class Transactions
	{
        public DateOnly date { get; set; }
        public string transaction { get; set; }
        public string symbol { get; set; }
        public decimal purchaseprice { get; set; }
        public int quantity { get; set; }
        public decimal total { get; set; }
        public decimal balance { get; set; }
    }
}

