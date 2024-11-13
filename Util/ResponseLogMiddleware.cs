using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using System.Text;

namespace ZERO.Util
{
    /// <summary>
    /// 紀錄 Response Log 使用的 Middleware
    /// </summary>
    public class ResponseLogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        private readonly ILogger _logger;

        public ResponseLogMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
            _logger = loggerFactory.CreateLogger<ResponseLogMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;
            await using var responseBody = _recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseBody;

            // 流入 pipeline
            await _next(context);
            // 流出 pipeline

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBodyTxt = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            await responseBody.CopyToAsync(originalBodyStream);

            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            var attribute = endpoint?.Metadata.GetMetadata<ApiLogAttribute>();

            if (attribute is not null)
            {
                // 須要紀錄 Log
                _logger.LogInformation(
                    $"LogId:{(string)context.Items["ApiLogId"]} " +
                    $"Schema:{context.Request.Scheme} " +
                    $"Host: {context.Request.Host.ToUriComponent()} " +
                    $"Path: {context.Request.Path} " +
                    $"QueryString: {context.Request.QueryString} " +
                    $"ResponseHeader: {GetHeaders(context.Response.Headers)} " +
                    $"ResponseBody: {responseBodyTxt}" +
                    $"ResponseStatus: {context.Response.StatusCode}");
            }
        }

        private static string GetHeaders(IHeaderDictionary headers)
        {
            var headerStr = new StringBuilder();
            foreach (var header in headers)
            {
                headerStr.Append($"{header.Key}: {header.Value}。");
            }

            return headerStr.ToString();
        }
    }
}
