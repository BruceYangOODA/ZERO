using ZERO.Database.StockInfo;
using ZERO.Sevice.IService;
using ZERO.Repository.IRepository;
using ZERO.Models;
using ZERO.Util;
using System.Transactions;
using ZERO.Models.Dto.StockInfo;
using ZERO.Models.Enum;
using System.Data.SqlTypes;

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
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                return null;// new OperationResult<List<QuoteInfo>>();
            }

        }

        public async Task<OperationResult<IEnumerable<QuoteInfoDto>>> GetAllQuoteInfo() 
        {
            try 
            {
                OperationResult<IEnumerable<QuoteInfoDto>> operationResult = new();
                IEnumerable<QuoteInfoDto> result = await _stockRepository.GetAllQuoteInfo();
                IEnumerable<QuoteInfoDto> result2 = await _stockRepository.PostListQuoteInfo(result.ToList());

                operationResult.Result = result2;
                operationResult.RequestResultCode = RequestResultCode.Success;
                //using var scope = new TransactionScope();
                //var gg = operationResult.Result.First();
                //await _stockRepository.InsertAsync(gg);
                //scope.Complete();
                return operationResult;
            }
            catch (Exception e) 
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                return null;// new OperationResult<List<QuoteInfo>>();
            }
            
        }

        public async Task<OperationResult<List<QuoteInfoDto>>> PostListQuoteInfo(List<QuoteInfoDto> quoteInfos) 
        {
            try 
            {
                OperationResult<List<QuoteInfoDto>> operationResult = new ();                
                string theDate = quoteInfos.First().date;        
                IEnumerable<QuoteInfoDto> quoteChecks = await _stockRepository.GetQuoteInfoByDate(theDate);
                operationResult.Result = quoteChecks.ToList();
                return operationResult;
            }
            catch (Exception e) 
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                return null;// new OperationResult<List<QuoteInfo>>();
            }
        }
    }
}
