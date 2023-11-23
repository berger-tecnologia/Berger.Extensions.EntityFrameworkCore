namespace Berger.Extensions.EntityFrameworkCore
{
    public interface IExpressionBuilder<T> where T : class
    {
        ExpressionBaseService<T> Get();
    }
}