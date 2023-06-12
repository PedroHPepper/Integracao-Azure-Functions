using Pepper.Data.Database;
using Pepper.Models.Entities;
using Pepper.Service.Repositories.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pepper.Service.Repositories
{
    public class ContactService : BaseService<Contact>, IContactService
    {
        public ContactService(ApplicationDbContext ApplicationDbContext) : base(ApplicationDbContext)
        {
        }
    }
}
