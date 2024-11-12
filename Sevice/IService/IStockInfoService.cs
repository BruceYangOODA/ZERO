using ZERO.Database.StockInfo;
using ZERO.Models;
using ZERO.Models.Dto.StockInfo;

namespace ZERO.Sevice.IService
{
    public interface IStockInfoService
    {
        /// <summary> 登入並取得使用者資料 </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public Task<OperationResult<string>> Scraper(string cookie, string signature, int differDays);
        public Task<OperationResult<IEnumerable<QuoteInfoDto>>> GetAllQuoteInfo();
        public Task<string> PostListQuoteInfo(List<QuoteInfoDto> dtos);
        public Task<string> PostListForeinBuySell(List<ForeignBuySellDto> dtos);
        public Task<string> PostListTrustBuySell(List<TrustBuySellDto> dtos);
        public Task<string> PostListVolumeData(List<VolumeDataDto> dtos);
    }
}
