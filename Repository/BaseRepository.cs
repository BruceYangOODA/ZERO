﻿using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ZERO.Repository.IRepository;

namespace ZERO.Repository
{
    public class BaseRepository<TEntity> where TEntity : BaseRepository<TEntity>
    {
        protected readonly IConfiguration _configuration;        
        protected readonly string _connectStringStockInfo;
        protected readonly string _connectStringAIShelves;
        protected ILogger<TEntity> _logger;
        protected readonly DbContext _context;        
        public BaseRepository(IConfiguration configuration, ILogger<TEntity> logger, DbContext context)
        {
            _context = context;
            _configuration = configuration;
            _connectStringStockInfo = _configuration.GetConnectionString("StockInfoMysql");            
            //_connectStringAIShelves = _configuration.GetConnectionString("AIShelvesMysql");
            _logger = logger;
        }

    }
}
