using System;
using System.Linq;
using Christ3D.Domain.Interfaces;
using Christ3D.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Christ3D.Infra.Data.Repository
{
    /// <summary>
    /// 泛型仓储，实现泛型仓储接口
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly StudyContext Db;
        protected readonly DbSet<TEntity> DbSet;

        public Repository(StudyContext context)
        {
            Db = context;
            DbSet = Db.Set<TEntity>();
        }

        public virtual void Add(TEntity obj)
        {
            DbSet.Add(obj);
        }

        public virtual TEntity GetById(Guid id)
        {
            return DbSet.Find(id);
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return DbSet;
        }

        public virtual void Update(TEntity obj)
        {
            DbSet.Update(obj);
        }

        public virtual void Remove(Guid id)
        {
            DbSet.Remove(DbSet.Find(id));
        }

        public int SaveChanges()
        {
            return Db.SaveChanges();
        }

        public void Dispose()
        {
            Db.Dispose();
            GC.SuppressFinalize(this);//告知垃圾回收器(GC)不再调用对象的终结器(Finalize)。这通常发生在对象被正确释放并且不再需要执行额外的清理操作的情况下。
            /*
             * GC.SuppressFinalize(this); 的目的是告诉垃圾回收器，该对象的终结器已经被显式地调用或者对象已经被释放，不再需要进行额外的终结器调用。这样可以避免额外的终结器回收周期，提高程序性能。

通常情况下，当对象被正确地释放并且终结器不再需要执行时，应该调用 GC.SuppressFinalize(this);。这个通常发生在实现了 IDisposable 接口的类中的 Dispose 方法中，因为 Dispose 方法通常会执行与终结器相同的清理工作。在 Dispose 方法中，先调用 GC.SuppressFinalize(this);，然后再执行清理操作。这样，如果对象已被释放，垃圾回收器就不再需要调用终结器。
             */
        }
    }
}
