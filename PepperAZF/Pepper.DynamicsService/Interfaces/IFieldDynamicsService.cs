using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pepper.DynamicsService.Interfaces
{
    public interface IFieldDynamicsService
    {
        Task<string> GetLookupValue(string EntityName, string FieldName, string FieldValue);
        Task<Guid?> GetLookupId(string EntityName, string FieldName, string FieldValue);
        Task<Entity> TransferFieldLookupValue(Entity entity, Entity entityReturn,
            string lookupFieldName, string newLookupFieldValue,
            string entityLogicalName, string entityTargetFieldName,
            string aliasedValueName);
        Entity TransferFieldValue(Entity entity, Entity entityReturn, string fieldName, string value);
        Entity TransferFieldOptionSetValue(Entity entity, Entity entityReturn, string fieldName, int? value);
        bool isEntityChanged();
    }
}
