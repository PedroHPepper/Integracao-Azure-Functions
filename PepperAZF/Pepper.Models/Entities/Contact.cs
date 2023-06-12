namespace Pepper.Models.Entities
{
    public class Contact
    {
        public string ppr_state { get; set; } //statecode
        public string ppr_status { get; set; } //statecode
        public string ppr_tipo { get; set; } //bz_tipo_contato
        public string ppr_nome { get; set; } //firtsname e lastname
        public string ppr_email { get; set; } //emailaddress1 e bzp_usuariocrm
        public string ppr_codigo { get; set; } //bzp_codigo
        public string ppr_phone { get; set; } //telephone1
        public string ppr_cargo { get; set; } //jobtitle
        public string ppr_area { get; set; } //bz_area_atuacao
        public string ppr_office{ get; set; } //bz_escritorio
    }
}