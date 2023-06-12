using Microsoft.PowerPlatform.Dataverse.Client;
using Pepper.DynamicsData.Interfaces;
using Pepper.DynamicsModels.Settings;

namespace Pepper.DynamicsData
{
    public class DataverseConnectionService : IDataverseConnectionService
    {
        public ServiceClient DataverseConnection(DynamicsConfig _dynamicsConfig)
        {
            try
            {
                var connectionString = @$"Url=https://{_dynamicsConfig.Environment}.dynamics.com;AuthType=ClientSecret;ClientId={_dynamicsConfig.ClientId};ClientSecret={_dynamicsConfig.ClientSecret};RequireNewInstance=true";
                var service = new ServiceClient(connectionString);
                return service;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}