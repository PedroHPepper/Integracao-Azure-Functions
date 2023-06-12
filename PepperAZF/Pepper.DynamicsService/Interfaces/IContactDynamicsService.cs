using Microsoft.Xrm.Sdk;
using Pepper.Models.Entities;

namespace Pepper.DynamicsService.Interfaces
{
    public interface IContactDynamicsService
    {
        Task<Entity> GetContact(Contact colaborador);
        Task<Entity> BuildContact(Contact colaborador);
        bool isEntityChanged();
    }
}