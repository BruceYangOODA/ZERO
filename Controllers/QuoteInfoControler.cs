﻿using Microsoft.AspNetCore.Authorization;
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
using Swashbuckle.AspNetCore.Annotations;

namespace ZERO.Controllers
{
    //[Authorize]
    //[EnableCors("AllowCors")]    
    [Route("zero/api/[controller]")]
    [ApiController]
    public class QuoteInfoControler : BaseController<QuoteInfoControler>
    {
        private readonly IStockInfoService _stockInfoService;
        public QuoteInfoControler(IStockInfoService stockService)
        {
            _stockInfoService = stockService;
        }

        [HttpGet("GetUnitTimestamp")]
        [SwaggerResponse(200, "Success", typeof(string))]
        public async Task<string> GetUnitTimestamp(string date) 
        {
            try 
            {
                if (date.Length != 8) { return "日期字元長度不足"; }
                int year = int.Parse(date.Substring(0, 4));
                int month = int.Parse(date.Substring(4, 2));
                int day = int.Parse(date.Substring(6, 2));
                if (year < 1970 || month > 12 || day > 31)
                {
                    return "日期格式錯誤 西元年月日 ex 20190123:";                    
                }
                var tagetDay = new DateTime(year, month, day, 16, 0, 0, DateTimeKind.Utc).AddDays(-1);
                long unixTimestamp = (long)(tagetDay - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;              
                return unixTimestamp.ToString();
            }
            catch (Exception e) 
            {
                Logger.LogError(e.Message);
                Logger.LogError(e.StackTrace);
                throw;
            }
        }

        /*[HttpGet("UpdateUnitTimestamp")]
        [SwaggerResponse(200, "Success", typeof(string))]
        public async Task<string> UpdateUnitTimestamp(string date)
        {
            try
            {
                if (date.Length != 8) { return "日期字元長度不足"; }
                int year = int.Parse(date.Substring(0, 4));
                int month = int.Parse(date.Substring(4, 2));
                int day = int.Parse(date.Substring(6, 2));
                if (year < 1970 || month > 12 || day > 31)
                {
                    return "日期格式錯誤 西元年月日 ex 20190123:";
                }
                var tagetDay = new DateTime(year, month, day, 16, 0, 0, DateTimeKind.Utc).AddDays(-1);
                long unixTimestamp = (long)(tagetDay - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                await _stockInfoService.UpdateUnitTimestamp(date);
                return unixTimestamp.ToString();
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                Logger.LogError(e.StackTrace);
                throw;
            }
        }*/

        [HttpGet("GetAllQuoteInfo")]
        //[SwaggerResponse(200, "Success", typeof(List<StockInfo>))]
        // [SwaggerResponse(400, "Bad Request", typeof(string))]
        // [SwaggerResponse(404, "Not Found", typeof(string))]
        public async Task<IActionResult> GetAllQuoteInfo()
        {
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

        [HttpGet("GetDayTradingRef")]
        //[SwaggerResponse(200, "Success", typeof(List<StockInfo>))]
        // [SwaggerResponse(400, "Bad Request", typeof(string))]
        // [SwaggerResponse(404, "Not Found", typeof(string))]
        public async Task<IActionResult> GetDayTradingRef(string cookie, string signature, int differDays = 0)        
        {
            try
            {
                // https://www.wantgoo.com/stock/institutional-investors/foreign/net-buy-sell-rank
                OperationResult<string> operationResult = new();
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
