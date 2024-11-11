using ZERO.Database.StockInfo;
using ZERO.Models;
using ZERO.Models.Dto.StockInfo;

namespace ZERO.Repository.IRepository
{
    public interface IStockInfoRepository
    {
        /// <summary> 存入資料 </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public Task<IEnumerable<QuoteInfoDto>> PostListQuoteInfo(List<QuoteInfoDto> stockInfos);
        public Task<IEnumerable<QuoteInfoDto>> GetAllQuoteInfo();
        public Task<IEnumerable<QuoteInfoDto>> GetQuoteInfoByDate(string date);
    }
}
