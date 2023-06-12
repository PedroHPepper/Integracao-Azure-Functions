using Microsoft.PowerPlatform.Dataverse.Client;
using Pepper.DynamicsModels.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pepper.DynamicsData.Interfaces
{
    public interface IDataverseConnectionService
    {
        ServiceClient DataverseConnection(DynamicsConfig _dynamicsConfig);
    }
}
