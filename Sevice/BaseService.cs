using System.Reflection;

namespace ZERO.Sevice
{
    public class BaseService<T> where T : BaseService<T>
    {
         public ILogger<T> _logger;
        public BaseService(ILogger<T> logger)
        {
                _logger = logger;
        }
         public void LogPrinter<P>(Exception e, P para, string type)// where P : class 
         {
             _logger.LogError($"AuthService錯誤，StackTrace : {e.StackTrace}");
             Type classType = typeof(P);
             PropertyInfo[] properties = classType.GetProperties();

             //foreach (var property in properties)
             //    if (property.Name == para.detailType)
             //        property.SetValue(vesselDataDto, para.detailContent);
             //return vesselDataDto;

             //_logger.LogError($"AuthService錯誤，傳入參數，RefreshTokenPara : {JsonConvert.SerializeObject(para)}");
             //_logger.LogError($"AuthService錯誤，呼叫函式 : GetRefreshToken，錯誤訊息：{e.Message}");
         }
    }
}
