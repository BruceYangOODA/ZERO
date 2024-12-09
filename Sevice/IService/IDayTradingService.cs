using ZERO.Models.Dto.StockInfo;
using ZERO.Models;

namespace ZERO.Sevice.IService
{
    public interface IDayTradingService
    {
        public Task<IEnumerable<QuoteInfoDto>> GetQuoteInfo(string date, int ma);
        public Task<OperationResult<IEnumerable<QuoteInfoDto>>> GetQuoteInfoById(string date, int ma, string id);
        public Task<OperationResult<List<FiveQuoteInfoDto>>> QueryMA5QuoteInfo(string date, int ma);
        public Task<OperationResult<List<FiveQuoteInfoDto>>> GetDayTradeRef(string date, int ma);
        public Task<string> test();
       // public Task<OperationResult<List<QuoteInfoDto>>> GetMA20QuoteInfo(string date);
    }
}
