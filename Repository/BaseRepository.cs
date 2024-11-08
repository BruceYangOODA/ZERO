using Microsoft.EntityFrameworkCore;

namespace ZERO.Repository
{
    public class BaseRepository<T> where T : BaseRepository<T>
    {
        protected readonly IConfiguration _configuration;        
        protected readonly string _connectStringJpmedRWD;
        protected readonly string _connectStringAIShelves;
        protected ILogger<T> _logger;
        protected readonly DbContext _context;
        public BaseRepository(IConfiguration configuration, ILogger<T> logger, DbContext context)
        {
            _context = context;
            _configuration = configuration;            
            //_connectStringJpmedRWD = _configuration.GetConnectionString("JpmedRWDMysql");
            //_connectStringAIShelves = _configuration.GetConnectionString("AIShelvesMysql");
            _logger = logger;
        }
    }
}
