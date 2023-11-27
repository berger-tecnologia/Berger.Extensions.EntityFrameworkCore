using Microsoft.EntityFrameworkCore;
using Berger.Extensions.Abstractions;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Berger.Extensions.EntityFrameworkCore
{
    public static class ExpressionHelper
    {
        #region Methods
        public static IQueryable<T> GetById<T>(this IQueryable<T> query, Guid id) where T : IBaseEntity<Guid>
        {
            return query.Where(e => e.Id == id);
        }
        public static IQueryable<T> GetByCode<T>(this IQueryable<T> query, string code) where T : ICoded
        {
            return query.Where(e => e.Code == code);
        }        
        public static IQueryable<T> GetVerified<T>(this IQueryable<T> query) where T : IVerified
        {
            return query.Where(e => e.Verified == true);
        }        
        public static IQueryable<T> GetByName<T>(this IQueryable<T> query, string name) where T : INamed
        {
            return query.Where(e => e.Name == name);
        }
        public static IQueryable<T> GetByEmail<T>(this IQueryable<T> query, string email) where T : IEmail
        {
            return query.Where(e => e.Email == email);
        }
        public static IQueryable<T> GetBySlug<T>(this IQueryable<T> query, string slug) where T : ISlug
        {
            return query.Where(e => e.Slug == slug);
        }
        public static IQueryable<T> GetActive<T>(this IQueryable<T> query) where T : IActivated
        {
            return query.Where(e => e.Active == true);
        }
        public static IQueryable<T> ConfigureExpressions<T>(this IQueryable<T> query, ExpressionBaseService<T> fields) where T : class
        {
            var elements = fields.Elements;

            var expressions = fields.Expressions;

            if (expressions.Count() > 0)
            {
                foreach (var expression in expressions)
                {
                    var members = expression.GetMemberAccessList();

                    foreach (var member in members)
                    {
                        query = query.Include(member.Name);
                    }
                }
            }

            if (elements.Count() > 0)
            {
                foreach (var expression in elements)
                {
                    query = query.Include(expression);
                }
            }

            return query;
        }
        #endregion
    }
}