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
                const string targetDate = "";
                const string cookie = "BID=CC0E163B-9984-4EA2-9A2A-A3D988785977; client_fingerprint=2549bb3a7543393b0bea074685f8c67d2b2db2df80210626dc7204e0155caac4; _gcl_au=1.1.1605239311.1731391794; _gid=GA1.2.1624168404.1731391794; twk_idm_key=eazUq1KZTWPP-elBd5VIK; _clck=159iftb%7C2%7Cfqt%7C0%7C1777; cf_clearance=0HNfjORCdiQGHRLyW1pEbZCnPxBvkg3pAQ.kBPxLr6w-1731398842-1.2.1.1-JEIIF34uNedF_a.SfTfr7vn.bFa2tAY3mkUxwA.sf33ZD1anhlYdYB2OljB4t7luEkeTk5amFi1nt5VPRpvPqazLkDWsy55SyRoyAkxrxe5nJ1aCG9K4xUxzmDKF26T2mX8rxuut0vQ0knTRxuDYhplupJJJ.UqAzkjR0DV8j4SfYu148BrRtnWSWcDI7Xo8Xvkglqhf96PAe02UM3FxggjdKgOOsY3TZdxOi4jFmgyfVUNgHrM6EHd2mVSluEkpaMCFimFzAyjOswnfrGkDGD6RSOIPy1FmTd1T3bn7A1Hs52YBxrJyth7dKwleZSehYaiHQopnsKkT7tV31fCBkZgyBYYOwUrF8nyqI1J2WXXQcl5xImk0K_F8IRI6tRnFp_8LjiEAWDjNRiemvrdKzg; FCNEC=%5B%5B%22AKsRol9yI4UyxN3zLwqp0Iz4mgRUrJgXyAlp-wbooEGvb6u9LnLm4YSWLSBMs25vlP-Zs7nYtnlY0pmysBHLnLXg5AXS3KIo8D4w-tXpY7lbq2uHkyMuxSG1vZc-jmR4E0UiQEOgZAx-J0HAImU7I-SoTbiKjotKLg%3D%3D%22%5D%5D; TawkConnectionTime=0; twk_uuid_630dbec937898912e9661d8a=%7B%22uuid%22%3A%221.70htXDoieHzcu8JCPzyAidqEjI26pzedM7woz803mFg6aFqGFnHuCZyuJYINY8iuxIzT1Ervp0HjfVB0JWuz18Cp5vSAm4ij9CzkdVl9TEJhYPepNJue%22%2C%22version%22%3A3%2C%22domain%22%3A%22wantgoo.com%22%2C%22ts%22%3A1731398842633%7D; __gads=ID=8ccd528cf8745ffe:T=1731391801:RT=1731398845:S=ALNI_MY5kGiw180CxIa9lA4b4f_gr1FEpg; __gpi=UID=00000f936299add3:T=1731391801:RT=1731398845:S=ALNI_MZ3VfiptUNZRuH6UgRC1zmQ5eEf8Q; __eoi=ID=abfe45218538df0d:T=1731391801:RT=1731398845:S=AA-AfjZhlC6kxGrS8MQMCMynC8ZF; _clsk=sgzvdl%7C1731398843681%7C5%7C1%7Ce.clarity.ms%2Fcollect; _ga_FCVGHSWXEQ=GS1.1.1731396621.2.1.1731399325.60.0.0; _ga=GA1.2.1916980588.1731391794; _gat_gtag_UA_6993262_2=1";
                const string signature = "6ee96106a94fa15db72b84226ed4a0bd902d15c68a67bed78eb074adb0fa2a95";
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
