using ZERO.Database.StockInfo;
using ZERO.Sevice.IService;
using ZERO.Repository.IRepository;
using ZERO.Models;
using ZERO.Util;
using System.Transactions;
using ZERO.Models.Dto.StockInfo;
using ZERO.Models.Enum;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ZERO.Sevice
{
    public class DayTradingService : IDayTradingService
    {
        private readonly ILogger<DayTradingService> _logger;
        private readonly IDayTradingRepository _dayTradingRepository;

        public DayTradingService(ILogger<DayTradingService> logger, IDayTradingRepository dayTradingRepository)
        {
            _logger = logger;
            _dayTradingRepository = dayTradingRepository;
        }

        public async Task<OperationResult<List<FiveQuoteInfoDto>>> GetFiveDayQuoteInfo(string date) 
        {
            try 
            {
                OperationResult<List<FiveQuoteInfoDto>> operationResult = new();
                IEnumerable<QuoteInfoDto> temp = await _dayTradingRepository.QueryFiveDayQuoteInfo(date);
                List<FiveQuoteInfoDto> result = new();
                string flag = "";
                List<QuoteInfoDto>? fiveDayList = null;                
                foreach (var item in temp) 
                {                    
                    if (flag != item.id)
                    {
                        if (fiveDayList == null)
                        {
                            fiveDayList = new List<QuoteInfoDto>();
                            flag = item.id;
                            fiveDayList.Add(item);
                        }
                        else
                        {
                            if (fiveDayList.Count == 0) 
                            {
                                // 空資料                                
                                flag = item.id;
                                continue;
                            }
                            FiveQuoteInfoDto dto = new FiveQuoteInfoDto();
                            bool isBurstVolume = false;
                            dto.id = "" + flag;
                            dto.close = fiveDayList.First().close;
                            dto.open = fiveDayList.Last().open;
                            dto.high = fiveDayList.Max(e => e.high);
                            dto.low = fiveDayList.Min(e => e.low);
                            dto.fiveDayVolume = fiveDayList.Sum(e => e.volume);
                            dto.fiveDayList = fiveDayList;

                            var avgVolume = dto.fiveDayVolume / fiveDayList.Count() * 1.8;
                            foreach (var dayInfo in fiveDayList) 
                            {
                                if (dayInfo.volume >= avgVolume) 
                                {
                                    isBurstVolume = true;
                                }
                            }
                            // 是否爆5日平均量 1.8x 爆量平均量低於 5000 不採計
                            dto.isBurstVolume = isBurstVolume;
                            if (avgVolume > 5000) result.Add(dto);

                            // 歸零
                            fiveDayList = new List<QuoteInfoDto>();
                            fiveDayList.Add(item);
                            flag = item.id;
                        }

                    }
                    else 
                    {
                        fiveDayList.Add(item);
                    }

                }
                int count = 0;
                foreach (var item in result)
                {
                 
                    if (item.isBurstVolume) 
                    {
                        count++;
                        Console.WriteLine(item.id+ " *=* "+ count);                        
                        
                    }
                }
                operationResult.Result = result;
                operationResult.RequestResultCode = RequestResultCode.Success;
                return operationResult;
            }
            catch(Exception e) 
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                throw;
            }
        }
    }
}
