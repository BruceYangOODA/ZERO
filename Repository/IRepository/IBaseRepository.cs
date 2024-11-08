using System.Linq.Expressions;

namespace ZERO.Repository.IRepository
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        #region 查詢
        /// <summary>
        /// 用於獲取傳入本方法的所有實體
        /// </summary>
        /// <returns>所有實體列表</returns>
        IQueryable<TEntity> GetAll();
        /// <summary>
        /// 用於獲取傳入本方法的所有實體 <paramref name="predicate" />
        /// </summary>
        /// <param name="predicate">篩選實體的條件</param>
        /// <returns>所有實體列表</returns>
        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// 用於獲取傳入本方法的所有實體
        /// </summary>
        /// <returns>所有實體列表</returns>
        List<TEntity> GetAllList();
        /// <summary>
        /// 用於獲取傳入本方法的所有實體 <paramref name="predicate" />
        /// </summary>
        /// <param name="predicate">篩選實體的條件</param>
        /// <returns>所有實體列表</returns>
        List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// 通過傳入的篩選條件來獲取實體訊息
        /// 如果查詢不到返回值，則會引起異常
        /// </summary>
        /// <param name="predicate">Entity</param>
        /// <returns></returns>
        TEntity Single(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// 通過傳入的篩選條件查詢實體訊息，如果沒有找到，則返回Null
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
        #endregion
        /// <summary> 異步添加一個新實體訊息 </summary>
        /// <param name="entity">被添加的實體</param>
        /// <returns></returns>
        Task<TEntity> InsertAsync(TEntity entity);
        /// <summary> 異步添加多個新實體訊息 </summary>
        /// <param name="entities">被添加的實體</param>
        /// <returns></returns>
        Task<List<TEntity>> InsertAsync(List<TEntity> entities);
        /// <summary> 異步更新現有實體 </summary>
        /// <param name="entity">Entity</param>
        /// <returns></returns>
        Task<TEntity> UpdateAsync(TEntity entity);
        /// <summary> 異步刪除一個實體 </summary>
        /// <param name="entity">無返回值</param>
        /// <returns></returns>
        Task DeleteAsync(TEntity entity);
        /// <summary>
        /// 按傳入的條件可刪除多個實體
        /// 注意：所有符合給定條件的實體都將被檢索和刪除
        /// 如果條件比較多，則待刪除的實體也比較多，這可能會導致主要的性能問題
        /// </summary>
        /// <param name="predicate">篩選實體的條件</param>
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
