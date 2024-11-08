using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using ZERO.Repository.IRepository;

namespace ZERO.Repository
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        protected readonly IConfiguration _configuration;        
        protected readonly string _connectStringJpmedRWD;
        protected readonly string _connectStringAIShelves;
        protected ILogger<TEntity> _logger;
        protected readonly DbContext _context;
        public virtual DbSet<TEntity> Table => _context.Set<TEntity>();
        public BaseRepository(IConfiguration configuration, ILogger<TEntity> logger, DbContext context)
        {
            _context = context;
            _configuration = configuration;            
            //_connectStringJpmedRWD = _configuration.GetConnectionString("JpmedRWDMysql");
            //_connectStringAIShelves = _configuration.GetConnectionString("AIShelvesMysql");
            _logger = logger;
        }

        public IQueryable<TEntity> GetAll()
        {
            return Table.AsQueryable();
        }
        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate);
        }
        public List<TEntity> GetAllList()
        {
            return GetAll().ToList();
        }
        public List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).ToList();
        }
        public TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Single(predicate);
        }
        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }
        public async Task<TEntity> InsertAsync(TEntity entity)
        {
            var entityEntry = await Table.AddAsync(entity);
            await SaveAsync();
            return entityEntry.Entity;
        }
        public async Task<List<TEntity>> InsertAsync(List<TEntity> entities)
        {
            await Table.AddRangeAsync(entities);
            await SaveAsync();
            return entities;
        }
        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            AttachIfNot(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await SaveAsync();
            return entity;
        }
        public async Task DeleteAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            AttachIfNot(entity);
            Table.Remove(entity);
            await SaveAsync();
        }
        public async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            foreach (var entity in GetAll().Where(predicate).ToList())
            {
                await DeleteAsync(entity);
            }
        }

        /// <summary>
        /// 檢查實體是否處於跟蹤狀態，如果是，則返回；如果不是，則添加跟蹤狀態
        /// </summary>
        /// <param name="entity"></param>
        protected virtual void AttachIfNot(TEntity entity)
        {
            var entry = _context.ChangeTracker.Entries().FirstOrDefault(ent => ent.Entity == entity);
            if (entry != null)
            {
                return;
            }
            Table.Attach(entity);
        }
        protected async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
