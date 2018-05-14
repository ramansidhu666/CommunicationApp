using CommunicationApp.Core;
using CommunicationApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq.Expressions;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;

namespace CommunicationApp.Data
{
    public class RepositoryService<TEntity, TContext> : IRepository<TEntity, TContext>
        where TEntity : class
        where TContext : DbContext
    {
        //private readonly IDbContext _context;
        protected TContext _context;
        private IDbSet<TEntity> _entities
        {
            get { return this._context.Set<TEntity>(); }
        }

        public RepositoryService(IDbContext context)
        {
            this._context = context as TContext;
        }

        public IQueryable<TEntity> GetAll()
        {
            return _entities.AsQueryable();
        }

        public TEntity GetById(object id)
        {
            return _entities.Find(id);
        }


        public IEnumerable<U> GetBy<U>(Expression<Func<TEntity, U>> columns, Expression<Func<TEntity, bool>> where)
        {
            return _entities.Where<TEntity>(where).Select<TEntity, U>(columns);
        }

        public virtual void Insert(TEntity entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                var entry = this._context.Entry(entity);

                if (entry.State != EntityState.Detached)
                {
                    this._context.Set<TEntity>().Attach(entity);
                    entry.State = EntityState.Added;
                }
                else
                {
                    this._entities.Add(entity);
                }

                this._context.SaveChanges();

            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;

                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }

        public virtual void Update(TEntity entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                var entry = this._context.Entry(entity);
                if (entry.State == EntityState.Detached)
                {
                    var key = this.GetPrimaryKey(entry);
                    var currentEntry = this._context.Set<TEntity>().Find(key);
                    if (currentEntry != null)
                    {
                        var attachedEntry = this._context.Entry(currentEntry);
                        attachedEntry.CurrentValues.SetValues(entity);
                    }
                    else
                    {
                        this._context.Set<TEntity>().Attach(entity);
                        entry.State = EntityState.Modified;
                    }
                }

                //if (_context.Entry(entity).State == EntityState.Detached)
                //{
                //    _context.Set<TEntity>().Attach(entity);
                //    this._context.Entry(entity).State = EntityState.Modified;
                //}

                this._context.SaveChanges();

            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;

                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }

            //if (this.AutoCommitEnabled)
            //{
            //    _context.SaveChanges();
            //}
            //else
            //{
            //    try
            //    {
            //        this.Entities.Attach(entity);
            //        InternalContext.Entry(entity).State = System.Data.EntityState.Modified;
            //    }
            //    finally { }
            //}
        }

        public virtual void Delete(TEntity entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity");

                this._entities.Remove(entity);

                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;

                var fail = new Exception(msg, dbEx);
                //Debug.WriteLine(fail.Message, fail);
                throw fail;
            }
        }

        private int GetPrimaryKey(DbEntityEntry entry)
        {
            var myObject = entry.Entity;
            var property = myObject.GetType().GetProperties().FirstOrDefault();
            //.GetProperties().FirstOrDefault(prop =&gt; Attribute.IsDefined(prop, typeof(KeyAttribute)));
            return (int)property.GetValue(myObject, null);

        }
    }
}
