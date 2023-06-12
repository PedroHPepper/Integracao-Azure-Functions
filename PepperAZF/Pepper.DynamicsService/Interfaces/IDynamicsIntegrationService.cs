using Pepper.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pepper.DynamicsService.Interfaces
{
    public interface IDynamicsIntegrationService
    {
        Task<string> IntegrateContact(Contact colaborador);
    }
}
