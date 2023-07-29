using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Berger.Extensions.EntityFrameworkCore
{
    public static class ExpressionHelper
    {
        public static IQueryable<T> ConfigureExpressions<T>(this IQueryable<T> query, ExpressionService<T> fields) where T : class
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
    }
}