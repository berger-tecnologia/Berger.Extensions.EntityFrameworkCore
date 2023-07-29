namespace Berger.Extensions.EntityFrameworkCore
{
    public interface IExpressionBuilder<T> where T : class
    {
        ExpressionService<T> Get();
    }
}