using ZERO.Database.StockInfo;
using ZERO.Sevice.IService;
using ZERO.Repository.IRepository;
using ZERO.Models;
using ZERO.Util;
using System.Transactions;
using ZERO.Models.Dto.StockInfo;
using ZERO.Models.Enum;
using System.Data.SqlTypes;
using Newtonsoft.Json;
using MySql.Data.MySqlClient;
using System.Diagnostics.Eventing.Reader;

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
        public async Task<OperationResult<string>> ScraperOne(string cookie, string signature, int differDays)
        {
            try
            {
                // https://www.wantgoo.com/stock/institutional-investors/foreign/net-buy-sell-rank
                string resultStr = "";
                OperationResult<string> operationResult = new();                
                operationResult.Result = "";            

                UtilScraper utilScraper = new UtilScraper(cookie, signature, differDays);
                
                OperationResult<List<QuoteInfoDto>> quoteInfoResult = await utilScraper.GetHistoricalAllQuoteInfo();
                if (quoteInfoResult.RequestResultCode == RequestResultCode.Success) 
                {

                    operationResult.Result = operationResult.Result  + "已取得 QuoteInfo " + "\n";                    
                    resultStr = await PostListQuoteInfo(quoteInfoResult.Result);
                    operationResult.Result = operationResult.Result  + resultStr + "\n";                    
                }
                else 
                {
                    operationResult.RequestResultCode = RequestResultCode.Failed;
                    operationResult.ErrorMessage = quoteInfoResult.ErrorMessage;
                    return operationResult;
                }


                OperationResult<List<ForeignBuySellDto>> foreignBuySellResult = await utilScraper.GetAllForeignBuySell();
                if (foreignBuySellResult.RequestResultCode == RequestResultCode.Success)
                {
                    operationResult.Result = operationResult.Result + "已取得 ForeignBuySell " + "\n";
                    resultStr = await PostListForeinBuySell(foreignBuySellResult.Result);
                    operationResult.Result = operationResult.Result + resultStr + "\n";
                }
                else
                {
                    operationResult.RequestResultCode = RequestResultCode.Failed;
                    operationResult.ErrorMessage = foreignBuySellResult.ErrorMessage;
                    return operationResult;
                }

                OperationResult<List<TrustBuySellDto>> trustBuySellResult = await utilScraper.GetAllTrustBuySell();
                if (trustBuySellResult.RequestResultCode == RequestResultCode.Success)
                {
                    operationResult.Result = operationResult.Result + "已取得 TrustBuySell " + "\n";
                    resultStr = await PostListTrustBuySell(trustBuySellResult.Result);
                    operationResult.Result = operationResult.Result + resultStr + "\n";
                }
                else
                {
                    operationResult.RequestResultCode = RequestResultCode.Failed;
                    operationResult.ErrorMessage = trustBuySellResult.ErrorMessage;
                    return operationResult;
                }

                /*
                OperationResult<List<DealerBuySellDto>> dealerBuySellResult = await utilScraper.GetAllDealerBuySell();
                if (dealerBuySellResult.RequestResultCode == RequestResultCode.Success)
                {
                    operationResult.Result = operationResult.Result + "已取得 DealerBuySell " + "\n";
                    resultStr = await PostListDealerBuySell(dealerBuySellResult.Result);
                    operationResult.Result = operationResult.Result + resultStr + "\n";
                }
                else
                {
                    operationResult.RequestResultCode = RequestResultCode.Failed;
                    operationResult.ErrorMessage = dealerBuySellResult.ErrorMessage;
                    return operationResult;
                }                  
             

              OperationResult<List<VolumeDataDto>> volumeDataResult = await utilScraper.GetAllVolumeData();
              if (volumeDataResult.RequestResultCode == RequestResultCode.Success)
              {
                  operationResult.Result = operationResult.Result + "已取得 VolumeData " + "\n";
                  resultStr = await PostListVolumeData(volumeDataResult.Result);
                  operationResult.Result = operationResult.Result + resultStr + "\n";
              }
              else
              {
                  operationResult.RequestResultCode = RequestResultCode.Failed;
                  operationResult.ErrorMessage = volumeDataResult.ErrorMessage;
                  return operationResult;
              }*/

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
        public async Task<OperationResult<string>> ScraperTwo(string cookie, string signature, int differDays)
        {
            try
            {
                // https://www.wantgoo.com/stock/institutional-investors/foreign/net-buy-sell-rank
                string resultStr = "";
                OperationResult<string> operationResult = new();
                operationResult.Result = "";

                UtilScraper utilScraper = new UtilScraper(cookie, signature, differDays);                               

                OperationResult<List<DealerBuySellDto>> dealerBuySellResult = await utilScraper.GetAllDealerBuySell();
                if (dealerBuySellResult.RequestResultCode == RequestResultCode.Success)
                {
                    operationResult.Result = operationResult.Result + "已取得 DealerBuySell " + "\n";
                    resultStr = await PostListDealerBuySell(dealerBuySellResult.Result);
                    operationResult.Result = operationResult.Result + resultStr + "\n";
                }
                else
                {
                    operationResult.RequestResultCode = RequestResultCode.Failed;
                    operationResult.ErrorMessage = dealerBuySellResult.ErrorMessage;
                    return operationResult;
                }
      
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
        
        public async Task<OperationResult<string>> ScraperThree(string cookie, string signature, int differDays)
        {
            try
            {
                // https://www.wantgoo.com/stock/institutional-investors/foreign/net-buy-sell-rank
                string resultStr = "";
                OperationResult<string> operationResult = new();
                operationResult.Result = "";

                UtilScraper utilScraper = new UtilScraper(cookie, signature, differDays);    

                OperationResult<List<VolumeDataDto>> volumeDataResult = await utilScraper.GetAllVolumeData();
                if (volumeDataResult.RequestResultCode == RequestResultCode.Success)
                {
                    operationResult.Result = operationResult.Result + "已取得 VolumeData " + "\n";
                    resultStr = await PostListVolumeData(volumeDataResult.Result);
                    operationResult.Result = operationResult.Result + resultStr + "\n";
                }
                else
                {
                    operationResult.RequestResultCode = RequestResultCode.Failed;
                    operationResult.ErrorMessage = volumeDataResult.ErrorMessage;
                    return operationResult;
                }

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
                IEnumerable<QuoteInfoDto> result = await _stockRepository.QueryAllQuoteInfo();                

                operationResult.Result = result;
                operationResult.RequestResultCode = RequestResultCode.Success;                
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

        public async Task<string> PostListQuoteInfo(List<QuoteInfoDto> dtos) 
        {
            try 
            {
                var result = await _stockRepository.UpdateListQuoteInfo(dtos);
                
                return $"已更新 QuoteInfo {dtos.First().date}";
               
            }
            catch (Exception e) 
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                return null;
            }
        }
        public async Task<string> PostListForeinBuySell(List<ForeignBuySellDto> dtos)
        {
            try
            {
                var result = await _stockRepository.UpdateListForeignBuySell(dtos);

                return $"已更新 ForeignBuySell {dtos.First().date}";

            }
            catch (Exception e)
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                return null;
            }
        }
        public async Task<string> PostListDealerBuySell(List<DealerBuySellDto> dtos)
        {
            try
            {
                var result = await _stockRepository.UpdateListDealerBuySell(dtos);

                return $"已更新 DealerBuySell {dtos.First().date}";

            }
            catch (Exception e)
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                return null;
            }
        }
        public async Task<string> PostListTrustBuySell(List<TrustBuySellDto> dtos)
        {
            try
            {
                var result = await _stockRepository.UpdateListTrustBuySell(dtos);

                return $"已更新 TrustBuySell {dtos.First().date}";

            }
            catch (Exception e)
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                return null;
            }
        }
        public async Task<string> PostListVolumeData(List<VolumeDataDto> dtos)
        {
            try
            {
                var result = await _stockRepository.UpdateListVolumeData(dtos);

                return $"已更新 VolumeData ";

            }
            catch (Exception e)
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                return null;
            }
        }
    }
}
