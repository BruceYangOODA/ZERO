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
                OperationResult<string> operationResult = new ();                
                const string cookie = "BID=CC0E163B-9984-4EA2-9A2A-A3D988785977; _gcl_au=1.1.1605239311.1731391794; _gid=GA1.2.1624168404.1731391794; twk_idm_key=eazUq1KZTWPP-elBd5VIK; _clck=159iftb%7C2%7Cfqt%7C0%7C1777; TawkConnectionTime=0; twk_uuid_630dbec937898912e9661d8a=%7B%22uuid%22%3A%221.70htXDoieHzcu8JCPzyAidqEjI26pzedM7woz803mFg6aFqGFnHuCZyuJYINY8iuxIzT1Ervp0HjfVB0JWuz18Cp5vSAm4ij9CzkdVl9TEJhYPepNJue%22%2C%22version%22%3A3%2C%22domain%22%3A%22wantgoo.com%22%2C%22ts%22%3A1731402177708%7D; __gads=ID=8ccd528cf8745ffe:T=1731391801:RT=1731402242:S=ALNI_MY5kGiw180CxIa9lA4b4f_gr1FEpg; __gpi=UID=00000f936299add3:T=1731391801:RT=1731402242:S=ALNI_MZ3VfiptUNZRuH6UgRC1zmQ5eEf8Q; __eoi=ID=abfe45218538df0d:T=1731391801:RT=1731402242:S=AA-AfjZhlC6kxGrS8MQMCMynC8ZF; FCNEC=%5B%5B%22AKsRol8IsO7YMVU-H99Lwsvk5kOSZuCrifRqjHmIsUGvpMOa9sos1ovDe36vodPmsqNUIWsMMGQaNOmjAt5J4CPUz66wyLnCO0zhhFu--nnGyfOpH6YyFLRxt7w2Ghig_FVKX2xe0JPDzG7GK_-vrmJBUsUeaIVTJA%3D%3D%22%5D%5D; _clsk=sgzvdl%7C1731404448174%7C22%7C1%7Ce.clarity.ms%2Fcollect; client_fingerprint=0ad5805ed30a5bb3c786469f842e76b962b22705dfcdc38d9e893d6e577eb04a; cf_clearance=RKZkYqY9rod9BFj1MUMdF1XBhWInvZ3xneNE0q7ejz8-1731404552-1.2.1.1-h5R7aYDwWhknRoB_bDf1VYx.D4SFO2JkMXXoObugWG9pbgS_QIA5PvS2RMDl3BkjRymlsJC2c4latIZwpckZzC1DCidEkMklEj_9FTue95k81Miixyq1nstnOfknyPWYeKvGdLTWd0xIyaK6aI4sytuY3AZfcHha21sStqSUE81M_Boiqtlr05Qzivd8QzGR1Nhe75qR4dfllxNwH4mfSU1mkd2al6QBOYtSiFmM7RzjRQW0AAnPm_g6FuXWmYUn4zo4LIMjJZIBih_uneBgVDTJNqS.Cq8EeSLtVr6jArqfXvoCJHTOSrQWEcLi0KkZZ_tb2SRODyVin7hZ9W0RapigaXUjGZkGqzNd30D4piP_n.C1inJXiU95Kv8ibPcDT7O0HpbeJkvJv2WXdfCKhw; _ga_FCVGHSWXEQ=GS1.1.1731396621.2.1.1731404550.59.0.0; _ga=GA1.2.1916980588.1731391794; _gat_gtag_UA_6993262_2=1";
                const string signature = "c986aae9cea7c45f2247b2092cb5e229cbbaff22bb560c8a92468e3d8ae2f370";
                OperationResult<string> scraperResult = await _stockInfoService.Scraper(cookie, signature, differDays);
                if (scraperResult.RequestResultCode == RequestResultCode.Failed) 
                {                   
                    return BadRequest(scraperResult.ErrorMessage);
                }
         
                //OperationResult<IEnumerable<QuoteInfoDto>> tempResult = await _stockInfoService.GetAllQuoteInfo();
                //string result = await _stockInfoService.PostListQuoteInfo(tempResult.Result);
                operationResult.Result = scraperResult.Result;
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
