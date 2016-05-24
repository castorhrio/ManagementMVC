using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;
using Domain;

namespace Service
{
    /// <summary>
    /// 数据操作基本实现类，公用实现方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class RepositoryBase<T>:IRepository<T> where T:class 
    {
        #region 固定公用帮助，含事物
        private DbContext context = new MyConfig().db;

        /// <summary>
        /// 事务
        /// </summary>
        private DbContextTransaction _transaction = null;

        /// <summary>
        /// 事务状态
        /// </summary>
        public bool Committed { get; set; }

        /// <summary>
        /// 异步锁定
        /// </summary>
        private readonly object sync = new object();

        public DbContext Context
        {
            get
            {
                context.Configuration.ValidateOnSaveEnabled = false;
                return context;
            }
        }

        /// <summary>
        /// 数据上下文--->拓展属性
        /// </summary>
        public MyConfig Config
        {
            get
            {
                return new MyConfig();
            }
        }

        /// <summary>
        ///公用泛型处理属性
        /// 注:所有泛型操作的基础
        /// </summary>
        public DbSet<T> dbSet
        {
            get { return this.Context.Set<T>(); }
        }

        /// <summary>
        /// 开始事务
        /// </summary>
        public DbContextTransaction Transaction
        {
            get
            {
                if (this._transaction == null)
                {
                    this._transaction = this.Context.Database.BeginTransaction();
                }

                return this._transaction;
            }

            set { this._transaction = value; }
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public void Commit()
        {
            if (!Committed)
            {
                lock (sync)
                {
                    if(this._transaction != null)
                        _transaction.Commit();
                }

                Committed = true;
            }
        }


        /// <summary>
        /// 回滚事务
        /// </summary>
        public void Rollback()
        {
            Committed = false;
            if(this._transaction != null)
                this._transaction.Rollback();
        }
        #endregion

        #region 获取单条记录

        public virtual T Get(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return dbSet.AsNoTracking().SingleOrDefault(predicate);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region 增删改操作
        /// <summary>
        /// 添加一条模型记录，自动提交更改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool Save(T entity)
        {
            try
            {
                int row = 0;
                var entry = this.Context.Entry<T>(entity);
                entry.State = System.Data.Entity.EntityState.Added;
                row = Context.SaveChanges();
                entry.State = System.Data.Entity.EntityState.Detached;
                return row > 0;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 更新一条模型记录，自动提交更改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool Update(T entity)
        {
            try
            {
                int rows = 0;
                var entry = this.Context.Entry(entity);
                entry.State = System.Data.Entity.EntityState.Modified;
                rows = this.Context.SaveChanges();
                entry.State = System.Data.Entity.EntityState.Detached;
                return rows > 0;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 更新模型记录，如不存在进行添加操作
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="isEdit"></param>
        /// <returns></returns>
        public virtual bool SaveOrUpdate(T entity, bool isEdit)
        {
            try
            {
                return isEdit ? Update(entity) : Save(entity);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 删除一条或多条模型记录，含事务
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual int Delete(Expression<Func<T, bool>> predicate = null)
        {
            try
            {
                int rows = 0;
                IQueryable<T> entry = (predicate == null) ? this.dbSet.AsQueryable() : this.dbSet.Where(predicate);
                List<T> list = entry.ToList();
                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        this.dbSet.Remove(list[i]);
                    }

                    rows = this.Context.SaveChanges();
                }

                return rows;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 使用原始SQL语句,含事务处理
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        public virtual int DeleteBySql(string sql, params IDbDataParameter[] para)
        {
            try
            {
                return this.Context.Database.ExecuteSqlCommand(sql, para);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region 多模型操作

        /// <summary>
        /// 增加多模型数据，指定独立模型集合
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual int SaveList<T1>(List<T1> t) where T1 : class
        {
            try
            {
                if (t == null || t.Count == 0)
                    return 0;

                this.Context.Set<T1>().Local.Clear();
                foreach (var item in t)
                {
                    this.Context.Set<T1>().Add(item);
                }

                return this.Context.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 增加多模型数据，与当前模型一致
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual int SaveList(List<T> t)
        {
            try
            {
                this.dbSet.Local.Clear();
                foreach (var item in t)
                {
                    this.dbSet.Add(item);
                }

                return this.Context.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 更新多模型，与当前模型一致
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual int UpdateList<T1>(List<T> t)
        {
            if (t.Count <= 0)
                return 0;
            try
            {
                foreach (var item in t)
                {
                    this.Context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                }

                return this.Context.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 批量删除数据，当前模型
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual int DeleteList(List<T> t)
        {
            if (t == null || t.Count == 0)
                return 0;
            foreach (var item in t)
            {
                this.dbSet.Remove(item);
            }

            return this.Context.SaveChanges();
        }

        public virtual int DeleteList<T1>(List<T1> t) where T1 : class
        {
            try
            {
                if (t == null || t.Count == 0)
                    return 0;
                foreach (var item in t)
                {
                    this.Context.Set<T1>().Remove(item);
                }
            }
        }
        #endregion
    }
}
