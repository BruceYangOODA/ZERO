using ZERO.Database.StockInfo;
using ZERO.Database;
using ZERO.Models.Enum;
using ZERO.Models;
using ZERO.Repository.IRepository;
using System.Collections.Generic;
using System.Transactions;
using System.Linq.Expressions;

namespace ZERO.Repository
{
    public class StockInfoRepository : BaseRepository<StockInfoRepository>, IStockInfoRepository
    {
        //private readonly ILogger<StockInfoRepository> _logger;
        protected readonly StockInfoContext _context;
        public StockInfoRepository(IConfiguration configuration, ILogger<StockInfoRepository> logger, StockInfoContext context) : base(configuration, logger, context)
        {
            _context = context;
        }
        /// <summary> 存入資料 </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public async Task<OperationResult<List<QuoteInfo>>> Add(List<QuoteInfo> quoteInfos)
        {
            OperationResult<List<QuoteInfo>> operationResult = new();
            operationResult.RequestResultCode = RequestResultCode.Success;
            operationResult.Result = quoteInfos;            
            return operationResult;
        }
        public async Task<OperationResult<List<QuoteInfo>>> GetAllQuoteInfo()
        {    
            OperationResult<List<QuoteInfo>> operationResult = new();
            operationResult.Result = _context.QuoteInfos.ToList();
            return operationResult;            
        }
    }
}
