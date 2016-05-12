using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using Common;

namespace Service
{
    public interface IRepository<T> where T:class
    {
        #region 单模型操作
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="predicate">主键</param>
        /// <returns>实体</returns>
        T Get(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 插入实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>ID</returns>
        bool Save(T entity);

        /// <summary>
        /// 修改实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        bool Update(T entity);

        /// <summary>
        /// 修改或保存实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="isEdit"></param>
        /// <returns></returns>
        bool SaveOrUpdate(T entity, bool isEdit);

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        int Delete(Expression<Func<T, bool>> predicate = null);

        /// <summary>
        /// 执行SQL删除
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        int DeleteBySql(string sql, params DbParameter[] para);

        /// <summary>
        /// 根据属性验证实体对象是否存在
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool IsExist(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 根据SQL验证实体对象是否存在
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        bool IsExist(string sql, params DbParameter[] para);

        #endregion

        #region 多模型操作
        /// <summary>
        /// 增加多模型数据，指定独立模型集合
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        int SaveList<T1>(List<T1> t) where T1 : class;

        /// <summary>
        /// 增加多模型数据，与当前模型一致
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        int SaveList(List<T> t);

        /// <summary>
        /// 更新多模型，指定独立模型集合
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        int UpdateList<T1>(List<T1> t) where T1 : class;

        /// <summary>
        /// 更新多模型，与当前模型一致
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        int UpdateList(List<T> t);

        /// <summary>
        /// 批量删除数据，当前模型
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        int DeleteList(List<T> t);

        /// <summary>
        /// 批量删除数据，独立模型
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        int DeleteList<T1>(List<T1> t) where T1 : class;

        #endregion

        #region 存储过程操作
        /// <summary>
        /// 执行增删改存储过程
        /// </summary>
        /// <param name="procname"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        object ExecuteProc(string procname, params DbParameter[] parameter);

        /// <summary>
        /// 执行查询的存储过程
        /// </summary>
        /// <param name="procname"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        object ExecuteQueryProc(string procname, params DbParameter[] parameter);

        #endregion

        #region 查询多条数据

        IQueryable<T> LoadAll(Expression<Func<T, bool>> predicate);

        List<T> LoadLiatAll(Expression<Func<T, bool>> predicate);

        DbQuery<T> LoadQueryAll(Expression<Func<T, bool>> predicate);

        IEnumerable<T> LoadEnumerableAll(string sql, params DbParameter[] para);

        System.Collections.IEnumerable LoadEnumerable(string sql, params DbParameter[] para);

        List<T> SelectBySql(string sql, params DbParameter[] para);

        List<T1> SelectBySql<T1>(string sql, params DbParameter[] para);

        List<TResult> QueryEntity<TEntity, TOrderBy, TResult>(Expression<Func<TEntity, bool>> where,
            Expression<Func<TEntity, TOrderBy>> orderby, Expression<Func<TEntity, TResult>> selector, bool IsAsc)
            where TEntity : class where TResult : class;
        #endregion

        #region 分页查询
        /// <summary>
        /// 通过SQL分页
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        IList<T1> PageByListSql<T1>(string sql, IList<DbParameter> parameters, PageCollection page);

        IList<T> PageByListSql(string sql, IList<DbParameter> parameters, PageCollection page);

        /// <summary>
        /// 通用EF分页，默认显示20条记录
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TOrderBy"></typeparam>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <param name="where"></param>
        /// <param name="orderby"></param>
        /// <param name="selector"></param>
        /// <param name="IsAsc"></param>
        /// <returns></returns>
        PageCollection.PageInfo<object> Query<TEntity, TOrderBy>(int index, int pageSize,
            Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, TOrderBy>> orderby,
            Func<IQueryable<TEntity>, List<object>> selector, bool IsAsc) where TEntity : class;

        /// <summary>
        /// 对IQueryable对象进行分页逻辑处理，过滤、查询项、排序对IQueryable操作
        /// </summary>
        /// <param name="query"></param>
        /// <param name="index"></param>
        /// <param name="PageSize"></param>
        /// <returns></returns>
        PageCollection.PageInfo<T> Query(IQueryable<T> query, int index, int PageSize);

        /// <summary>
        /// 普通SQL查询分页方法
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <param name="tableName"></param>
        /// <param name="field"></param>
        /// <param name="filter"></param>
        /// <param name="orderby"></param>
        /// <param name="group"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        PageCollection.PageInfo Query(int index, int pageSize, string tableName, string field, string filter, string orderby, string group, params DbParameter[] para);

        /// <summary>
        /// 简单的Sql查询分页
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <param name="sql"></param>
        /// <param name="orderby"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        PageCollection.PageInfo Query(int index, int pageSize, string sql, string orderby, params DbParameter[] para);

        /// <summary>
        /// 多表联合分页算法
        /// </summary>
        /// <param name="query"></param>
        /// <param name="index"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        PageCollection.PageInfo Query(IQueryable query, int index, int pagesize);
        #endregion

        #region ADO.NET增删改查方法
        /// <summary>
        /// 执行增删改方法,含事务处理
        /// </summary>
        object ExecuteSqlCommand(string sql, params DbParameter[] para);
        /// <summary>
        /// 执行多条SQL，增删改方法,含事务处理
        /// </summary>
        object ExecuteSqlCommand(Dictionary<string, object> sqllist);
        /// <summary>
        /// 执行查询方法,返回动态类，接收使用var，遍历时使用dynamic类型
        /// </summary>
        object ExecuteSqlQuery(string sql, params DbParameter[] para);
        #endregion
    }
}
