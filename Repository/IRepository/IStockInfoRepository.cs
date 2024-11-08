using ZERO.Database.StockInfo;
using ZERO.Models;

namespace ZERO.Repository.IRepository
{
    public interface IStockInfoRepository
    {
        /// <summary> 存入資料 </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public Task<OperationResult<List<QuoteInfo>>> Add(List<QuoteInfo> stockInfos);
        public Task<OperationResult<List<QuoteInfo>>> GetAllQuoteInfo();
    }
}
