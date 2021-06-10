using GP.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
//using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext _dbContext;
        protected readonly DbSet<T> _dbSet;
        protected readonly Database _db;
        public Repository(DbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public virtual int Count
        {
            get { return _dbSet.Count(); }
        }

        public bool ContextTrackChanges
        {
            get { return _dbContext.ChangeTracker.AutoDetectChangesEnabled; }
            set { _dbContext.ChangeTracker.AutoDetectChangesEnabled = value; }
        }

        public virtual IQueryable<T> All()
        {
            return _dbSet.AsQueryable();
        }

        public virtual T GetById(object id)
        {
            return _dbSet.Find(id);
        }

        public virtual IQueryable<T> Get(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "")
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!String.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderBy != null)
            {
                return orderBy(query).AsQueryable();
            }
            else
            {
                return query.AsQueryable();
            }
        }

        public virtual IQueryable<T> Filter(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate).AsQueryable();
        }

        public virtual IQueryable<T> Filter(Expression<Func<T, bool>> filter, out int total, int index = 0, int size = 50)
        {
            int skipCount = index * size;
            var resetSet = filter != null ? _dbSet.Where(filter).AsQueryable() : _dbSet.AsQueryable();
            resetSet = skipCount == 0 ? resetSet.Take(size) : resetSet.Skip(skipCount).Take(size);
            total = resetSet.Count();
            return resetSet.AsQueryable();
        }

        public bool Contains(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Count(predicate) > 0;
        }

        public virtual T Find(params object[] keys)
        {
            return _dbSet.Find(keys);
        }

        public virtual T Find(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate);
        }

        public virtual EntityEntry<T> Create(T entity)
        {
            var newEntry = _dbSet.Add(entity);
            return newEntry;
        }

        public virtual void Delete(object id)
        {
            var entityToDelete = _dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(T entity)
        {
            if (_dbContext.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }
            _dbSet.Remove(entity);
        }

        public virtual void Delete(Expression<Func<T, bool>> predicate)
        {
            var entitiesToDelete = Filter(predicate);
            foreach (var entity in entitiesToDelete)
            {
                if (_dbContext.Entry(entity).State == EntityState.Detached)
                {
                    _dbSet.Attach(entity);
                }
                _dbSet.Remove(entity);
            }
        }

        public virtual void Update(T entity)
        {
            var entry = _dbContext.Entry(entity);
            _dbSet.Attach(entity);
            entry.State = EntityState.Modified;
        }

        public void Commit()
        {

            try
            {
                _dbContext.SaveChanges();
            }
            //catch (DbEntityValidationException e)
            //{
            //    var newException = new FormattedDbEntityValidationException(e);
            //    throw newException;
            //}
            catch (Exception ex)
            {                
                throw;
            }

        }

        public IQueryable<T> AllNoTracking(Expression<Func<T, bool>> includeFilters = null, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _dbSet.AsQueryable();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            if (includeFilters == null)
                return query.AsQueryable().AsNoTracking();

            return query.AsQueryable().AsNoTracking().Where(includeFilters);
        }

        public int ExecuteSQL(string sql, SqlParameter[] parameters)
        {
            return parameters == null
                ? _dbContext.Database.ExecuteSqlRaw(sql)
                : _dbContext.Database.ExecuteSqlRaw(sql, parameters.ToArray<object>());
        }

        public IEnumerable<T> SqlQuery(string sql, params object[] parameters)
        {
            return _dbContext.Set<T>().FromSqlRaw(sql, parameters);
        }
    }
}
