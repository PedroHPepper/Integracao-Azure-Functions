using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using Pepper.DynamicsData.Interfaces;
using Pepper.DynamicsModels.Settings;
using Pepper.DynamicsService.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.PowerPlatform.Dataverse.Client;

namespace Pepper.DynamicsService
{
    public class FieldDynamicsService : IFieldDynamicsService
    {
        private readonly ResponseMessagesAPI _responseMessagesAPI;
        private readonly DynamicsConfig _dynamicsConfig;
        private readonly IDataverseConnectionService _dynamicsConectionDataverse;
        private readonly ILogger<ContactDynamicsService> _logger;
        private bool _entityUpdate = false;
        public FieldDynamicsService(IOptions<ResponseMessagesAPI> responseMessagesAPI,
                                 IOptions<DynamicsConfig> dynamicsConfig,
        [NotNull] IDataverseConnectionService dynamicsConectionDataverse,
                                 ILogger<ContactDynamicsService> logger)
        {
            _responseMessagesAPI = responseMessagesAPI?.Value ?? throw new ArgumentNullException("responseMessagesAPI");
            _dynamicsConfig = dynamicsConfig?.Value ?? throw new ArgumentNullException("dynamicsConfig");
            _dynamicsConectionDataverse = dynamicsConectionDataverse;
            _logger = logger;
        }
        public async Task<string> GetLookupValue(string EntityName, string FieldName, string FieldValue)
        {
            if (!string.IsNullOrEmpty(FieldValue))
            {
                _logger.LogInformation("-> Buscando lookup...");
                ServiceClient service = _dynamicsConectionDataverse.DataverseConnection(_dynamicsConfig);
                var entity = await service.RetrieveMultipleAsync(new FetchExpression($@"
                    <fetch>
                     <entity name=""{EntityName}"">
                      <attribute name=""{FieldName}"" />
                      <filter type=""and"">
                       <condition attribute=""{FieldName}"" operator=""eq"" value=""{FieldValue}"" />
                      </filter>
                     </entity>
                    </fetch>"
                ));

                if (entity.Entities.Count > 0)
                    return (string)entity.Entities.First()[FieldName];
                else
                    _logger.LogInformation($"-> {EntityName} não localizado no crm.");
            }
            return "";
        }
        public async Task<Guid?> GetLookupId(string EntityName, string FieldName, string FieldValue)
        {
            if (!string.IsNullOrEmpty(FieldValue))
            {
                _logger.LogInformation("-> Buscando lookup...");
                ServiceClient service = _dynamicsConectionDataverse.DataverseConnection(_dynamicsConfig);
                var entity = await service.RetrieveMultipleAsync(new FetchExpression($@"
                    <fetch>
                     <entity name=""{EntityName}"">
                      <attribute name=""{FieldName}"" />
                      <filter type=""and"">
                       <condition attribute=""{FieldName}"" operator=""eq"" value=""{FieldValue}"" />
                      </filter>
                     </entity>
                    </fetch>"
                ));

                if (entity.Entities.Count > 0)
                    return entity.Entities.First().Id;
                else
                    _logger.LogInformation($"-> {EntityName} não localizado no crm.");
            }
            return null;
        }
        public async Task<Entity> TransferFieldLookupValue(Entity entity, Entity entityReturn,
            string lookupFieldName, string newLookupFieldValue,
            string entityLogicalName, string entityTargetFieldName,
            string aliasedValueName)
        {

            if (entity.Contains(lookupFieldName))
            {
                if (entity.Contains(aliasedValueName))
                {
                    AliasedValue oldLookupAliasedValue = (AliasedValue)entity[aliasedValueName];
                    if (!string.IsNullOrEmpty(newLookupFieldValue))
                    {
                        if ((string)oldLookupAliasedValue.Value != newLookupFieldValue)
                        {
                            Guid? newLookupGuid = await GetLookupId(entityLogicalName, entityTargetFieldName, newLookupFieldValue);
                            if (newLookupGuid.HasValue)
                            {
                                _logger.LogInformation($"-> mudou {lookupFieldName} por outro...");
                                entityReturn[lookupFieldName] = new EntityReference(entityLogicalName, newLookupGuid.Value);
                                _entityUpdate = true;
                            }
                            else
                            {
                                _logger.LogInformation($"-> remove {lookupFieldName}...");
                                entityReturn[lookupFieldName] = null;
                                _entityUpdate = true;
                            }
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"-> remove {lookupFieldName}...");
                        entityReturn[lookupFieldName] = null;
                        _entityUpdate = true;
                    }

                    return entityReturn;
                }





                Guid? newLookupId = await GetLookupId(entityLogicalName, entityTargetFieldName, newLookupFieldValue);
                EntityReference lookup = (EntityReference)entity[lookupFieldName];
                if (!string.IsNullOrEmpty(newLookupFieldValue))
                {
                    if (newLookupId.HasValue)
                    {
                        if (lookup.Id != newLookupId.Value)
                        {
                            _logger.LogInformation($"-> mudou {lookupFieldName} por outro...");
                            entityReturn[lookupFieldName] = new EntityReference(entityLogicalName, newLookupId.Value);
                            _entityUpdate = true;
                        }
                    }
                    else
                    {
                        _logger.LogInformation($"-> remove {lookupFieldName}...");
                        entityReturn[lookupFieldName] = null;
                        _entityUpdate = true;
                    }
                }
                else
                {
                    _logger.LogInformation($"-> remove {lookupFieldName}...");
                    entityReturn[lookupFieldName] = null;
                    _entityUpdate = true;
                }
                return entityReturn;
            }
            else
            {
                if (!string.IsNullOrEmpty(newLookupFieldValue))
                {
                    Guid? newLookupId = await GetLookupId(entityLogicalName, entityTargetFieldName, newLookupFieldValue);
                    if (newLookupId.HasValue)
                    {
                        _logger.LogInformation($"-> atribui um {lookupFieldName}...");
                        entityReturn[lookupFieldName] = new EntityReference(entityLogicalName, newLookupId.Value);
                        _entityUpdate = true;
                    }
                }
                return entityReturn;
            }
        }
        public Entity TransferFieldValue(Entity entity, Entity entityReturn, string fieldName, string value)
        {
            if (entity.Contains(fieldName))
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (value != (string)entity[fieldName])
                    {
                        _entityUpdate = true;
                        entityReturn[fieldName] = value;
                    }
                }
                else
                {
                    _entityUpdate = true;
                    entityReturn[fieldName] = "";
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _entityUpdate = true;
                    entityReturn[fieldName] = value;
                }
            }
            return entityReturn;
        }
        public Entity TransferFieldOptionSetValue(Entity entity, Entity entityReturn, string fieldName, int? value)
        {
            if (entity.Contains(fieldName))
            {
                if (value.HasValue)
                {
                    if (value.Value != ((OptionSetValue)entity[fieldName]).Value)
                    {
                        _entityUpdate = true;
                        entityReturn[fieldName] = new OptionSetValue(value.Value);
                    }
                }
                else
                {
                    _entityUpdate = true;
                    entityReturn[fieldName] = null;
                }
            }
            else
            {
                if (value.HasValue)
                {
                    _entityUpdate = true;
                    entityReturn[fieldName] = new OptionSetValue(value.Value);
                }
            }
            return entityReturn;
        }
        public bool isEntityChanged()
        {
            return _entityUpdate;
        }
    }
}
