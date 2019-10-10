using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections;
using System.Diagnostics;

namespace JuristicMonitor
{
    public class DbMango
    {
        MongoClient client = null;
        IMongoDatabase twstock_db = null;
        public void connect()
        {
            client = new MongoClient("mongodb://localhost:27017");
            twstock_db = client.GetDatabase("twstock");
        }

        public void FinancialReport_save(BsonDocument data, String stock_index, int year, int season)
        {
            var collection = twstock_db.GetCollection<BsonDocument>("financial_report");
            data.Add("stock_index", stock_index);
            data.Add("year", year);
            data.Add("season", season);
            collection.InsertOne(data);

            //var filter = Builders<Dictionary<String, String>>.Filter.Eq("name", "dsfasdf");
            //var filter = Builders<BsonDocument>.Filter.Eq("name", "dsfasdf");
            //var query = Query.EQ("name", "dsfasdf");
            //var res = collection.Find(filter).First();
            //var res = collection.Find(x=>x["stock_index"] == "2379").First();
            //Debug.Write(res["year"]);
        }

        public YearSeason FinancialReport_FindLatest(String stock_index)
        {
            YearSeason res = null;
            var collection = twstock_db.GetCollection<BsonDocument>("financial_report");

            //var filter = Builders<Dictionary<String, String>>.Filter.Eq("name", "dsfasdf");
            //var filter = Builders<BsonDocument>.Filter.Eq("name", "dsfasdf");
            //var query = Query.EQ("name", "dsfasdf");
            //var res = collection.Find(filter).First();
            var stock_found = collection.Find(x => x["stock_index"] == stock_index).Sort("{year:1, season:1}").Limit(10).ToList();
            if(stock_found.Count != 0)
            {
                res = new YearSeason(stock_found[0]["year"].AsInt32, stock_found[0]["season"].AsInt32);
            }
            return res;
        }

        public void StockData_Save(ArrayList data)
        {
            
            BsonDocument stock = new BsonDocument();
            stock.Add("stock_index", 2379);

            foreach(Object obj in data)
            {
                BsonDocument perday = new BsonDocument();
                StockTradeInfo stock_info = (StockTradeInfo)obj;

                var share_price = twstock_db.GetCollection<BsonDocument>(stock_info.stock_index);
                perday.Add("trans_volume", stock_info.trans_volume);
                perday.Add("trans_count", stock_info.trans_count);
                perday.Add("trans_value", stock_info.trans_value);
                perday.Add("open", stock_info.open);
                perday.Add("high", stock_info.high);
                perday.Add("low", stock_info.low);
                perday.Add("close", stock_info.close);
                perday.Add("diff", stock_info.diff);
                perday.Add("close_buy_price", stock_info.close_buy_price);
                perday.Add("close_sell_price", stock_info.close_sell_price);
                perday.Add("pe_ratio", stock_info.pe_ratio);
                perday.Add("close_buy_volume", stock_info.close_buy_volume);
                perday.Add("close_sell_volume", stock_info.close_sell_volume);

                share_price.InsertOne(perday);
            }
        }
    }
}
