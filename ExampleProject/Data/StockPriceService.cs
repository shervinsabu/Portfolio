using System;
using System.Data.SqlClient;
namespace ExampleProject.Data
{
	public interface StockPriceInterface
	{
		Task UploadStockPrices(string filePath);
        Task DeleteFileFromDB();
    }

	public class StockPriceService : StockPriceInterface
	{
        private readonly IConfiguration _config;

        const string CONN_KEY = "UserDatabase";

        public StockPriceService(IConfiguration config)
		{
            _config = config;
        }

		public async Task UploadStockPrices(string filePath)
		{
            string[] currentRow;
            int recordCount = 0;
            string line;
            SqlConnection conn = new SqlConnection();
            var connectionString = _config.GetConnectionString(CONN_KEY);
            conn.ConnectionString = connectionString;
            conn.Open();
            Console.WriteLine("Connection successfully Established");
            var reader = new StreamReader(filePath);
            while ((line = reader.ReadLine()) != null)
            {
                if (recordCount != 0)
                {
                    currentRow = line.Split(",");
                    string insertQuery = "INSERT INTO StockPrices (StockDate, OpenValue, HighValue, LowValue, " +
                        "CloseValue, AdjCloseValue, Volume, Symbol) VALUES (@stockDate, @openValue, @highValue, " +
                        "@lowValue, @closeValue, @adjCloseValue, @volume, @symbol)";
                    Console.WriteLine("1st data :" + currentRow[0]);
                    /*double d = double.Parse(currentRow[0]);
                    DateTime dt = DateTime.FromOADate(d);*/
                    Console.WriteLine("Stock Dates :" + DateOnly.Parse(currentRow[0]));
                    using (var cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@stockDate", DateOnly.Parse(currentRow[0]).ToString());
                        cmd.Parameters.AddWithValue("@openValue", decimal.Parse(currentRow[1]));
                        cmd.Parameters.AddWithValue("@highValue", decimal.Parse(currentRow[2]));
                        cmd.Parameters.AddWithValue("@lowValue", decimal.Parse(currentRow[3]));
                        cmd.Parameters.AddWithValue("@closeValue", decimal.Parse(currentRow[4]));
                        cmd.Parameters.AddWithValue("@adjCloseValue", decimal.Parse(currentRow[5]));
                        cmd.Parameters.AddWithValue("@volume", long.Parse(currentRow[6]));
                        if (filePath.IndexOf("ULVR.L", StringComparison.OrdinalIgnoreCase) >= 0)
                            cmd.Parameters.AddWithValue("@symbol", "ULVR");
                        else if(filePath.IndexOf("VOD.L", StringComparison.OrdinalIgnoreCase) >= 0)
                            cmd.Parameters.AddWithValue("@symbol", "VOD");

                        cmd.ExecuteNonQuery();
                    }
                }
                recordCount++;
            }
            Console.WriteLine("Connection is about to close");
            conn.Close();
            Console.WriteLine("Connection closed");
        }

        public async Task DeleteFileFromDB()
        {
            SqlConnection conn = new SqlConnection();
            var connectionString = _config.GetConnectionString(CONN_KEY);
            conn.ConnectionString = connectionString;
            conn.Open();
            Console.WriteLine("Connection successfully Established");
            try
            {
                string deleteQuery = "DELETE FROM StockPrices";
                using (var cmd = new SqlCommand(deleteQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
                Console.WriteLine("Connection Closed");
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in DeleteFileFromDBs :" + e.Message);
            }
        }

    }
}

