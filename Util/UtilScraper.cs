using System.Globalization;
using System.Net;
using ZERO.Database.StockInfo;
using ZERO.Models.Enum;
using ZERO.Models;
using ZERO.Models.Dto.StockInfo;
using Newtonsoft.Json;

namespace ZERO.Util
{
    public class UtilScraper
    {
        // https://www.wantgoo.com/stock/institutional-investors/foreign/net-buy-sell-rank
        HttpClient _httpClient = new HttpClient();
        long _unixTimestamp = 0;
        long _unixTimestampBefore = 0;
        string _date = "";
        string _dateBefore = "";
        public UtilScraper(string cookie, string signature, int differDays)
        {
            const string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/130.0.0.0 Safari/537.36";

            /*_httpClient.DefaultRequestHeaders.Add("Authority", "www.wantgoo.com");
            _httpClient.DefaultRequestHeaders.Add("Path", "/stock/institutional-investors/net-buy-sell-tradedate");
            _httpClient.DefaultRequestHeaders.Add("Scheme", "https");
            _httpClient.DefaultRequestHeaders.Add("Method", "GET");*/
            _httpClient.DefaultRequestHeaders.Add("Accept", "*/*");
            //_httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br, zstd");
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "zh-TW,zh;q=0.9");
            _httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            _httpClient.DefaultRequestHeaders.Add("Pragma", "no-cache");
            _httpClient.DefaultRequestHeaders.Add("Priority", "u=1, i");
            //_httpClient.DefaultRequestHeaders.Add("Referer", "https://www.wantgoo.com/stock/institutional-investors/foreign/net-buy-sell-rank");
            _httpClient.DefaultRequestHeaders.Add("Sec-Ch-Ua-Mobile", "?0");
            _httpClient.DefaultRequestHeaders.Add("Sec-Ch-Ua-Platform", "\"Window\"");
            _httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
            _httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
            _httpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", userAgent);
            _httpClient.DefaultRequestHeaders.Add("X-Client-Signature", signature);
            _httpClient.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            _httpClient.DefaultRequestHeaders.Add("Cookie", cookie);

            var tagetDay = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 16, 0, 0).AddDays(differDays - 1);
            _unixTimestamp = (long)(tagetDay - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
            _unixTimestampBefore = _unixTimestamp - 86400000;
            string[] _days = tagetDay.ToString(new CultureInfo("zh-tw")).Split(' ')[0].Split('/');
            _dateBefore = _dateBefore + _days[0] + (int.Parse(_days[1]) > 9 ? _days[1] : "0" + _days[1]) + (int.Parse(_days[2]) > 9 ? _days[2] : "0" + _days[2]);
            _days = tagetDay.AddDays(1).ToString(new CultureInfo("zh-tw")).Split(' ')[0].Split('/');
            _date = _date + _days[0] + (int.Parse(_days[1]) > 9 ? _days[1] : "0" + _days[1]) + (int.Parse(_days[2]) > 9 ? _days[2] : "0" + _days[2]);

            Console.WriteLine("_unixTimestamp" + _unixTimestamp);
            Console.WriteLine("_date" + _date);
            Console.WriteLine("_unixTimestampBefore" + _unixTimestampBefore);
            Console.WriteLine("_dateBefore" + _dateBefore);
        }

