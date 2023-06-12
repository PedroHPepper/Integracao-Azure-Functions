using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using Pepper.DynamicsService.Interfaces;
using Pepper.DynamicsService.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Pepper.DynamicsData.Interfaces;
using Pepper.DynamicsModels.Settings;
using Microsoft.Extensions.Options;
using Microsoft.PowerPlatform.Dataverse.Client;
using Pepper.Models.Entities;

namespace Pepper.DynamicsService
{
    public class ContactDynamicsService : IContactDynamicsService
    {
        private readonly ResponseMessagesAPI _responseMessagesAPI;
        private readonly DynamicsConfig _dynamicsConfig;
        private readonly IDataverseConnectionService _dynamicsConectionDataverse;
        private IFieldDynamicsService _fieldDynamicsService;
        private readonly ILogger<ContactDynamicsService> _logger;
        private bool _entityUpdate = false;

        public ContactDynamicsService(IOptions<ResponseMessagesAPI> responseMessagesAPI,
                                 IOptions<DynamicsConfig> dynamicsConfig,
                                 IFieldDynamicsService fieldDynamicsService,
                                 [NotNull] IDataverseConnectionService dynamicsConectionDataverse,
                                 ILogger<ContactDynamicsService> logger)
        {
            _responseMessagesAPI = responseMessagesAPI?.Value ?? throw new ArgumentNullException("responseMessagesAPI");
            _dynamicsConfig = dynamicsConfig?.Value ?? throw new ArgumentNullException("dynamicsConfig");
            _fieldDynamicsService = fieldDynamicsService;
            _dynamicsConectionDataverse = dynamicsConectionDataverse;
            _logger = logger;
        }
        public async Task<Entity> BuildContact(Contact contact)
        {
            if (string.IsNullOrEmpty(contact.ppr_codigo))
            {
                return null;
            }
            Entity contactCRM = await GetContact(contact);

            Entity contactReturn = new Entity("contact");
            if (contactCRM["contactid"] != null)
                contactReturn = new Entity("contact", contactCRM.Id);


            if (!contactCRM.Contains("bzp_codigo"))
                contactReturn["bzp_codigo"] = contact.ppr_codigo;

            #region contact name
            string[] fullnameArray = StringUtils.formatName(contact.ppr_nome);
            contactReturn = _fieldDynamicsService.TransferFieldValue(contactCRM, contactReturn, "firstname", fullnameArray[0]);
            contactReturn = _fieldDynamicsService.TransferFieldValue(contactCRM, contactReturn, "lastname", fullnameArray[1]);
            #endregion

            if (!string.IsNullOrEmpty(contact.ppr_status))
            {
                contactReturn = _fieldDynamicsService.TransferFieldOptionSetValue(contactCRM, contactReturn,
                    "statecode", Convert.ToInt32(contact.ppr_status));
            }

            if (!string.IsNullOrEmpty(contact.ppr_tipo))
            {
                contactReturn = _fieldDynamicsService.TransferFieldOptionSetValue(contactCRM, contactReturn,
                    "pep_tipo", Convert.ToInt32(contact.ppr_tipo));
            }


            contactReturn = _fieldDynamicsService.TransferFieldValue(contactCRM, contactReturn, "emailaddress1", contact.ppr_email);

            contactReturn = await _fieldDynamicsService.TransferFieldLookupValue(contactCRM, contactReturn, "pep_usuario",
                contact.ppr_email, "systemuser", "internalemailaddress", "pep_usuario.internalemailaddress");

            contactReturn = _fieldDynamicsService.TransferFieldValue(contactCRM, contactReturn, "emailaddress1", contact.ppr_email);
            contactReturn = _fieldDynamicsService.TransferFieldValue(contactCRM, contactReturn, "telephone1", contact.ppr_phone);
            contactReturn = _fieldDynamicsService.TransferFieldValue(contactCRM, contactReturn, "jobtitle", contact.ppr_cargo);

            
            contactReturn = await _fieldDynamicsService.TransferFieldLookupValue(contactCRM, contactReturn, "pep_area",
                contact.ppr_area, "pep_area", "pep_codigo", "pep_area.pep_codigo");

            contactReturn = await _fieldDynamicsService.TransferFieldLookupValue(contactCRM, contactReturn, "pep_office",
                contact.ppr_office, "pep_office", "pep_codigo", "pep_office.pep_codigo");

            _entityUpdate = _fieldDynamicsService.isEntityChanged();

            return contactReturn;
        }
        public async Task<Entity> GetContact(Contact contact)
        {
            _logger.LogInformation("-> Buscando contato...");

            ServiceClient service = _dynamicsConectionDataverse.DataverseConnection(_dynamicsConfig);

            var contacts = await service.RetrieveMultipleAsync(new FetchExpression($@"
                    <fetch>
                     <entity name=""contact"">
                      <attribute name=""contactid"" />
                      <attribute name=""statecode"" />
                      <attribute name=""firstname"" />
                      <attribute name=""lastname"" />
                      <attribute name=""emailaddress1"" />
                      <attribute name=""pep_tipo"" />

                      <attribute name=""pep_usuario"" />
                      <link-entity name=""systemuser"" from=""systemuserid"" to=""pep_usuario"" visible=""false"" link-type=""outer"" alias=""pep_usuario"">
                         <attribute name=""internalemailaddress"" />
                      </link-entity>
                      <attribute name=""pep_codigo"" />
                      <attribute name=""telephone1"" />
                      <attribute name=""jobtitle"" />

                      <attribute name=""pep_area"" />
                      <link-entity name=""pep_area"" from=""pep_areaid"" to=""pep_area"" visible=""false"" link-type=""outer"" alias=""pep_area"">
                         <attribute name=""pep_codigo"" />
                      </link-entity>

                      <attribute name=""pep_office"" />
                      <link-entity name=""pep_office"" from=""pep_officeid"" to=""pep_office"" visible=""false"" link-type=""outer"" alias=""pep_office"">
                         <attribute name=""pep_codigo"" />
                      </link-entity>
                      <filter type=""and"">
                       <condition attribute=""pep_codigo"" operator=""eq"" value=""{contact.ppr_codigo}"" />
                      </filter>
                     </entity>
                    </fetch>"
            ));

            if (contacts.Entities.Count > 0)
                return contacts.Entities.First();
            else
                _logger.LogInformation("-> Contato não localizado no crm.");
            return new Entity("contact");
        }
        public bool isEntityChanged()
        {
            return _entityUpdate;
        }
    }
}
