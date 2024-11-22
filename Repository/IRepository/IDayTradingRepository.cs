using ZERO.Models.Dto.StockInfo;

namespace ZERO.Repository.IRepository
{
    public interface IDayTradingRepository
    {
        public Task<IEnumerable<QuoteInfoDto>> QueryFiveDayQuoteInfo(string date);
    }
}
