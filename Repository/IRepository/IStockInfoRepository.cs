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
        public Task<List<QuoteInfoDto>> UpdateListQuoteInfo(List<QuoteInfoDto> stockInfos);
        public Task<IEnumerable<QuoteInfoDto>> QueryAllQuoteInfo();
        public Task<IEnumerable<QuoteInfoDto>> QueryQuoteInfoByDate(string date);
        public Task<QuoteInfoDto> InsertQuoteInfo(QuoteInfoDto qid);
        public Task<QuoteInfoDto> UpdateQuoteInfo(QuoteInfoDto qid);        

        public Task<List<ForeignBuySellDto>> UpdateListForeignBuySell(List<ForeignBuySellDto> fbsdtos);
        public Task<List<DealerBuySellDto>> UpdateListDealerBuySell(List<DealerBuySellDto> fbsdtos);

    }
}
