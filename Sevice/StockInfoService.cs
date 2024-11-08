using ZERO.Database.StockInfo;
using ZERO.Sevice.IService;
using ZERO.Repository.IRepository;
using ZERO.Models;
using ZERO.Util;

namespace ZERO.Sevice
{
    public class StockInfoService : IStockInfoService
    {
        private readonly ILogger<StockInfoService> _logger;
        private readonly IStockInfoRepository _stockRepository;

        public StockInfoService(ILogger<StockInfoService> logger, IStockInfoRepository stockRepository)
        {
            _logger = logger;
            _stockRepository = stockRepository;
        }
        /// <summary> 爬蟲 </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public async Task<OperationResult<List<QuoteInfo>>> Scraper(string cookie, string signature, int differDays)
        {
            try
            {
                // OperationResult<List<QuoteInfo>> operationResult = new();
                // operationResult.Result = _stockRepository.GetAllQuoteInfo();
                // return operationResult;

                HttpClient _httpClient = new HttpClient();
                UtilScraper utilScraper = new UtilScraper(cookie, signature, differDays);
                OperationResult<List<QuoteInfo>> operationResult = await utilScraper.GetHistoricalAllQuoteInfo();
                return operationResult;

            }
            catch (Exception e)
            {
                _logger.LogError($"StockService錯誤，StackTrace : {e.StackTrace}");
                // _logger.LogError($"StockService錯誤，傳入參數，UserLoginPara : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"StockService錯誤，呼叫函式 : Scraper，錯誤訊息：{e.Message}");
                return null;// new OperationResult<List<QuoteInfo>>();
            }

        }

        public async Task<OperationResult<List<QuoteInfo>>> GetAllQuoteInfo() 
        {
            OperationResult<List<QuoteInfo>> operationResult = await _stockRepository.GetAllQuoteInfo();
            return operationResult;
        }
    }
}