        public async Task<OperationResult<List<QuoteInfoDto>>> GetHistoricalAllQuoteInfo()
        {
            OperationResult<List<QuoteInfoDto>> operationResult = new();
            try
            {
                string apiUrl = "https://www.wantgoo.com/investrue/historical-all-quote-info?tradeDate=" + _unixTimestamp;
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    operationResult.ErrorMessage = "無法取得網頁資料 刷新 Cookie & X-Client-Signature _ GetHistoricalAllQuoteInfo";
                    operationResult.RequestResultCode = RequestResultCode.Failed;
                    return operationResult;
                }
                //Console.WriteLine(response.Headers);
                //Console.WriteLine("=======");
                //Console.WriteLine(response.RequestMessage);
                //Console.WriteLine("=======");
                Console.WriteLine("StatusCode " + response.StatusCode);
                // Console.WriteLine(response.Content.GetType());


                HttpContent content = response.Content;
                //string responseData =  await content.ReadAsStringAsync();
                //List<QuoteInfo> quoteInfoList = JsonConvert.DeserializeObject<List<QuoteInfo>>(responseData) as List<QuoteInfo>;
                //quoteInfoList = quoteInfoList.FindAll(q =>
                //{
                //    q.date = _date;
                //    return true;
                //});
                //
                List<QuoteInfoDto>? quoteInfoList = await content.ReadFromJsonAsync<List<QuoteInfoDto>>();
                if (quoteInfoList == null)
                {
                    quoteInfoList = new List<QuoteInfoDto>();
                    operationResult.RequestResultCode = RequestResultCode.NotFound;
                    return operationResult;
                }

                quoteInfoList = quoteInfoList.FindAll(q =>
                {
                    q.date = _date;
                    return Char.IsDigit(q.id[0]);
                });
                operationResult.RequestResultCode = RequestResultCode.Success;
                operationResult.Result = quoteInfoList;

                return operationResult;
            }
            catch (Exception e)
            {
                operationResult.RequestResultCode = RequestResultCode.Failed;
                operationResult.ErrorMessage = "無法取得網頁資料 刷新 Cookie & X-Client-Signature" + e.Message;
                return operationResult;
            }
        }
        public async Task<OperationResult<List<ForeignBuySellDto>>> GetAllForeignBuySell()
        {
            OperationResult<List<ForeignBuySellDto>> operationResult = new();
            try 
            {
                string apiUrl = "https://www.wantgoo.com/stock/institutional-investors/foreign/all-net-buy-rank-data?tradeDate=" + _unixTimestamp + "&accumulationDays=1&market=Listed,OTC";
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    operationResult.ErrorMessage = "無法取得網頁資料 刷新 Cookie & X-Client-Signature _ GetAllForeignBuySell";
                    operationResult.RequestResultCode = RequestResultCode.Failed;
                    return operationResult;
                }

                HttpContent content = response.Content;
  
                List<ForeignBuySellDto>? buySellList = await content.ReadFromJsonAsync<List<ForeignBuySellDto>>();
                if (buySellList == null)
                {
                    buySellList = new List<ForeignBuySellDto>();
                    operationResult.RequestResultCode = RequestResultCode.NotFound;
                    return operationResult;
                }
                buySellList.ForEach((item) => 
                {
                    item.date = _date;
                });
               // Console.WriteLine(JsonConvert.SerializeObject(buySellList));

                apiUrl = "https://www.wantgoo.com/stock/institutional-investors/foreign/all-net-sell-rank-data?tradeDate="+ _unixTimestamp + "&accumulationDays=1&market=Listed,OTC";
                response = await _httpClient.GetAsync(apiUrl);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    operationResult.ErrorMessage = "無法取得網頁資料 刷新 Cookie & X-Client-Signature _ GetAllForeignBuySell";
                    operationResult.RequestResultCode = RequestResultCode.Failed;
                    return operationResult;
                }

                content = response.Content;

                List<ForeignBuySellDto>? sellBuyList = await content.ReadFromJsonAsync<List<ForeignBuySellDto>>();
                if (sellBuyList == null)
                {
                    sellBuyList = new List<ForeignBuySellDto>();
                    operationResult.RequestResultCode = RequestResultCode.NotFound;
                    return operationResult;
                }
                sellBuyList.ForEach((item) =>
                {
                    item.date = _date;
                });
                //Console.WriteLine(JsonConvert.SerializeObject(sellBuyList));
                operationResult.RequestResultCode = RequestResultCode.Success;
                operationResult.Result = buySellList.Concat(sellBuyList).ToList();
                return operationResult;
            }
            catch (Exception e) 
            {
                operationResult.RequestResultCode = RequestResultCode.Failed;
                operationResult.ErrorMessage = "無法取得網頁資料 刷新 Cookie & X-Client-Signature" + e.Message;
                return operationResult;
            }
     
        }

        public async Task<OperationResult<List<DealerBuySellDto>>> GetAllDealerBuySell()
        {
            OperationResult<List<DealerBuySellDto>> operationResult = new();
            try
            {
                string apiUrl = "https://www.wantgoo.com/stock/institutional-investors/dealer/all-net-buy-rank-data?tradeDate="+ _unixTimestamp + "&accumulationDays=1&market=Listed,OTC";
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    operationResult.ErrorMessage = "無法取得網頁資料 刷新 Cookie & X-Client-Signature _ GetAllDealerBuySell";
                    operationResult.RequestResultCode = RequestResultCode.Failed;
                    return operationResult;
                }

                HttpContent content = response.Content;

                List<DealerBuySellDto>? buySellList = await content.ReadFromJsonAsync<List<DealerBuySellDto>>();
                if (buySellList == null)
                {
                    buySellList = new List<DealerBuySellDto>();
                    operationResult.RequestResultCode = RequestResultCode.NotFound;
                    return operationResult;
                }
                buySellList.ForEach((item) =>
                {
                    item.date = _date;
                });
                //Console.WriteLine(JsonConvert.SerializeObject(buySellList));

                apiUrl = "https://www.wantgoo.com/stock/institutional-investors/dealer/all-net-sell-rank-data?tradeDate=" + _unixTimestamp + "&accumulationDays=1&market=Listed,OTC";
                response = await _httpClient.GetAsync(apiUrl);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    operationResult.ErrorMessage = "無法取得網頁資料 刷新 Cookie & X-Client-Signature _ GetAllDealerBuySell";
                    operationResult.RequestResultCode = RequestResultCode.Failed;
                    return operationResult;
                }

                content = response.Content;

                List<DealerBuySellDto>? sellBuyList = await content.ReadFromJsonAsync<List<DealerBuySellDto>>();
                if (sellBuyList == null)
                {
                    sellBuyList = new List<DealerBuySellDto>();
                    operationResult.RequestResultCode = RequestResultCode.NotFound;
                    return operationResult;
                }
                sellBuyList.ForEach((item) =>
                {
                    item.date = _date;
                });

                //Console.WriteLine(JsonConvert.SerializeObject(sellBuyList));
                operationResult.RequestResultCode = RequestResultCode.Success;
                operationResult.Result = buySellList.Concat(sellBuyList).ToList();
                return operationResult;
            }
            catch (Exception e)
            {
                operationResult.RequestResultCode = RequestResultCode.Failed;
                operationResult.ErrorMessage = "無法取得網頁資料 刷新 Cookie & X-Client-Signature" + e.Message;
                return operationResult;
            }

        }

        public async Task<OperationResult<List<TrustBuySellDto>>> GetAllTrustBuySell()
        {
            OperationResult<List<TrustBuySellDto>> operationResult = new();
            try
            {
                string apiUrl = "https://www.wantgoo.com/stock/institutional-investors/investment-trust/all-net-buy-rank-data?tradeDate=" + _unixTimestamp + "&accumulationDays=1&market=Listed,OTC";
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    operationResult.ErrorMessage = "無法取得網頁資料 刷新 Cookie & X-Client-Signature _ GetHistoricalAllQuoteInfo";
                    operationResult.RequestResultCode = RequestResultCode.Failed;
                    return operationResult;
                }

                HttpContent content = response.Content;

                List<TrustBuySellDto>? buySellList = await content.ReadFromJsonAsync<List<TrustBuySellDto>>();
                if (buySellList == null)
                {
                    buySellList = new List<TrustBuySellDto>();
                    operationResult.RequestResultCode = RequestResultCode.NotFound;
                    return operationResult;
                }
                buySellList.ForEach((item) =>
                {
                    item.date = _date;
                });     
                
                //Console.WriteLine(JsonConvert.SerializeObject(buySellList));

                apiUrl = "https://www.wantgoo.com/stock/institutional-investors/investment-trust/all-net-sell-rank-data?tradeDate=" + _unixTimestamp + "&accumulationDays=1&market=Listed,OTC";
                response = await _httpClient.GetAsync(apiUrl);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    operationResult.ErrorMessage = "無法取得網頁資料 刷新 Cookie & X-Client-Signature _ GetAllTrustBuySell";
                    operationResult.RequestResultCode = RequestResultCode.Failed;
                    return operationResult;
                }

                content = response.Content;

                List<TrustBuySellDto>? sellBuyList = await content.ReadFromJsonAsync<List<TrustBuySellDto>>();
                if (sellBuyList == null)
                {
                    sellBuyList = new List<TrustBuySellDto>();
                    operationResult.RequestResultCode = RequestResultCode.NotFound;
                    return operationResult;
                }
                sellBuyList.ForEach((item) =>
                {
                    item.date = _date;
                });            
                //Console.WriteLine(JsonConvert.SerializeObject(sellBuyList));
                operationResult.RequestResultCode = RequestResultCode.Success;
                operationResult.Result = buySellList.Concat(sellBuyList).ToList();
                return operationResult;
            }
            catch (Exception e)
            {
                operationResult.RequestResultCode = RequestResultCode.Failed;
                operationResult.ErrorMessage = "無法取得網頁資料 刷新 Cookie & X-Client-Signature" + e.Message;
                return operationResult;
            }

        }
        public async Task<OperationResult<List<VolumeDataDto>>> GetAllVolumeData()
        {
            OperationResult<List<VolumeDataDto>> operationResult = new();
            try
            {
                string apiUrl = "https://www.wantgoo.com/stock/day-trading/all-volume-data?tradeDate=" + _unixTimestampBefore;
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    operationResult.ErrorMessage = "無法取得網頁資料 刷新 Cookie & X-Client-Signature _ GetAllVolumeData";
                    operationResult.RequestResultCode = RequestResultCode.Failed;
                    return operationResult;
                }

                HttpContent content = response.Content;

                List<VolumeDataDto>? dtoList = await content.ReadFromJsonAsync<List<VolumeDataDto>>();
                if (dtoList == null)
                {
                    dtoList = new List<VolumeDataDto>();
                    operationResult.RequestResultCode = RequestResultCode.NotFound;
                    return operationResult;
                }

                dtoList.ForEach((dto) => {
                    dto.theDate = _dateBefore;
                });

                Console.WriteLine(JsonConvert.SerializeObject(dtoList));

                operationResult.RequestResultCode = RequestResultCode.Success;
                operationResult.Result = dtoList;

                return operationResult;
            }
            catch (Exception e)
            {
                operationResult.RequestResultCode = RequestResultCode.Failed;
                operationResult.ErrorMessage = "無法取得網頁資料 刷新 Cookie & X-Client-Signature" + e.Message;
                return operationResult;
            }
        }


        


    }

}
