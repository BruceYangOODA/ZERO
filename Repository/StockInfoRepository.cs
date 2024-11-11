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
        public async Task<IEnumerable<QuoteInfoDto>> PostListQuoteInfo(List<QuoteInfoDto> quoteInfos)
        {
            try 
            {                
                string theDate = quoteInfos.First().date;    
                IEnumerable<QuoteInfoDto> checkList = new List<QuoteInfoDto>();
                List<QuoteInfoDto> result = new();
                string sql = @"
                    select * from stock_info.quote_info qi
                    where qi.date = @theDate;
                ";
                using (var sqlConnection = new MySqlConnection(_connectStringStockInfo)) 
                {
                    sqlConnection.Open();
                    var temp = await sqlConnection.QueryAsync<QuoteInfo>(sql, new { theDate = theDate });
                    result = temp.ToList().ConvertAll<QuoteInfoDto>(q => new QuoteInfoDto(q));                    
                }
                result = result.FindAll(e => e.id != "999999");         
                return result;
            }
            catch(Exception e) 
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                return null;// new OperationResult<List<QuoteInfo>>();
            }
            /*OperationResult<List<QuoteInfo>> operationResult = new();
            operationResult.RequestResultCode = RequestResultCode.Success;
            operationResult.Result = quoteInfos;            
            return operationResult;*/
        }
        public async Task<IEnumerable<QuoteInfoDto>> GetAllQuoteInfo()
        {   
            IEnumerable<QuoteInfoDto> result = new List<QuoteInfoDto>();
            result = _context.QuoteInfos.ToList().ConvertAll<QuoteInfoDto>(q => new QuoteInfoDto(q));
            return result;            
        }
        public async Task<IEnumerable<QuoteInfoDto>> GetQuoteInfoByDate(string theDate)
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
                    sqlConnection.Open();
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
                return null;// new OperationResult<List<QuoteInfo>>();
            }
        }
    }
}
