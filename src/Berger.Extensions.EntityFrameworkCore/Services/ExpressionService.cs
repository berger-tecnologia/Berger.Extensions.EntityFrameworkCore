using System.Linq.Expressions;

namespace Berger.Extensions.EntityFrameworkCore
{
    public class ExpressionService<T> where T : class
    {
        #region Properties
        private readonly List<string> _Elements = new List<string>();
        private readonly List<Expression<Func<T, object>>> _Expressions = new List<Expression<Func<T, object>>>();
        #endregion

        #region Methods
        public void Include(Expression<Func<T, object>> selector)
        {
            _Expressions.Add(selector);
        }
        public void Add(string selector)
        {
            _Elements.Add(selector);
        }
        public IEnumerable<Expression<Func<T, object>>> Expressions
        {
            get { return _Expressions; }
        }
        public IEnumerable<string> Elements
        {
            get { return _Elements; }
        }
        #endregion
    }
}