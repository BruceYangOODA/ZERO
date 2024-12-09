using ZERO.Models.Dto.StockInfo;

namespace ZERO.Repository.IRepository
{
    public interface IDayTradingRepository
    {

        public Task<IEnumerable<TwTradeDayDto>> QueryUnixTime(string theDate, int ma);
        public Task<IEnumerable<QuoteInfoDto>> QueryQuoteInfo(IEnumerable<TwTradeDayDto> dayDtos);
        public Task<IEnumerable<QuoteInfoDto>> QueryQuoteInfoById(IEnumerable<TwTradeDayDto> dayDtos, string id);
        public Task<string> test();
        // public Task<IEnumerable<QuoteInfoDto>> QueryMA5QuoteInfo(IEnumerable<TwTradeDayDto> dayDtos);
    }
}
