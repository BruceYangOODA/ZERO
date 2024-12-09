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
using MySqlX.XDevAPI.Common;
//using static Dapper.SqlMapper;

namespace ZERO.Repository
{
    public class DayTradingRepository : BaseRepository<DayTradingRepository>, IDayTradingRepository
    {      
   
        public DayTradingRepository(IConfiguration configuration, ILogger<DayTradingRepository> logger, StockInfoContext context) : base(configuration, logger, context)
        {
        }
        public async Task<IEnumerable<TwTradeDayDto>> QueryUnixTime(string date, int ma)
        {
            try
            {
                string year, month, day;
                if (date == "")
                {
                    year = DateTime.Now.Year.ToString();
                    month = DateTime.Now.Month.ToString();
                    day = DateTime.Now.Day.ToString();
                }
                else
                {
                    year = date.Substring(0, 4);
                    month = date.Substring(4, 2);
                    day = date.Substring(6, 2);
                }
                var tagetDay = year + month + day;                
                IEnumerable<TwTradeDayDto> result;
                string sql = @"
                    SELECT * FROM stock_info.tw_trade_day ttd WHERE date <= @tagetDay
                    ORDER BY date DESC LIMIT @ma;
                ";
                using (var sqlConnection = new MySqlConnection(_connectStringStockInfo))
                {
                    await sqlConnection.OpenAsync();
                    var temp = await sqlConnection.QueryAsync<TwTradeDayDto>(sql, new { tagetDay = tagetDay, ma = ma });
                    result = temp;
                }
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                throw;
            }
        }
        public async Task<IEnumerable<QuoteInfoDto>> QueryQuoteInfo(IEnumerable<TwTradeDayDto> dayDtos)
        {
            try 
            {
                long unixTimestamp = dayDtos.First().unixTimestamp;
                long limitTimestamp = dayDtos.Last().unixTimestamp;
                IEnumerable<QuoteInfoDto> result;
                string sql = @"
                    SELECT qi.id, qi.date, qi.open, qi.high, qi.low, qi.close, qi.millionAmount, qi.volume, 
                    qi.buyAmount, qi.sellAmount, qi.sharesVolume, qi.unixTimestamp,
                    qi.amplitude, qi.fluctuation, qi.sharesRate,
                    fbs.buy AS foreignBuy, fbs.sell AS foreignSell, 
                    tbs.buy AS trustBuy, tbs.sell AS trustSell,
                    dbs.buy AS dealerBuy, dbs.sell AS dealerSell  
                    FROM stock_info.quote_info qi
                    left join stock_info.foreign_buy_sell fbs on qi.date = fbs.date and qi.id = fbs.investrueId
                    left join stock_info.trust_buy_sell tbs on qi.date = tbs.date and qi.id = tbs.investrueId
                    left join stock_info.dealer_buy_sell dbs on qi.date = dbs.date and qi.id = dbs.investrueId
                    WHERE qi.unixTimestamp <= @unixTimestamp AND qi.unixTimestamp >= @limitTimestamp
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
            catch (Exception e) 
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                throw;
            }
        }
        public async Task<IEnumerable<QuoteInfoDto>> QueryQuoteInfoById(IEnumerable<TwTradeDayDto> dayDtos, string id)
        {
            try
            {
                long unixTimestamp = dayDtos.First().unixTimestamp;
                long limitTimestamp = dayDtos.Last().unixTimestamp;
                IEnumerable<QuoteInfoDto> result;
                string sql = @"
                    SELECT qi.id, qi.date, qi.open, qi.high, qi.low, qi.close, qi.millionAmount, qi.volume, 
                    qi.buyAmount, qi.sellAmount, qi.sharesVolume, qi.unixTimestamp,
                    qi.amplitude, qi.fluctuation, qi.sharesRate,
                    fbs.buy AS foreignBuy, fbs.sell AS foreignSell, 
                    tbs.buy AS trustBuy, tbs.sell AS trustSell,
                    dbs.buy AS dealerBuy, dbs.sell AS dealerSell  
                    FROM stock_info.quote_info qi
                    left join stock_info.foreign_buy_sell fbs on qi.date = fbs.date and qi.id = fbs.investrueId
                    left join stock_info.trust_buy_sell tbs on qi.date = tbs.date and qi.id = tbs.investrueId
                    left join stock_info.dealer_buy_sell dbs on qi.date = dbs.date and qi.id = dbs.investrueId
                    WHERE qi.unixTimestamp <= @unixTimestamp AND qi.unixTimestamp >= @limitTimestamp AND qi.id = @id
                    ORDER BY qi.date DESC
                    ;
                ";
                using (var sqlConnection = new MySqlConnection(_connectStringStockInfo))
                {
                    await sqlConnection.OpenAsync();
                    var temp = await sqlConnection.QueryAsync<QuoteInfoDto>(sql, new { unixTimestamp = unixTimestamp, limitTimestamp = limitTimestamp, id = id });
                    result = temp;
                }
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                throw;
            }
        }

