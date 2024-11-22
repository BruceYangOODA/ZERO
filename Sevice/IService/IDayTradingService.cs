using ZERO.Models.Dto.StockInfo;
using ZERO.Models;

namespace ZERO.Sevice.IService
{
    public interface IDayTradingService
    {
        public Task<OperationResult<List<FiveQuoteInfoDto>>> GetFiveDayQuoteInfo(string date);
    }
}
