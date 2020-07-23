using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Hiromi.Bot
{
    public static class Extensions
    {
        public static IAsyncEnumerable<TEntity> AsAsyncEnumerable<TEntity>(this DbSet<TEntity> obj) where TEntity : class
        {
            return EntityFrameworkQueryableExtensions.AsAsyncEnumerable(obj);
        }
        
        public static IQueryable<TEntity> Where<TEntity>(this DbSet<TEntity> obj, Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return Queryable.Where(obj, predicate);
        }
    }
}