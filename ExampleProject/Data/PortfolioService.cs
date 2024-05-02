using System;
using System.Data.SqlClient;
using System.Globalization;
using ExampleProject.Pages;

namespace ExampleProject.Data
{
    public class PortfolioService
    {
        private readonly IConfiguration _config;

        const string CONN_KEY = "UserDatabase";

        public PortfolioService(IConfiguration config)
        {
            _config = config;
        }

        public List<Portfolio> processData()
        {
            List<Portfolio> portfolios = new List<Portfolio>();
            Portfolio portfolio = null;
            SqlConnection conn = new SqlConnection();
            try
            {
                var connectionString = _config.GetConnectionString(CONN_KEY);
                conn.ConnectionString = connectionString;
                conn.Open();
                Console.WriteLine("Connection successfully Established");
                var cmd = new SqlCommand("CalculatePortfolioValue", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    portfolio = new Portfolio();
                    portfolio.date = DateTime.Parse(reader["PortFolioDate"].ToString()).ToString("dd-MM-yyyy", CultureInfo.InvariantCulture);
                    portfolio.portfolioValue = decimal.Parse(reader["PortfolioValue"].ToString());
                    portfolio.cashValue = decimal.Parse(reader["CashValue"].ToString());
                    portfolio.ulvrValue = decimal.Parse(reader["ULVRValue"].ToString());
                    portfolio.vodValue = decimal.Parse(reader["VODValue"].ToString());
                    portfolios.Add(portfolio);
                }
                Console.WriteLine("Portfolio List size :" + portfolios.Count);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception :" + e.Message);
            }
            return portfolios;
        }
    }
}

