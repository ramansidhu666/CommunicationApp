using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Data.Entity;

namespace CommunicationApp.Data
{
    public interface IRepository<TEntity, TContext> 
        where TEntity : class
        where TContext : DbContext
    {
        IQueryable<TEntity> GetAll();
        TEntity GetById(object id);
        IEnumerable<U> GetBy<U>(Expression<Func<TEntity, U>> columns, Expression<Func<TEntity, bool>> where);
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}
