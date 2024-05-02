using System;
using System.Data.SqlClient;
using OfficeOpenXml;

namespace ExampleProject.Data
{
	public interface TransactionInterface
	{
		Task UploadFiletoDB(string filepath);
        Task DeleteFileFromDB();
    }

	public class TransactionService : TransactionInterface
    {
        private readonly IConfiguration _config;

        const string CONN_KEY = "UserDatabase";

        public TransactionService(IConfiguration config)
		{
            _config = config;
        }

		public async Task UploadFiletoDB(string filepath)
        {
            List<Transactions> transactions = new List<Transactions>();
            SqlConnection conn = new SqlConnection();

            var connectionString = _config.GetConnectionString(CONN_KEY);
            transactions = ReadingData(filepath);
            conn.ConnectionString = connectionString;
            conn.Open();
            Console.WriteLine("Connection successfully Established");

            foreach (var transaction in transactions)
            {
                string insertQuery = "INSERT INTO Transactions (TransactionDate, TransactionType, Symbol, Purchaseprice, " +
                        "Quantity, Total, Balance) VALUES (@date, @transaction, @symbol, @purchaseprice, @quantity, @total, @balance)";
                using (var cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@date", transaction.date.ToString());
                    cmd.Parameters.AddWithValue("@transaction", transaction.transaction);
                    cmd.Parameters.AddWithValue("@symbol", transaction.symbol);
                    cmd.Parameters.AddWithValue("@purchaseprice", transaction.purchaseprice);
                    cmd.Parameters.AddWithValue("@quantity", transaction.quantity);
                    cmd.Parameters.AddWithValue("@total", transaction.total);
                    cmd.Parameters.AddWithValue("@balance", transaction.balance);

                    cmd.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Connection is about to close");
            conn.Close();
            Console.WriteLine("Connection Closed");
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
                string deleteQuery = "DELETE FROM Transactions";
                using (var cmd = new SqlCommand(deleteQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
                Console.WriteLine("Connection Closed");
            }
            catch(Exception e)
            {
                Console.WriteLine("Exception in DeleteFileFromDB :" + e.Message);
            }
        }

        List<Transactions> ReadingData(string filePath)
        {
            var transactions = new List<Transactions>();
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                for (int row = 3; row <= worksheet.Dimension.End.Row; row++)
                {
                    double d = double.Parse(worksheet.Cells[row, 2].Value.ToString());
                    DateTime dt = DateTime.FromOADate(d);
                    Console.WriteLine("Dates :" + DateOnly.FromDateTime(dt));
                    Console.WriteLine("Dates11 :" + worksheet.Cells[row, 3].Value.ToString());
                    Console.WriteLine("Dates12233 :" + worksheet.Cells[row, 4].Value.ToString());
                    bool a = String.IsNullOrEmpty(worksheet.Cells[row, 5].Value.ToString());
                    Console.WriteLine("aaa:"+a);
                    if (worksheet.Cells[row, 5].Value == "" || worksheet.Cells[row, 5].Value.ToString() == "" ||
                        worksheet.Cells[row, 5].Value.ToString() is null || worksheet.Cells[row, 5].Value is null)
                    {
                        Console.WriteLine("purchaseprice is null");
                    }
                    else
                    Console.WriteLine("purchaseprice :" + worksheet.Cells[row, 5].Value.ToString());
                    var transaction = new Transactions
                    {
                        date = DateOnly.FromDateTime(dt),
                        transaction = worksheet.Cells[row, 3].Value.ToString(),
                        symbol = worksheet.Cells[row, 4].Value.ToString(),
                        purchaseprice = decimal.Parse((worksheet.Cells[row, 5].Value.ToString() != null || worksheet.Cells[row, 5].Value.ToString() != "") ? worksheet.Cells[row, 5].Value.ToString() : "0.00"),
                        quantity = Convert.ToInt32(worksheet.Cells[row, 6].Value.ToString() != null || worksheet.Cells[row, 6].Value.ToString() != "" ? worksheet.Cells[row, 6].Value : "0.00"),
                        total = decimal.Parse(worksheet.Cells[row, 7].Value.ToString()),
                        balance = decimal.Parse(worksheet.Cells[row, 8].Value.ToString() != null || worksheet.Cells[row, 8].Value.ToString() != "" ? worksheet.Cells[row, 8].Value.ToString() : "0.00")
                    };
                    transactions.Add(transaction);
                }
            }
            return transactions;
        }
    }
}

