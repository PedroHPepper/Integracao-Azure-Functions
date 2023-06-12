using Pepper.Data.Database;
using Pepper.Service.Repositories.Contracts;

namespace Pepper.Service.Repositories
{
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : class
    {
        protected readonly ApplicationDbContext _DbContext;
        public BaseService(ApplicationDbContext ApplicationDbContext)
        {
            _DbContext = ApplicationDbContext;
        }
        public IEnumerable<TEntity> GetAll()
        {
            var query = _DbContext.Set<TEntity>();
            if (query.Any())
            {
                return query.ToList();
            }
            return null;
        }
        public void Dispose()
        {
            _DbContext.Dispose();
        }
    }
}