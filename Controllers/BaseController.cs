using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ZERO.Controllers
{
    public class BaseController<T> : ControllerBase where T : BaseController<T>
    {
        //protected UserInfo loginInfo;
        private ILogger<T> _logger;
        protected ILogger<T> Logger => _logger ??= HttpContext?.RequestServices.GetService<ILogger<T>>();
       // protected readonly IUserService _userService;
        protected BaseController() { }
    }
}
