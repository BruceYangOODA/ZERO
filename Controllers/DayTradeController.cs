using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ZERO.Models.Dto.StockInfo;
using ZERO.Models.Enum;
using ZERO.Models;
using ZERO.Sevice.IService;
using ZERO.Util;

namespace ZERO.Controllers
{
    //[Authorize]
    [EnableCors("AllowCors")]
    [Route("zero/api/[controller]")]
    [ApiController]
    public class DayTradeController : BaseController<DayTradeController>
    {
        private readonly IStockInfoService _stockInfoService;
        private readonly IDayTradingService _dayTradingService;
        public DayTradeController(IStockInfoService stockService, IDayTradingService dayTradingService)
        {
            _stockInfoService = stockService;
            _dayTradingService = dayTradingService;
        }

        /*
        [HttpGet("GetQuoteInfoRate")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<QuoteInfoDto>))]
        [SwaggerResponse(400, "Bad Request", typeof(string))]
        [SwaggerResponse(404, "Not Found", typeof(string))]
        public async Task<IActionResult> GetQuoteInfoRate()
        {
            try 
            {
                OperationResult<IEnumerable<QuoteInfoDto>> operationResult = new();
                operationResult.RequestResultCode = RequestResultCode.Success;
                await _dayTradingService.test();

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
        */
        [HttpGet("GetQuoteInfoById/{id}")]
        [SwaggerResponse(200, "Success", typeof(IEnumerable<QuoteInfoDto>))]
        [SwaggerResponse(400, "Bad Request", typeof(string))]
        [SwaggerResponse(404, "Not Found", typeof(string))]
        public async Task<IActionResult> GetQuoteInfoById(string id, string date = "",  int ma = (int)MovingAverage.MA1)
        {
            try 
            {
                OperationResult<IEnumerable<QuoteInfoDto>> operationResult;
                if (id == "")
                {
                    operationResult = new();
                    operationResult.ErrorMessage = "請填寫id";
                    operationResult.RequestResultCode = RequestResultCode.Failed;
                }
                else { 
                    operationResult = await _dayTradingService.GetQuoteInfoById(date, ma, id);
                }
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

        [HttpGet("GetFiveDayQuoteInfo")]
        [SwaggerResponse(200, "Success", typeof(List<FiveQuoteInfoDto>))]
        [SwaggerResponse(400, "Bad Request", typeof(string))]
        [SwaggerResponse(404, "Not Found", typeof(string))]
        public async Task<IActionResult> GetFiveDayQuoteInfo(string date = "", int ma = (int)MovingAverage.MA5 + 1)
        {
            try
            {
                OperationResult<List<FiveQuoteInfoDto>> operationResult = await _dayTradingService.QueryMA5QuoteInfo(date, ma);
                Console.WriteLine("********");
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

        [HttpGet("GetDayTradeRef")]
        [SwaggerResponse(200, "Success", typeof(List<FiveQuoteInfoDto>))]
        [SwaggerResponse(400, "Bad Request", typeof(string))]
        [SwaggerResponse(404, "Not Found", typeof(string))]
        public async Task<IActionResult> GetDayTradeRef(string date = "", int ma = (int)MovingAverage.MA5)
        {
            try
            {
                OperationResult<List<FiveQuoteInfoDto>> operationResult = new();
                if (date != "")
                {
                    ValidMsg isValidDate = UtilDateValidate.CheckDateValidate(date);
                    if (!isValidDate.isValid)
                    {
                        operationResult.ErrorMessage = isValidDate.MSG;
                        operationResult.RequestResultCode = RequestResultCode.Failed;
                    }
                }
                else
                {
                    operationResult = await _dayTradingService.GetDayTradeRef(date, ma);
                }

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

    }
}
