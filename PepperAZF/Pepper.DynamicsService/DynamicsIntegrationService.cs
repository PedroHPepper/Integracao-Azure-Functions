using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Pepper.DynamicsData.Interfaces;
using Pepper.DynamicsModels.Settings;
using Pepper.DynamicsService.Interfaces;
using Pepper.Models.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pepper.DynamicsService
{
    public class DynamicsIntegrationService : IDynamicsIntegrationService
    {
        private readonly ResponseMessagesAPI _responseMessagesAPI;
        private readonly DynamicsConfig _dynamicsConfig;
        private readonly IContactDynamicsService _contactDynamicsService;
        private readonly IDataverseConnectionService _dynamicsConectionDataverse;
        private readonly ILogger<ContactDynamicsService> _logger;
        public DynamicsIntegrationService(IOptions<ResponseMessagesAPI> responseMessagesAPI,
                                 IOptions<DynamicsConfig> dynamicsConfig,
                                 IContactDynamicsService contactDynamicsService,
                                 [NotNull] IDataverseConnectionService dynamicsConectionDataverse,
                                 ILogger<ContactDynamicsService> logger)
        {
            _responseMessagesAPI = responseMessagesAPI?.Value ?? throw new ArgumentNullException("responseMessagesAPI");
            _dynamicsConfig = dynamicsConfig?.Value ?? throw new ArgumentNullException("dynamicsConfig");
            _dynamicsConectionDataverse = dynamicsConectionDataverse;
            _contactDynamicsService = contactDynamicsService;
            _logger = logger;
        }
        public async Task<string> IntegrateContact(Contact colaborador)
        {
            Entity contact = await _contactDynamicsService.BuildContact(colaborador);
            if (contact == null)
            {
                return _responseMessagesAPI.ResponseCollaboratorWithoutCode;
            }

            //Integrate contact
            ServiceClient service = _dynamicsConectionDataverse.DataverseConnection(_dynamicsConfig);
            if (contact["contactid"] != null)
            {
                if (_contactDynamicsService.isEntityChanged())
                {
                    service.Update(contact);
                    return _responseMessagesAPI.ResponseCollaboratorUpdated;
                }
                return _responseMessagesAPI.ResponseCollaboratorNotUpdated;
            }
            else
            {
                service.Create(contact);
                return _responseMessagesAPI.ResponseCollaboratorCreated;
            }
        }
    }
}
