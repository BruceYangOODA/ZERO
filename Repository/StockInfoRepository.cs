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
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.VisualBasic;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json.Nodes;

namespace ZERO.Repository
{
    public class StockInfoRepository : BaseRepository<StockInfoRepository>, IStockInfoRepository
    {
       // private readonly ILogger<StockInfoRepository> _logger;
       // protected readonly StockInfoContext _context;
        public StockInfoRepository(IConfiguration configuration, ILogger<StockInfoRepository> logger, StockInfoContext context) : base(configuration, logger, context)
        {
           // _context = context;
          //  _logger = logger;            
        }
        /// <summary> 存入資料 </summary>
        /// <param name="para"></param>
        /// <returns></returns>        
        public async Task<IEnumerable<QuoteInfoDto>> QueryAllQuoteInfo()
        {
            try
            {
                IEnumerable<QuoteInfoDto> result;
                string sql = @"
                    SELECT * FROM stock_info.quote_info qi LIMIT 200;
                ";
                using (var sqlConnection = new MySqlConnection(_connectStringStockInfo))
                {
                    await sqlConnection.OpenAsync();
                    var temp = await sqlConnection.QueryAsync<QuoteInfoDto>(sql);
                    result = temp;
                    Console.WriteLine("result" + result.Count());
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
       
        public async Task<IEnumerable<QuoteInfoDto>> QueryQuoteInfoByDate(string theDate)
        {
            try
            {
                IEnumerable<QuoteInfoDto> result;
                string sql = @"
                    select * from stock_info.quote_info qi
                    where qi.date = @theDate;
                ";
                using (var sqlConnection = new MySqlConnection(_connectStringStockInfo))
                {
                    await sqlConnection.OpenAsync();
                    var temp = await sqlConnection.QueryAsync<QuoteInfoDto>(sql, new { theDate = theDate });
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
        public async Task<IEnumerable<ForeignBuySellDto>> QueryForeignBuySellByDate(string theDate)
        {
            try
            {
                IEnumerable<ForeignBuySellDto> result;
                string sql = @"
                    select * from stock_info.foreign_buy_sell bs
                    where bs.date = @theDate;
                ";
                using (var sqlConnection = new MySqlConnection(_connectStringStockInfo))
                {
                    await sqlConnection.OpenAsync();
                    var temp = await sqlConnection.QueryAsync<ForeignBuySellDto>(sql, new { theDate = theDate });
                    result = temp.ToList();
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

        public async Task<IEnumerable<DealerBuySellDto>> QueryDealerBuySellByDate(string theDate)
        {
            try
            {
                IEnumerable<DealerBuySellDto> result;
                string sql = @"
                    select * from stock_info.dealer_buy_sell bs
                    where bs.date = @theDate;
                ";
                using (var sqlConnection = new MySqlConnection(_connectStringStockInfo))
                {
                    await sqlConnection.OpenAsync();
                    var temp = await sqlConnection.QueryAsync<DealerBuySellDto>(sql, new { theDate = theDate });
                    result = temp.ToList();
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

        public async Task<IEnumerable<TrustBuySellDto>> QueryTrustBuySellByDate(string theDate)
        {
            try
            {
                IEnumerable<TrustBuySellDto> result;
                string sql = @"
                    select * from stock_info.trust_buy_sell bs
                    where bs.date = @theDate;
                ";
                using (var sqlConnection = new MySqlConnection(_connectStringStockInfo))
                {
                    await sqlConnection.OpenAsync();
                    var temp = await sqlConnection.QueryAsync<TrustBuySellDto>(sql, new { theDate = theDate });
                    result = temp.ToList();
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
        public async Task<TwTradeDayDto> InsertOrUpdateTwTradeDayDto(TwTradeDayDto dto) 
        {
            try 
            {
                IEnumerable<TwTradeDayDto> result;
                string sql = @"
                    select * from stock_info.tw_trade_day ttd
                    where ttd.date = @theDate;
                ";
                using (var sqlConnection = new MySqlConnection(_connectStringStockInfo))
                {
                    await sqlConnection.OpenAsync();
                    var temp = await sqlConnection.QueryAsync<TwTradeDayDto>(sql, new { theDate = dto.date });
                    result = temp.ToList();

                    if (result.Count() == 0)
                    {
                        string insertSql = @"
                        INSERT INTO stock_info.tw_trade_day (date, unixTimestamp) 
                        VALUES (@date, @unixTimestamp);";
                       
                        var tempA = await sqlConnection.QueryAsync<TwTradeDayDto>(insertSql, dto);
                       
                    }
                    else
                    {
                        string updateSql = @"
                        UPDATE stock_info.tw_trade_day 
                        SET unixTimestamp = @unixTimestamp
                        WHERE date = @date;  ";                   
                        var tempB = await sqlConnection.QueryAsync<TwTradeDayDto>(updateSql, dto);                       
                    }
                }
                
                
                return dto;
            }
            catch (Exception e)
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                throw;
            }
}

        public async Task<QuoteInfoDto> InsertQuoteInfo(QuoteInfoDto dto) 
        {
            try 
            {
                QuoteInfoDto result;
                string sql = @"
                    INSERT INTO stock_info.quote_info (id, date, open, high, low, close, millionAmount, volume, unixTimestamp) 
                    VALUES (@id, @date, @open, @high, @low, @close, @millionAmount, @volume, @unixTimestamp);             
                ";
                using (var sqlConnection = new MySqlConnection(_connectStringStockInfo))
                {
                    await sqlConnection.OpenAsync();
                    var temp = (await sqlConnection.QueryAsync<QuoteInfoDto>(sql, dto)).FirstOrDefault();
                    result = temp;
                    Console.WriteLine($"新增資料 QuoteInfo {dto.date} {dto.id} ");
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
        public async Task<QuoteInfoDto> UpdateQuoteInfo(QuoteInfoDto dto) 
        {
            try 
            {
                QuoteInfoDto result;
                string sql = @"
                    UPDATE stock_info.quote_info 
                    SET open = @open, high = @high, low = @low, close = @close, volume = @volume, millionAmount = @millionAmount, unixTimestamp = @unixTimestamp
                    WHERE id = @id AND date = @date;                    
                ";         

                using (var sqlConnection = new MySqlConnection(_connectStringStockInfo))
                {
                    await sqlConnection.OpenAsync();                    
                    var temp = await sqlConnection.ExecuteReaderAsync(sql, dto);
                    Console.WriteLine($"更新資料 QuoteInfo {dto.date} {dto.id} ");
                    return null;
                   
                }               
            }
            catch (Exception e) 
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                throw;
            }
        }

        public async Task<List<QuoteInfoDto>> UpdateListQuoteInfo(List<QuoteInfoDto> dtos)
        {
            try
            {
                string theDate = dtos.First().date;
                IEnumerable<QuoteInfoDto> quoteChecks = await QueryQuoteInfoByDate(theDate);
                List<QuoteInfoDto> checkList = quoteChecks.ToList();
                string updateSql = @"
                    update stock_info.quote_info 
                    set open = @open, high = @high, low = @low, close = @close, volume = @volume, millionAmount = @millionAmount, unixTimestamp = @unixTimestamp
                    where id = @id and date = @date;                    
                ";
                string insertSql = @"
                    INSERT INTO stock_info.quote_info (id, date, open, high, low, close, millionAmount, volume, unixTimestamp) 
                    VALUES (@id, @date, @open, @high, @low, @close, @millionAmount, @volume, @unixTimestamp);             
                ";

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    using (var sqlConnection = new MySqlConnection(_connectStringStockInfo))
                    {
                        await sqlConnection.OpenAsync();
                        foreach (var dto in dtos)
                        {
                            var find = checkList.Find((item) => { return dto.id == item.id && dto.date == item.date; });
                            if (find != null)
                            {                                
                                await sqlConnection.ExecuteAsync(updateSql, dto);
                                Console.WriteLine($"更新資料 QuoteInfo {dto.date} {dto.id} ");
                            }
                            else
                            {
                                await sqlConnection.ExecuteAsync(insertSql, dto);
                                Console.WriteLine($"新增資料 QuoteInfo {dto.date} {dto.id} ");
                            }

                        }
                    }
                    scope.Complete();
                }
                return dtos;
            }
            catch (Exception e)
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                throw;
            }
        }

        public async Task<List<ForeignBuySellDto>> UpdateListForeignBuySell(List<ForeignBuySellDto> dtos) 
        {
            try
            {
                string theDate = dtos.First().date;
                IEnumerable<ForeignBuySellDto> foreignChecks = await QueryForeignBuySellByDate(theDate);
                List< ForeignBuySellDto> checkList = foreignChecks.ToList();
                string updateSql = @"
                    update stock_info.foreign_buy_sell 
                    set buy = @buy, sell = @sell, unixTimestamp = @unixTimestamp
                    where investrueId = @investrueId and date = @date;                    
                ";
                string insertSql = @"
                    INSERT INTO stock_info.foreign_buy_sell (investrueId, date, buy, sell, unixTimestamp) 
                    VALUES (@investrueId, @date, @buy, @sell, @unixTimestamp);             
                ";

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    using (var sqlConnection = new MySqlConnection(_connectStringStockInfo))
                    {
                        await sqlConnection.OpenAsync();
                        foreach (var dto in dtos)
                        {
                            var find = checkList.Find((item) => { return dto.investrueId == item.investrueId && dto.date == item.date; });
                            if (find != null)
                            {
                                await sqlConnection.ExecuteAsync(updateSql, dto);
                                Console.WriteLine($"更新資料 foreign_buy_sell {dto.date} {dto.investrueId} ");
                            }
                            else
                            {
                                await sqlConnection.ExecuteAsync(insertSql, dto);
                                Console.WriteLine($"新增資料 foreign_buy_sell {dto.date} {dto.investrueId} ");
                            }

                        }
                    }
                    scope.Complete();
                }
                return dtos;
            }
            catch (Exception e)
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                throw;
            }
        }

        public async Task<List<DealerBuySellDto>> UpdateListDealerBuySell(List<DealerBuySellDto> dtos)
        {
            try
            {
                string theDate = dtos.First().date;
                IEnumerable<DealerBuySellDto> foreignChecks = await QueryDealerBuySellByDate(theDate);
                List<DealerBuySellDto> checkList = foreignChecks.ToList();
                string updateSql = @"
                    update stock_info.dealer_buy_sell 
                    set buy = @buy, sell = @sell, unixTimestamp = @unixTimestamp
                    where investrueId = @investrueId and date = @date;                    
                ";
                string insertSql = @"
                    INSERT INTO stock_info.dealer_buy_sell (investrueId, date, buy, sell, unixTimestamp) 
                    VALUES (@investrueId, @date, @buy, @sell, @unixTimestamp);             
                ";

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    using (var sqlConnection = new MySqlConnection(_connectStringStockInfo))
                    {
                        await sqlConnection.OpenAsync();
                        foreach (var dto in dtos)
                        {
                            var find = checkList.Find((item) => { return dto.investrueId == item.investrueId && dto.date == item.date; });
                            if (find != null)
                            {
                                await sqlConnection.ExecuteAsync(updateSql, dto);
                                Console.WriteLine($"更新資料 dealer_buy_sell {dto.date} {dto.investrueId} ");
                            }
                            else
                            {
                                await sqlConnection.ExecuteAsync(insertSql, dto);
                                Console.WriteLine($"新增資料 dealer_buy_sell {dto.date} {dto.investrueId} ");
                            }

                        }
                    }
                    scope.Complete();
                }
                return dtos;
            }
            catch (Exception e)
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                throw;
            }
        }

        public async Task<List<TrustBuySellDto>> UpdateListTrustBuySell(List<TrustBuySellDto> dtos)
        {
            try
            {
                string theDate = dtos.First().date;
                IEnumerable<TrustBuySellDto> foreignChecks = await QueryTrustBuySellByDate(theDate);
                List<TrustBuySellDto> checkList = foreignChecks.ToList();
                string updateSql = @"
                    update stock_info.trust_buy_sell 
                    set buy = @buy, sell = @sell, unixTimestamp = @unixTimestamp
                    where investrueId = @investrueId and date = @date;                    
                ";
                string insertSql = @"
                    INSERT INTO stock_info.trust_buy_sell (investrueId, date, buy, sell, unixTimestamp) 
                    VALUES (@investrueId, @date, @buy, @sell, @unixTimestamp);             
                ";

                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    using (var sqlConnection = new MySqlConnection(_connectStringStockInfo))
                    {
                        await sqlConnection.OpenAsync();
                        foreach (var dto in dtos)
                        {
                            var find = checkList.Find((item) => { return dto.investrueId == item.investrueId && dto.date == item.date; });
                            if (find != null)
                            {
                                await sqlConnection.ExecuteAsync(updateSql, dto);
                                Console.WriteLine($"更新資料 trust_buy_sell {dto.date} {dto.investrueId} ");
                            }
                            else
                            {
                                await sqlConnection.ExecuteAsync(insertSql, dto);
                                Console.WriteLine($"新增資料 trust_buy_sell {dto.date} {dto.investrueId} ");
                            }

                        }
                    }
                    scope.Complete();
                }
                return dtos;
            }
            catch (Exception e)
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                throw;
            }
        }

        public async Task<List<VolumeDataDto>> UpdateListVolumeData(List<VolumeDataDto> dtos)
        {
            try
            {  
                
                string updateSql = @"
                    UPDATE stock_info.quote_info 
                    SET buyAmount = @buyAmount, sellAmount = @sellAmount, sharesVolume = @sharesVolume,
                        amplitude = @amplitude, fluctuation = @fluctuation, sharesRate = @sharesRate
                    WHERE id = @stockNo and date = @theDate;                    
                ";

                string tradeDaySql = @"
                    SELECT * FROM stock_info.tw_trade_day WHERE date <= @theDate ORDER BY date DESC LIMIT 2;";

                string queryQuoteInfoSql = @"
                    SELECT * FROM stock_info.quote_info 
                    WHERE date <= @dayOne AND date >= @dayTwo
                    ORDER BY date DESC";
                
               
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    using (var sqlConnection = new MySqlConnection(_connectStringStockInfo))
                    {
                        await sqlConnection.OpenAsync();
                        string date = dtos[0].theDate;
                        var twTradeDay = (await sqlConnection.QueryAsync<TwTradeDayDto>(tradeDaySql, new { theDate = dtos[0].theDate })).ToList();
                        IEnumerable<QuoteInfoDto> yesterdayQuoteInfoList = await sqlConnection.QueryAsync<QuoteInfoDto>(queryQuoteInfoSql, new { dayOne = twTradeDay[0].date, dayTwo = twTradeDay[1].date });
                        Dictionary<string, List<QuoteInfoDto>> stockDictionary = new Dictionary<string, List<QuoteInfoDto>>() { };
                        List<QuoteInfoDto> dicResult;
                        foreach (var dto in yesterdayQuoteInfoList) 
                        {
                            if (stockDictionary.TryGetValue(dto.id, out dicResult)) 
                            {
                                //dicResult.Add(dto);
                                //stockDictionary.Remove(dto.id);
                                dicResult.Add(dto);
                                //stockDictionary[dto.id].Add(dto);
                            }
                            else 
                            {
                                List<QuoteInfoDto> temp = new List<QuoteInfoDto>();
                                temp.Add(dto);
                                stockDictionary.Add(dto.id, temp);
                            }                                
                        }
                        
                        foreach (var dto in dtos)
                        {                            
                            if (stockDictionary.TryGetValue(dto.stockNo, out dicResult)) 
                            {
                                if (dicResult.Count() == 2)
                                {
                                    var today = stockDictionary[dto.stockNo][0];
                                    var yesterday = stockDictionary[dto.stockNo][1];
                                    var fluctuation = (today.close - yesterday.close) / yesterday.close * 100f;
                                    var amplitude = (today.high - today.low) / yesterday.close * 100f;
                                    float sharesRate = 0f;
                                    if (dto.sharesVolume != 0 && today.volume != 0)
                                    {
                                        sharesRate = (float)dto.sharesVolume / (float)today.volume * 100f;
                                    }
                                    dto.fluctuation = fluctuation;
                                    dto.sharesRate = sharesRate;
                                    dto.amplitude = amplitude;
                                    await sqlConnection.ExecuteAsync(updateSql, dto);
                                   /* if (dto.stockNo == "3211") 
                                    {
                                        Console.WriteLine("fluctuation" + fluctuation);
                                        Console.WriteLine("sharesRate" + sharesRate);
                                        Console.WriteLine("amplitude" + amplitude);
                                        Console.WriteLine(JsonConvert.SerializeObject(dto));
                                        Console.WriteLine("today");
                                        Console.WriteLine(JsonConvert.SerializeObject(today));
                                        Console.WriteLine("yesterday");
                                        Console.WriteLine(JsonConvert.SerializeObject(yesterday));
                                    }*/
                                }
                                else 
                                {
                                    await sqlConnection.ExecuteAsync(updateSql, dto);
                                }
                            }


                        }
                    }
                    scope.Complete();
                }
                return dtos;
            }
            catch (Exception e)
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                throw;
            }
        }

        public async Task<string> UpdateUnitTimestamp(string date) 
        {
            try 
            {
                int year = int.Parse(date.Substring(0, 4));
                int month = int.Parse(date.Substring(4, 2));
                int day = int.Parse(date.Substring(6, 2));
                if (year < 1970 || month > 12 || day > 31)
                {
                    return "日期格式錯誤 西元年月日 ex 20190123:";
                }
                var tagetDay = new DateTime(year, month, day, 16, 0, 0, DateTimeKind.Utc).AddDays(-1);
                long unixTimestamp = (long)(tagetDay - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;

                string updateSql = @"
                    update stock_info.quote_info 
                    set unixTimestamp = @unixTimestamp
                    where id = @id and date = @date;                     
                ";

                List<QuoteInfoDto> quoteChecks = (await QueryQuoteInfoByDate(date)).ToList();
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    using (var sqlConnection = new MySqlConnection(_connectStringStockInfo))
                    {
                        await sqlConnection.OpenAsync();
                        foreach (var dto in quoteChecks)
                        {
                            dto.unixTimestamp = unixTimestamp;
                            Console.WriteLine(JsonConvert.SerializeObject(dto));
                            await sqlConnection.ExecuteAsync(updateSql, dto);
                        }
                    }
                    scope.Complete();
                }

                Console.WriteLine("已更新 QueryQuoteInfoByDate");
                Console.WriteLine(JsonConvert.SerializeObject(quoteChecks));

                updateSql = @"
                    update stock_info.dealer_buy_sell 
                    set unixTimestamp = @unixTimestamp
                    where investrueId = @investrueId and date = @date;                     
                ";

                List<DealerBuySellDto> dealerChecks = (await QueryDealerBuySellByDate(date)).ToList();
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    using (var sqlConnection = new MySqlConnection(_connectStringStockInfo))
                    {
                        await sqlConnection.OpenAsync();
                        foreach (var dto in dealerChecks)
                        {
                            dto.unixTimestamp = unixTimestamp;
                            await sqlConnection.ExecuteAsync(updateSql, dto);
                        }
                    }
                    scope.Complete();
                }

                updateSql = @"
                    update stock_info.foreign_buy_sell 
                    set unixTimestamp = @unixTimestamp
                    where investrueId = @investrueId and date = @date;                     
                ";

                List<ForeignBuySellDto> foreignChecks = (await QueryForeignBuySellByDate(date)).ToList();
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    using (var sqlConnection = new MySqlConnection(_connectStringStockInfo))
                    {
                        await sqlConnection.OpenAsync();
                        foreach (var dto in foreignChecks)
                        {
                            dto.unixTimestamp = unixTimestamp;
                            await sqlConnection.ExecuteAsync(updateSql, dto);
                        }
                    }
                    scope.Complete();
                }

                updateSql = @"
                    update stock_info.trust_buy_sell 
                    set unixTimestamp = @unixTimestamp
                    where investrueId = @investrueId and date = @date;                     
                ";

                List<TrustBuySellDto> trustChecks = (await QueryTrustBuySellByDate(date)).ToList();
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    using (var sqlConnection = new MySqlConnection(_connectStringStockInfo))
                    {
                        await sqlConnection.OpenAsync();
                        foreach (var dto in trustChecks)
                        {
                            dto.unixTimestamp = unixTimestamp;
                            await sqlConnection.ExecuteAsync(updateSql, dto);
                        }
                    }
                    scope.Complete();
                }

                return unixTimestamp.ToString();
            }
            catch (Exception e) 
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                throw;
            }
        }

    }

}
