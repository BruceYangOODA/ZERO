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
using static Dapper.SqlMapper;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.VisualBasic;
using System.Security.Cryptography;

namespace ZERO.Repository
{
    public class StockInfoRepository : BaseRepository<StockInfoRepository>, IStockInfoRepository
    {
        private readonly ILogger<StockInfoRepository> _logger;
        protected readonly StockInfoContext _context;
        public StockInfoRepository(IConfiguration configuration, ILogger<StockInfoRepository> logger, StockInfoContext context) : base(configuration, logger, context)
        {
            _context = context;
            _logger = logger;            
        }
        /// <summary> 存入資料 </summary>
        /// <param name="para"></param>
        /// <returns></returns>        
        public async Task<IEnumerable<QuoteInfoDto>> QueryAllQuoteInfo()
        {   
            IEnumerable<QuoteInfoDto> result = new List<QuoteInfoDto>();
            result = _context.QuoteInfos.ToList().ConvertAll<QuoteInfoDto>(q => new QuoteInfoDto(q));
            return result;            
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
                    var temp = await sqlConnection.QueryAsync<QuoteInfo>(sql, new { theDate = theDate });
                    result = temp.ToList().ConvertAll<QuoteInfoDto>(q => new QuoteInfoDto(q));
                }                
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                return null;
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
                    var temp = await sqlConnection.QueryAsync<ForeignBuySell>(sql, new { theDate = theDate });
                    result = temp.ToList().ConvertAll<ForeignBuySellDto>(q => new ForeignBuySellDto(q));
                }
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                return null;
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
                    var temp = await sqlConnection.QueryAsync<DealerBuySell>(sql, new { theDate = theDate });
                    result = temp.ToList().ConvertAll<DealerBuySellDto>(q => new DealerBuySellDto(q));
                }
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                return null;
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
                    var temp = await sqlConnection.QueryAsync<TrustBuySell>(sql, new { theDate = theDate });
                    result = temp.ToList().ConvertAll<TrustBuySellDto>(q => new TrustBuySellDto(q));
                }
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                return null;
            }
        }

        public async Task<QuoteInfoDto> InsertQuoteInfo(QuoteInfoDto qid) 
        {
            try 
            {
                QuoteInfoDto result;
                string sql = @"
                    INSERT INTO stock_info.quote_info (id, date, open, high, low, close, millionAmount, volume) 
                    VALUES (@id, @date, @open, @high, @low, @close, @millionAmount, @volume);             
                ";
                using (var sqlConnection = new MySqlConnection(_connectStringStockInfo))
                {
                    await sqlConnection.OpenAsync();
                    var temp = await sqlConnection.QueryAsync<QuoteInfo>(sql, qid);
                    result = temp.ToList().ConvertAll<QuoteInfoDto>(q => new QuoteInfoDto(q)).First();
                }
                return result;
            }
            catch (Exception e) 
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                return null;
            }
        }
        public async Task<QuoteInfoDto> UpdateQuoteInfo(QuoteInfoDto qid) 
        {
            try 
            {
                QuoteInfoDto result;
                string sql = @"
                    update stock_info.quote_info 
                    set open = @open, high = @high, low = @low, close = @close, volume = @volume, millionAmount = @millionAmount
                    where id = @id and date = @date;                    
                ";
              
                using (var sqlConnection = new MySqlConnection(_connectStringStockInfo))
                {
                    await sqlConnection.OpenAsync();                    
                    var temp = await sqlConnection.ExecuteReaderAsync(sql, qid);
                    Console.WriteLine("temp" + JsonConvert.SerializeObject(temp));
                  
                    return null;
                   // result = temp.ToList().ConvertAll<QuoteInfoDto>(q => new QuoteInfoDto(q)).First();
                }
                return result;
            }
            catch (Exception e) 
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                return null;
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
                    set open = @open, high = @high, low = @low, close = @close, volume = @volume, millionAmount = @millionAmount
                    where id = @id and date = @date;                    
                ";
                string insertSql = @"
                    INSERT INTO stock_info.quote_info (id, date, open, high, low, close, millionAmount, volume) 
                    VALUES (@id, @date, @open, @high, @low, @close, @millionAmount, @volume);             
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
                                await sqlConnection.ExecuteAsync(updateSql, find);
                            }
                            else
                            {
                                await sqlConnection.ExecuteAsync(insertSql, dto);
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
                return null;
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
                    set buy = @buy, sell = @sell
                    where investrueId = @investrueId and date = @date;                    
                ";
                string insertSql = @"
                    INSERT INTO stock_info.foreign_buy_sell (investrueId, date, buy, sell) 
                    VALUES (@investrueId, @date, @buy, @sell);             
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
                                await sqlConnection.ExecuteAsync(updateSql, find);
                            }
                            else
                            {
                                await sqlConnection.ExecuteAsync(insertSql, dto);
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
                return null;
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
                    set buy = @buy, sell = @sell
                    where investrueId = @investrueId and date = @date;                    
                ";
                string insertSql = @"
                    INSERT INTO stock_info.dealer_buy_sell (investrueId, date, buy, sell) 
                    VALUES (@investrueId, @date, @buy, @sell);             
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
                                await sqlConnection.ExecuteAsync(updateSql, find);
                            }
                            else
                            {
                                await sqlConnection.ExecuteAsync(insertSql, dto);
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
                return null;
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
                    set buy = @buy, sell = @sell
                    where investrueId = @investrueId and date = @date;                    
                ";
                string insertSql = @"
                    INSERT INTO stock_info.trust_buy_sell (investrueId, date, buy, sell) 
                    VALUES (@investrueId, @date, @buy, @sell);             
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
                                await sqlConnection.ExecuteAsync(updateSql, find);
                            }
                            else
                            {
                                await sqlConnection.ExecuteAsync(insertSql, dto);
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
                return null;
            }
        }

        public async Task<List<VolumeDataDto>> UpdateListVolumeData(List<VolumeDataDto> dtos)
        {
            try
            {                   
                string updateSql = @"
                    update stock_info.quote_info 
                    set buyAmount = @buyAmount, sellAmount = @sellAmount, sharesVolume = @sharesVolume
                    where id = @stockNo and date = @theDate;                    
                ";
               
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    using (var sqlConnection = new MySqlConnection(_connectStringStockInfo))
                    {
                        await sqlConnection.OpenAsync();
                        foreach (var dto in dtos)
                        {        
                            await sqlConnection.ExecuteAsync(updateSql, dto);
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
                return null;
            }
        }

    }

}
