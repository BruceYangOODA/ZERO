using ZERO.Database.StockInfo;
using ZERO.Database;
using ZERO.Models.Enum;
using ZERO.Models;
using ZERO.Repository.IRepository;
using System.Collections.Generic;
using System.Transactions;
using System.Linq.Expressions;
using ZERO.Models.Dto.StockInfo;
using System.Linq;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using Dapper;
using Microsoft.EntityFrameworkCore;
//using static Dapper.SqlMapper;

namespace ZERO.Repository
{
    public class DayTradingRepository : BaseRepository<DayTradingRepository>, IDayTradingRepository
    {      
   
        public DayTradingRepository(IConfiguration configuration, ILogger<DayTradingRepository> logger, StockInfoContext context) : base(configuration, logger, context)
        {
        }

        public async Task<IEnumerable<QuoteInfoDto>> QueryFiveDayQuoteInfo(string date) 
        {
            try 
            {
                int year, month, day;
                if (date == "" || date.Length != 8)
                {
                    year = DateTime.Now.Year;
                    month = DateTime.Now.Month;
                    day = DateTime.Now.Day;
                }
                else
                {                    
                    year = int.Parse(date.Substring(0, 4));
                    month = int.Parse(date.Substring(4, 2));
                    day = int.Parse(date.Substring(6, 2));              
                }
                var tagetDay = new DateTime(year, month, day, 16, 0, 0, DateTimeKind.Utc).AddDays(-1);
                
                long unixTimestamp = (long)(tagetDay - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                tagetDay = tagetDay.AddDays(-7);                
                long limitTimestamp = (long)(tagetDay - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;

                Console.WriteLine("tagetDay" + tagetDay.DayOfWeek);
                Console.WriteLine("unixTimestamp"+ unixTimestamp);
                Console.WriteLine("limitTimestamp"+ limitTimestamp);
                IEnumerable<QuoteInfoDto> result;
                string sql = @"
                    SELECT qi.id, qi.date, qi.open, qi.high, qi.low, qi.close, qi.millionAmount, qi.volume, qi.buyAmount, qi.sellAmount, qi.sharesVolume, qi.unixTimestamp,
                    fbs.buy AS foreignBuy, fbs.sell AS foreignSell, 
                    tbs.buy AS trustBuy, tbs.sell AS trustSell,
                    dbs.buy AS dealerBuy, dbs.sell AS dealerSell  
                    FROM stock_info.quote_info qi
                    left join stock_info.foreign_buy_sell fbs on qi.date = fbs.date and qi.id = fbs.investrueId
                    left join stock_info.trust_buy_sell tbs on qi.date = tbs.date and qi.id = tbs.investrueId
                    left join stock_info.dealer_buy_sell dbs on qi.date = dbs.date and qi.id = dbs.investrueId
                    WHERE qi.unixTimestamp <= @unixTimestamp AND qi.unixTimestamp >= @limitTimestamp AND qi.buyAmount > 0
                    ORDER BY qi.id ASC, qi.date DESC
                    ;
                ";
                using (var sqlConnection = new MySqlConnection(_connectStringStockInfo))
                {
                    await sqlConnection.OpenAsync();
                    var temp = await sqlConnection.QueryAsync<QuoteInfoDto>(sql, new { unixTimestamp = unixTimestamp, limitTimestamp = limitTimestamp });                    
                    result = temp;                    
                }
                return result;
            } 
            catch(Exception e) 
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                throw;
            }
        }
    }
}
