using Microsoft.AspNetCore.Http;

namespace Boerman.GraphQL.Contrib
{
    public interface IContextProvider<T>
    {
        T Get();
    }

    public class ContextProvider<T> : IContextProvider<T>
    {
        readonly IHttpContextAccessor _contextAccessor;
        public ContextProvider(IHttpContextAccessor contextAccessor)
        {
            this._contextAccessor = contextAccessor;
        }

        public T Get()
        {
            return _contextAccessor?.HttpContext?.RequestServices == null
                ? default(T)
                : _contextAccessor.HttpContext.RequestServices.GetService<T>();
        }
    }
}
