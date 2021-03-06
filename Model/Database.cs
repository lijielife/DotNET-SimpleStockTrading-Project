﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TradingApp.Model
{
    class Database
    {
        public SqlConnection conn;



        //Setting up database connection 
        public Database()
        {
            conn = new SqlConnection(@"Data Source=john-gri.database.windows.net;Initial Catalog=TradingDB;Persist Security Info=True;User ID=dbadmin;Password=JohnIsGreat2000");
            conn.Open();
        }

        public List<Entities.StockDb> GetAllStockPricesFromDatabase()
        {
            List<Entities.StockDb> result = new List<Entities.StockDb>();

            using (SqlCommand command = new SqlCommand("SELECT * FROM StockQuotesTable", conn))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = (int)reader["StockId"];
                    string symbol = (string)reader["Symbol"];
                    string name = (string)reader["Name"];
                    decimal? bid = (decimal)reader["Bid"];
                    decimal? ask = (decimal)reader["Ask"];
                    decimal? open = (decimal)reader["Open"];
                    decimal? previousClose = (decimal)reader["PreviousClose"];
                    decimal? lastTrade = (decimal)reader["LastTrade"];
                    decimal? high = (decimal)reader["High"];
                    decimal? low = (decimal)reader["Low"];
                    int volume = (int)reader["Volume"];
                    decimal? high52 = (decimal)reader["High52"];
                    decimal? low52 = (decimal)reader["Low52"];
                    Entities.StockDb s = new Entities.StockDb(id, symbol, name, bid, ask, open, previousClose, lastTrade, high, low, volume, high52, low52);
                    result.Add(s);
                }
            }
            return result;
        }

        

       

        //This method is used to get all Symbols from database 
        public List<String> GetAllSymbolsFromDatabase()
        {
            List<String> result = new List<String>();

            using (SqlCommand command = new SqlCommand("SELECT Symbol FROM StockQuotesTable", conn))
            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(reader.GetString(0));
                }
            }

            return result;
        }




        //Method adds Stock To Database in case if record does not exists
        public void AddStockToStockTable(YahooStock s)
        {

            string sql = "INSERT INTO StockQuotesTable (Symbol, [Name], Bid, Ask, [Open], PreviousClose, LastTrade, Volume, High, Low, High52, Low52)"
                        + "VALUES (@Symbol, @Name, @Bid, @Ask, @Open, @PreviousClose, @LastTrade, @Volume, @High, @Low, @High52, @Low52)";

            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@Symbol", SqlDbType.NChar).Value = s.Symbol.Trim();
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar).Value = s.Name.Trim();
            cmd.Parameters.Add("@Bid", SqlDbType.Money).Value = s.Bid ?? SqlMoney.Null;
            cmd.Parameters.Add("@Ask", SqlDbType.Money).Value = s.Ask ?? SqlMoney.Null;
            cmd.Parameters.Add("@Open", SqlDbType.Money).Value = s.Open ?? SqlMoney.Null;
            cmd.Parameters.Add("@PreviousClose", SqlDbType.Money).Value = s.PreviousClose ?? SqlMoney.Null;
            cmd.Parameters.Add("@LastTrade", SqlDbType.Money).Value = s.LastTrade ?? SqlMoney.Null;
            cmd.Parameters.Add("@Volume", SqlDbType.Int).Value = s.Volume;
            cmd.Parameters.Add("@High", SqlDbType.Money).Value = s.High ?? SqlMoney.Null;
            cmd.Parameters.Add("@Low", SqlDbType.Money).Value = s.Low ?? SqlMoney.Null;
            cmd.Parameters.Add("@High52", SqlDbType.Money).Value = s.High52 ?? SqlMoney.Null;
            cmd.Parameters.Add("@Low52", SqlDbType.Money).Value = s.Low52 ?? SqlMoney.Null;

            cmd.ExecuteNonQuery();
        }

        

        public void DeleteAllFromQoutesHistoryTable()
        {
            using (SqlCommand cmd = new SqlCommand("DELETE FROM QoutesHistory", conn))
            {


                cmd.ExecuteNonQuery();
            }
        }


        public void DelteAllRecordWhereQtyIsZeroFromPortfolio()
        {

            string sql = "Delete FROM PortfolioStock Where NumberOfSharesOwned=0";
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.ExecuteNonQuery();

        }



        //Method updates record if it already exists in database
        public void UpdateStockToStockTable(YahooStock s)
        {

            string sql = "UPDATE StockQuotesTable " +
                "SET Bid=@Bid, Ask=@Ask, [Open]=@Open, PreviousClose=@PreviousClose, LastTrade=@LastTrade, Volume =@Volume, High=@High, Low=@Low, High52=@High52, Low52=@low52 " +
                "WHERE Symbol=@Symbol";

            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@Symbol", SqlDbType.NChar).Value = s.Symbol;
            cmd.Parameters.Add("@Bid", SqlDbType.Money).Value = s.Bid ?? SqlMoney.Null;
            cmd.Parameters.Add("@Ask", SqlDbType.Money).Value = s.Ask ?? SqlMoney.Null;
            cmd.Parameters.Add("@Open", SqlDbType.Money).Value = s.Open ?? SqlMoney.Null;
            cmd.Parameters.Add("@PreviousClose", SqlDbType.Money).Value = s.PreviousClose ?? SqlMoney.Null;
            cmd.Parameters.Add("@LastTrade", SqlDbType.Money).Value = s.LastTrade ?? SqlMoney.Null;
            cmd.Parameters.Add("@Volume", SqlDbType.Int).Value = s.Volume;
            cmd.Parameters.Add("@High", SqlDbType.Money).Value = s.High ?? SqlMoney.Null;
            cmd.Parameters.Add("@Low", SqlDbType.Money).Value = s.Low ?? SqlMoney.Null;
            cmd.Parameters.Add("@High52", SqlDbType.Money).Value = s.High52 ?? SqlMoney.Null;
            cmd.Parameters.Add("@Low52", SqlDbType.Money).Value = s.Low52 ?? SqlMoney.Null;

            cmd.ExecuteNonQuery();
        }


        ////Adding a transaction buy
        public void AddBuyTransaction(Entities.Portfolio p, Entities.StockDb s, int quantity)
        {

            string sqlBuy = "INSERT INTO Transactions (PortfolioId, Type, Symbol, BuySellPrice, SharesBoughtSold, Date)"
                        + "VALUES (@PortfolioId, @Type, @Symbol, @Ask, @SharesBought, @Date)";

            SqlCommand cmd = new SqlCommand(sqlBuy, conn);
            cmd.Parameters.Add("@PortfolioId", SqlDbType.Int).Value = p.PortfolioID;
            cmd.Parameters.Add("@Type", SqlDbType.NChar).Value = "Buy";
            cmd.Parameters.Add("@Symbol", SqlDbType.NChar).Value = s.Symbol;
            cmd.Parameters.Add("@Ask", SqlDbType.Money).Value = s.Ask;
            cmd.Parameters.Add("@SharesBought", SqlDbType.Int).Value = quantity;
            cmd.Parameters.Add("@Date", SqlDbType.DateTime).Value = DateTime.Now;

            cmd.ExecuteNonQuery();

            string sqlUpdateCash = "Update Portfolio SET Cash=@Cash Where PortfolioId=@PortfolioId";

            SqlCommand cmdUpdate = new SqlCommand(sqlUpdateCash, conn);
            cmdUpdate.Parameters.Add("@PortfolioId", SqlDbType.Int).Value = p.PortfolioID;
            cmdUpdate.Parameters.Add("@Cash", SqlDbType.Money).Value = p.Cash - (s.Ask * quantity);
            cmdUpdate.ExecuteNonQuery();

        }


        public void AddSellTransaction(String symbol, int qty, decimal sellPrice, Entities.PortfolioStock p, Entities.Portfolio up)
        {

            string sqlBuy = "INSERT INTO Transactions (PortfolioId, Type, Symbol, BuySellPrice, SharesBoughtSold, Date)"
                        + "VALUES (@PortfolioId, @Type, @Symbol, @Bit, @SharesBought, @Date)";

            SqlCommand cmd = new SqlCommand(sqlBuy, conn);
            cmd.Parameters.Add("@PortfolioId", SqlDbType.Int).Value = p.PortfolioId;
            cmd.Parameters.Add("@Type", SqlDbType.NChar).Value = "Sell";
            cmd.Parameters.Add("@Symbol", SqlDbType.NChar).Value = symbol;
            cmd.Parameters.Add("@Bit", SqlDbType.Money).Value = sellPrice;
            cmd.Parameters.Add("@SharesBought", SqlDbType.Int).Value = qty;
            cmd.Parameters.Add("@Date", SqlDbType.DateTime).Value = DateTime.Now;

            cmd.ExecuteNonQuery();

            string sqlUpdateCash = "Update Portfolio SET Cash=@Cash Where PortfolioId=@PortfolioId";

            SqlCommand cmdUpdate = new SqlCommand(sqlUpdateCash, conn);
            cmdUpdate.Parameters.Add("@PortfolioId", SqlDbType.Int).Value = p.PortfolioId;
            cmdUpdate.Parameters.Add("@Cash", SqlDbType.Money).Value = up.Cash + (sellPrice * qty);
            cmdUpdate.ExecuteNonQuery();


            string sqlUpdatPortfolioStock = "Update PortfolioStock SET NumberOfSharesOwned=@NumberOfSharesOwned Where GameID=@GameID AND Symbol=@Symbol";

            SqlCommand cmdUpdatePortfolioStock = new SqlCommand(sqlUpdatPortfolioStock, conn);
            cmdUpdatePortfolioStock.Parameters.Add("@GameID", SqlDbType.Int).Value = p.PortfolioId;
            cmdUpdatePortfolioStock.Parameters.Add("@Symbol", SqlDbType.NChar).Value = symbol;
            cmdUpdatePortfolioStock.Parameters.Add("@NumberOfSharesOwned", SqlDbType.Int).Value =p.SharesOwned - qty;
            cmdUpdatePortfolioStock.ExecuteNonQuery();




        }

        public void AddToPortfolioStock(Entities.Portfolio p, Entities.StockDb s, int quantity)
        {
            string sqlAddToPortfolioStock = "INSERT INTO PortfolioStock (Symbol, GameID, NumberOfSharesOwned, AveragePurchasePrice)" +
    "Values (@Symbol, @GameID, @NumberOfSharesOwned, @AveragePurchasePrice)";

            SqlCommand cmdInsertStock = new SqlCommand(sqlAddToPortfolioStock, conn);
            cmdInsertStock.Parameters.Add("@Symbol", SqlDbType.NChar).Value = s.Symbol;
            cmdInsertStock.Parameters.Add("@GameID", SqlDbType.Int).Value = p.PortfolioID;
            cmdInsertStock.Parameters.Add("@NumberOfSharesOwned", SqlDbType.Int).Value = quantity;
            cmdInsertStock.Parameters.Add("@AveragePurchasePrice", SqlDbType.Money).Value = s.Ask;
            cmdInsertStock.ExecuteNonQuery();

        }

        public void AddPortfolioStock(Entities.Portfolio p, Entities.StockDb s, int quantity)
        {
            string sqlAddToPortfolioStock = "INSERT INTO PortfolioStock (Symbol, GameID, NumberOfSharesOwned, AveragePurchasePrice)" +
    "Values (@Symbol, @GameID, @NumberOfSharesOwned, @AveragePurchasePrice)";

            SqlCommand cmdInsertStock = new SqlCommand(sqlAddToPortfolioStock, conn);
            cmdInsertStock.Parameters.Add("@Symbol", SqlDbType.NChar).Value = s.Symbol;
            cmdInsertStock.Parameters.Add("@GameID", SqlDbType.Int).Value = p.PortfolioID;
            cmdInsertStock.Parameters.Add("@NumberOfSharesOwned", SqlDbType.Int).Value = quantity;
            cmdInsertStock.Parameters.Add("@AveragePurchasePrice", SqlDbType.Money).Value = s.Ask;
            cmdInsertStock.ExecuteNonQuery();

        }

        public void UpdatePortfolioStock(Entities.Portfolio p, Entities.StockDb s, int quantity)
        {
            int finalQty=0;
            decimal newAverage=0;

            string sqlGetVolumePrice = "SELECT NumberOfSharesOwned, AveragePurchasePrice FROM PortfolioStock WHERE Symbol=@Symbol AND GameID=@GameID";

            SqlCommand cmdGetVolumePrice = new SqlCommand(sqlGetVolumePrice, conn);
            cmdGetVolumePrice.Parameters.Add("@Symbol", SqlDbType.NVarChar).Value = s.Symbol;
            cmdGetVolumePrice.Parameters.Add("@GameID", SqlDbType.Int).Value = p.PortfolioID;

            int quantityDB;
            decimal priceDB;


            // this part is needed to convert decimal? to decimal
            decimal askPrice = (decimal)s.Ask;
            using (SqlDataReader rd = cmdGetVolumePrice.ExecuteReader())
            {


                while (rd.Read())


                {
                    //rd.NextResult();

                    //var quantityDB = (int)rd["NumberOfSharesOwned"];

                    quantityDB = Convert.ToInt32(rd["NumberOfSharesOwned"]);
                    priceDB = (decimal)rd["AveragePurchasePrice"];


      
                decimal DatabaseTotalPrice = priceDB * quantityDB;


                    decimal NewTotalPrice = quantity * askPrice;



              finalQty = quantityDB + quantity;
               newAverage = (DatabaseTotalPrice + NewTotalPrice) / finalQty;


                    finalQty = quantityDB + quantity;
                    newAverage = (DatabaseTotalPrice + NewTotalPrice) / finalQty;

                }

            }

            string sqlAddToPortfolioStock = "UPDATE PortfolioStock SET NumberOfSHaresOWned=@NumberOfSHaresOWned, AveragePurchasePrice=@AveragePurchasePrice " +
                "WHERE Symbol=@Symbol AND GameID=@GameID";

            SqlCommand cmdUpdate = new SqlCommand(sqlAddToPortfolioStock, conn);
            cmdUpdate.Parameters.Add("@Symbol", SqlDbType.NChar).Value = s.Symbol;
            cmdUpdate.Parameters.Add("@GameID", SqlDbType.Int).Value = p.PortfolioID;
           cmdUpdate.Parameters.Add("@NumberOfSharesOwned", SqlDbType.Int).Value = finalQty;
           cmdUpdate.Parameters.Add("@AveragePurchasePrice", SqlDbType.Money).Value = newAverage;
            cmdUpdate.ExecuteNonQuery();
            Console.Write("END");


            cmdUpdate.ExecuteNonQuery();
        }




        public List<String> GetAllStockOwnedByUser(Entities.Portfolio p)
        {
            List<String> result = new List<String>();

            String sqlGetAllStockOwnedByUser = "SELECT Symbol FROM PortfolioStock WHERE GameID = @GameID";
            SqlCommand cmdInsertStock = new SqlCommand(sqlGetAllStockOwnedByUser, conn);
            cmdInsertStock.Parameters.Add("@GameID", SqlDbType.Int).Value = p.PortfolioID;

            using (SqlDataReader reader = cmdInsertStock.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(reader.GetString(0));
                }
            }

            return result;
        }

    }







}
