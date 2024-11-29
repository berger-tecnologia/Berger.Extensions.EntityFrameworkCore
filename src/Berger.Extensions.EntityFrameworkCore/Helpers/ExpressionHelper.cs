using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Berger.Extensions.EntityFrameworkCore
{
    public static class ExpressionHelper
    {
        #region Methods
        public static IQueryable<T> ConfigureExpressions<T>(this IQueryable<T> query, ExpressionBaseService<T> fields) where T : class
        {
            var entities = fields.Entities;

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

            if (entities.Count() > 0)
            {
                foreach (var expression in entities)
                {
                    query = query.Include(expression);
                }
            }

            return query;
        }
        #endregion
    }
}