        public async Task<string> test() 
        {
            try 
            {
                string sql = @"
                    SELECT *
                    FROM stock_info.quote_info
                    WHERE date = @date
                    ORDER BY date DESC
                    ;
                ";

                string sql2 = @"
                    SELECT *
                    FROM stock_info.quote_info                    
                    WHERE id = @id
                    ORDER BY date DESC
                    ;
                ";

                string updateSql = @"
                    UPDATE stock_info.quote_info 
                    SET fluctuation = @fluctuation, amplitude = @amplitude, sharesRate = @sharesRate
                    WHERE id = @id AND date = @date;                     
                ";
                string updateSql2 = @"
                    UPDATE stock_info.quote_info 
                    SET sharesVolume = @sharesVolume
                    WHERE id = @id AND date = @date;                     
                ";
                using (var sqlConnection = new MySqlConnection(_connectStringStockInfo))
                {
                    await sqlConnection.OpenAsync();
                    var temp = await sqlConnection.QueryAsync<QuoteInfoDto>(sql, new { date = "20241203"});
                    foreach (var item in temp) 
                    {
                        var temp2 = (await sqlConnection.QueryAsync<QuoteInfoDto>(sql2, new { id = item.id })).ToArray();
                       /* QuoteInfoDto tempItem = temp2[0];
                        QuoteInfoDto currentItem;
                        var amplitude = 0f;
                        var fluctuation = 0f;
                        var sharesRate = 0f;*/
                        for (int i = 0; i < temp2.Count(); i++) 
                        {

                            var tempItem = temp2[i];
                            if (tempItem.sharesVolume != null && tempItem.sharesVolume != 0) 
                            {
                                tempItem.sharesVolume = tempItem.sharesVolume / 1000;
                                await sqlConnection.QueryAsync<QuoteInfoDto>(updateSql2, new
                                {
                                    id = tempItem.id,
                                    date = tempItem.date,
                                    sharesVolume = tempItem.sharesVolume
                                });
                            }
                            
                            /*amplitude = 0f;
                            fluctuation = 0f;
                            sharesRate = 0f;
                            currentItem = temp2[i];
                            if (currentItem.close != null && tempItem.close != null && currentItem.close != 0 && tempItem.close != 0) 
                            {
                                fluctuation = (Convert.ToSingle(currentItem.close) - Convert.ToSingle(tempItem.close)) / Convert.ToSingle(tempItem.close) * 100f;
                                amplitude = (Convert.ToSingle(currentItem.high) - Convert.ToSingle(currentItem.low)) / Convert.ToSingle(tempItem.close) * 100f;
                            }
                            if (currentItem.volume != null && currentItem.sharesVolume != null && currentItem.volume != 0 && currentItem.sharesVolume != 0)
                            {
                                sharesRate = (float)currentItem.sharesVolume / (float)currentItem.volume / 10f;
                            }
                            await sqlConnection.QueryAsync<QuoteInfoDto>(updateSql, new { 
                                id = currentItem.id, 
                                date = currentItem.date,
                                amplitude = amplitude,
                                fluctuation = fluctuation,
                                sharesRate = sharesRate,
                            });
                            tempItem = temp2[i];*/
                        }
                       // Console.WriteLine(JsonConvert.SerializeObject(temp2.First()));
                    }
                    

                }
                return "GG";
            }
            catch (Exception e) 
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                throw;
            }
        }

        /*public async Task<IEnumerable<QuoteInfoDto>> QuerySixDayQuoteInfo(IEnumerable<TwTradeDayDto> dayDtos) 
    {
        try 
        {

            long unixTimestamp = dayDtos.First().unixTimestamp;
            long limitTimestamp = dayDtos.Last().unixTimestamp;
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
                WHERE qi.unixTimestamp <= @unixTimestamp AND qi.unixTimestamp >= @limitTimestamp AND qi.volume > 0 AND qi.buyAmount > 0
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
    }*/
    }
}
