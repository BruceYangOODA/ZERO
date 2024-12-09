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
using ZERO.Repository;
using System.Security.Cryptography.Xml;
using NLog.Time;

namespace ZERO.Sevice
{
    public class DayTradingService : IDayTradingService
    {
        private readonly ILogger<DayTradingService> _logger;
        private readonly IDayTradingRepository _dayTradingRepository;
        private readonly IStockInfoRepository _stockInfoRepository;

        public DayTradingService(ILogger<DayTradingService> logger, IDayTradingRepository dayTradingRepository, IStockInfoRepository stockInfoRepository)
        {
            _logger = logger;
            _dayTradingRepository = dayTradingRepository;
            _stockInfoRepository = stockInfoRepository;
        }

        public async Task<IEnumerable<QuoteInfoDto>> GetQuoteInfo(string date, int ma) 
        {
            try 
            {
                IEnumerable<TwTradeDayDto> dayDtos = await _dayTradingRepository.QueryUnixTime(date, ma);
                IEnumerable<QuoteInfoDto> result = await _dayTradingRepository.QueryQuoteInfo(dayDtos);
                return result;
            }
            catch (Exception e) 
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                throw;
            }
        }
        public async Task<OperationResult<IEnumerable<QuoteInfoDto>>> GetQuoteInfoById(string date, int ma, string id)
        {
            try
            {
                OperationResult<IEnumerable<QuoteInfoDto>> operationResult = new();
                IEnumerable<TwTradeDayDto> dayDtos = await _dayTradingRepository.QueryUnixTime(date, ma);
                IEnumerable<QuoteInfoDto> result = await _dayTradingRepository.QueryQuoteInfoById(dayDtos, id);
                operationResult.Result = result;
                operationResult.RequestResultCode = RequestResultCode.Success;
                return operationResult;
            }
            catch (Exception e)
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                throw;
            }
        }

        public async Task<OperationResult<List<FiveQuoteInfoDto>>> QueryMA5QuoteInfo(string date, int ma)
        {
            try
            {              
                OperationResult<List<FiveQuoteInfoDto>> operationResult = new();
                IEnumerable<QuoteInfoDto> temp = await GetQuoteInfo(date, ma);
                
                return operationResult;
            }
            catch (Exception e)
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                throw;
            }
        }

        public async Task<OperationResult<List<FiveQuoteInfoDto>>> GetDayTradeRef(string date, int ma)
        {
            try 
            {
                OperationResult<List<FiveQuoteInfoDto>> operationResult = new();
                IEnumerable<TwTradeDayDto> dayDtos = await _dayTradingRepository.QueryUnixTime(date, ma);
                IEnumerable<QuoteInfoDto> tempList = await _dayTradingRepository.QueryQuoteInfo(dayDtos);
                Console.Write("tempList");
                Console.Write(JsonConvert.SerializeObject( tempList));
                List<FiveQuoteInfoDto> result = new();
                string flag = "";
                List<QuoteInfoDto>? fiveDayList = null;
                foreach (var item in tempList)
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
                            bool isDayTrading = false;
                            bool isAmplify = false;
                            dto.id = "" + flag;       
                      
                            dto.open = fiveDayList.Last().open;
                            dto.high = fiveDayList.Max(e => e.high);
                            dto.low = fiveDayList.Min(e => e.low);
                            dto.close = fiveDayList.First().close;
                            dto.fluctuation = ((fiveDayList.First().close - fiveDayList.Last().close) / fiveDayList.Last().close) * 100f;
                            dto.amplitude = (dto.high - dto.low) / fiveDayList.Last().close * 100f;
                            dto.fiveDayVolume = fiveDayList.Sum(e => e.volume);
                            dto.fiveDayList = fiveDayList;

                            var avgVolume = dto.fiveDayVolume / fiveDayList.Count * 1.5;
                            foreach (var dayInfo in fiveDayList)
                            {
                                if (dayInfo.volume >= avgVolume) isBurstVolume = true;                            
                                if (dayInfo.amplitude > 5) isAmplify = true;
                                if (dayInfo.sharesRate > 50) isDayTrading = true;                             
                            }
                            
                            dto.isBurstVolume = isBurstVolume;
                            dto.isDayTrading = isDayTrading;
                            dto.isAmplify = isAmplify;
                            // 爆量平均量低於 5000 不採計
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

                // 當沖標的 低於 200 高於 20 // 
                result = result.FindAll(e => e.isBurstVolume && e.isDayTrading && e.isAmplify && e.open <= 200 && e.open >= 20 );
                result.Sort((a, b) => {                    
                    return (int)Math.Floor(Convert.ToSingle(b.amplitude) * 100) - (int)Math.Floor(Convert.ToSingle(a.amplitude)*100);
                });

                operationResult.Result = result;
                operationResult.RequestResultCode = RequestResultCode.Success;
                return operationResult;

            }
            catch (Exception e) 
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                throw;
            }
        }

        public async Task<string> test() 
        {
            try 
            {
                await _dayTradingRepository.test();
                return "aa";
            }
            catch (Exception e) 
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                throw;
            }
        }

        /*public async Task<OperationResult<List<QuoteInfoDto>>> GetMA20QuoteInfo(string date)
        {
            try
            {
                IEnumerable<TwTradeDayDto> dayDtos = await _dayTradingRepository.QueryUnixTimeByTop21Date(date);

                OperationResult<List<FiveQuoteInfoDto>> operationResult = new();
                IEnumerable<QuoteInfoDto> temp = await _dayTradingRepository.Query6DayQuoteInfo(dayDtos);
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
                            bool isDayTrading = false;
                            bool isAmplify = false;
                            dto.id = "" + flag;

                            float? open = 0;
                            float? high = 0;
                            float? close = 0;
                            float? low = 999999;
                            long? fiveDayVolume = 0;
                            for (var i = 0; i < fiveDayList.Count; i++)
                            {
                                if (i == fiveDayList.Count - 1) continue;
                                if (i == 0) close = fiveDayList[i].close;
                                open = fiveDayList[i].open;
                                if (fiveDayList[i].high >= high) high = fiveDayList[i].high;
                                if (fiveDayList[i].low <= low) low = fiveDayList[i].low;
                                fiveDayVolume = fiveDayVolume + fiveDayList[i].volume;
                                fiveDayList[i].rate = (fiveDayList[i].close - fiveDayList[i + 1].close) / fiveDayList[i + 1].close * 100;
                                fiveDayList[i].sharesRate = (Convert.ToSingle(fiveDayList[i].sharesVolume) / Convert.ToSingle(fiveDayList[i].volume)) / 1000;
                                fiveDayList[i].amplitude = ((fiveDayList[i].high - fiveDayList[i].low) / fiveDayList[i + 1].close) * 100;
                                if (fiveDayList[i].amplitude > 5) isAmplify = true;
                            }

                            dto.open = open;
                            dto.high = high;
                            dto.low = low;
                            dto.close = close;
                            dto.rate = ((close / open) - 1) * 100;
                            dto.amplitude = (dto.high - dto.low) / dto.close * 100;
                            dto.fiveDayVolume = fiveDayVolume;

                            if (fiveDayList.Count == 6) { fiveDayList.RemoveAt(fiveDayList.Count - 1); }

                            dto.fiveDayList = fiveDayList;

                            var avgVolume = dto.fiveDayVolume / fiveDayList.Count * 1.5;
                            foreach (var dayInfo in fiveDayList)
                            {
                                if (dayInfo.volume >= avgVolume)
                                {
                                    isBurstVolume = true;
                                }
                                if (dayInfo.amplitude > 5) isAmplify = true;
                                if (dayInfo.volume > 0 && dayInfo.sharesVolume > 0)
                                {
                                    if (dayInfo.sharesRate > 0.45)
                                    {
                                        isDayTrading = true;
                                    }
                                }
                            }
                            // 是否爆5日平均量 2.0x 爆量平均量低於 5000 不採計
                            dto.isBurstVolume = isBurstVolume;
                            dto.isDayTrading = isDayTrading;
                            dto.isAmplify = isAmplify;
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
                operationResult.Result = result;
                operationResult.RequestResultCode = RequestResultCode.Success;
                return operationResult;
            }
            catch (Exception e)
            {
                _logger.LogError($"錯誤來源 : {e.StackTrace}");
                // _logger.LogError($"傳入參數 : {JsonConvert.SerializeObject(para)}");
                _logger.LogError($"錯誤訊息： {e.Message}");
                throw;
            }
        }*/
    }
}
