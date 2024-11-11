using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZERO.Sevice.IService;
using ZERO.Models;
using ZERO.Models.Dto.StockInfo;
using ZERO.Database.StockInfo;
using ZERO.Models.Enum;
using System.Net;
using System.Collections.Generic;

namespace ZERO.Controllers
{
    //[Authorize]
    //[EnableCors("AllowCors")]    
    [Route("zero/api/[controller]")]
    [ApiController]
    public class ScraperWantgooController : BaseController<ScraperWantgooController>
    {
        private readonly IStockInfoService _stockInfoService;
        public ScraperWantgooController(IStockInfoService stockService) {
            _stockInfoService = stockService;
        }

        [HttpGet]
        //[SwaggerResponse(200, "Success", typeof(List<StockInfo>))]
        // [SwaggerResponse(400, "Bad Request", typeof(string))]
        // [SwaggerResponse(404, "Not Found", typeof(string))]
        public async Task<IActionResult> Scraper(int differDays = 0)
        {             // string targetDate, string _cookie, string _signature
            try
            {
                const string targetDate = "";
                const string cookie = "BID=FA26FAD5-1CE8-4A87-8337-A37C5A0AC3DF; client_fingerprint=0ad5805ed30a5bb3c786469f842e76b962b22705dfcdc38d9e893d6e577eb04a; ad-popup-overlay-12=1730822400000; _gcl_au=1.1.88634124.1730874113; _gid=GA1.2.120011651.1730874114; twk_idm_key=4HRg2Wi0oHpDh8qCj969D; _clck=1oc8qu2%7C2%7Cfqn%7C0%7C1771; cf_clearance=8lSpu29dd_RSJkrbtd48tUEQwncpCJtwQbSl4FkCi8A-1730882296-1.2.1.1-HIccVZ58hgseVKmQzTDYZLZynzQPewgNHpcyJBH_C6P3H3p3LecNGEBRyG8w4Y.Fknr050hRDEB0RZ_8tzZuF1Pg3gfwtLzg9VuhU4qWukwi1gz_TwZU2CWx0p3AnkHOprdXpEL6IaJKOFPicCH.QrR42uRbooew7GFEorwAYNc6pap9DYS1ys_jajsr.qBbiKksm3shRGK8V2ovCNVIZ6Ivu8pzRzbCULxWRN1CV_rnxigst5I3rx_uKZG5EUeNaZ7GEAyWesWUNLFV7fz2cI60pGAyuG7qY_8giM1H3MVPSEpH6ItLTrU1B_bv7AIGG_HEUqdTRhRTiFGwbSYRMdNscSS38kmwSPbwmBiZmCWa_Se2CbHp4aHRT_2GEL9Qdlh6iQ_DFlztOV7Kew0aRg; _gat_gtag_UA_6993262_2=1; FCNEC=%5B%5B%22AKsRol9F2zE2SHqGuFLBKk5aY76aGwoLzZy6DZ43cNnlzRECaoAut4sZaFBnKrmbjq9_OODAlchSMVet_98LrW03vkjxA4rkev7SKNI5UiYI55F_j0sixaRqd9OKR-kO9Kh-tIVqvQe3TGIS0bHwruZF7ux5SjDrrQ%3D%3D%22%5D%5D; TawkConnectionTime=0; twk_uuid_630dbec937898912e9661d8a=%7B%22uuid%22%3A%221.70hsSEJaIqOOgcLvp816zXyfsIqVxtxtv8VQjPmggEUlgQHnVaNjwRZlfp7J7Qx2W4Hq9xVymfqvV2Le1Z6KQ2UKyol7VLbRhdF9Dwz2oR0RwN2cPSgp%22%2C%22version%22%3A3%2C%22domain%22%3A%22wantgoo.com%22%2C%22ts%22%3A1730882294897%7D; __gads=ID=26f3533e2d0162ca:T=1730874120:RT=1730882299:S=ALNI_MZCaFs8DJfpJB4LGPxRQKD7IwcSig; __gpi=UID=00000f6ff554d1e0:T=1730874120:RT=1730882299:S=ALNI_MabbrV9Py3D2HIUHDPzXSNQsAVwaw; __eoi=ID=0e71c923ba2c2ec0:T=1730874120:RT=1730882299:S=AA-AfjaHerVD2KYAxmnJ3kTW4824; _clsk=qaoa9x%7C1730882295935%7C2%7C1%7Ce.clarity.ms%2Fcollect; _ga=GA1.1.1551311679.1730874113; _ga_FCVGHSWXEQ=GS1.1.1730882292.3.1.1730882313.39.0.0";
                const string signature = "22d936226e3c083c483318475ffe1e292b3583740bc4e5773708fb95e2bc32d0";
                // OperationResult<List<QuoteInfo>> operationResult = await _stockInfoService.Scraper(cookie, signature, differDays);
                OperationResult<IEnumerable<QuoteInfoDto>> tempResult = await _stockInfoService.GetAllQuoteInfo();
                OperationResult<List<QuoteInfoDto>> operationResult = await _stockInfoService.PostListQuoteInfo(tempResult.Result.ToList());
                return operationResult.RequestResultCode switch
                {
                    RequestResultCode.Success => Ok(operationResult.Result),
                    RequestResultCode.Failed => BadRequest(operationResult.ErrorMessage),
                    RequestResultCode.NotFound => NotFound(operationResult.ErrorMessage),
                    RequestResultCode.Conflict => Conflict(operationResult.ErrorMessage),
                    RequestResultCode.InternalServerError => throw new Exception(operationResult.ErrorMessage),
                    _ => throw new NotImplementedException()
                };
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                Logger.LogError(e.StackTrace);
                throw;
            }

        }

        [HttpGet("GETALL")]
        //[SwaggerResponse(200, "Success", typeof(List<StockInfo>))]
        // [SwaggerResponse(400, "Bad Request", typeof(string))]
        // [SwaggerResponse(404, "Not Found", typeof(string))]
        public async Task<IActionResult> GETALL() {
            OperationResult<IEnumerable<QuoteInfoDto>> operationResult = await _stockInfoService.GetAllQuoteInfo();
            return operationResult.RequestResultCode switch
            {
                RequestResultCode.Success => Ok(operationResult.Result),
                RequestResultCode.Failed => BadRequest(operationResult.ErrorMessage),
                RequestResultCode.NotFound => NotFound(operationResult.ErrorMessage),
                RequestResultCode.Conflict => Conflict(operationResult.ErrorMessage),
                RequestResultCode.InternalServerError => throw new Exception(operationResult.ErrorMessage),
                _ => throw new NotImplementedException()
            };
        }
    }
}
