using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicsModels.Settings
{
    public class AppSettings
    {
        public DynamicsConfig DynamicsConfig { get; set; }
    }
    public class DynamicsConfig
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Environment { get; set; }
        public string UrlToken { get; set; }
    }
    public class ResponseMessagesAPI
    {
        //Colaborador
        public string ResponseCollaboratorCreated { get; set; }
        public string ResponseCollaboratorUpdated { get; set; }
        public string ResponseCollaboratorNotUpdated { get; set; }
        public string ResponseCollaboratorWithoutCode { get; set; }
    }
}
