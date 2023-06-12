using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pepper.Service.Repositories.Contracts
{
    public interface IBaseService<TEntity> : IDisposable where TEntity : class
    {
        IEnumerable<TEntity> GetAll();
    }
}
