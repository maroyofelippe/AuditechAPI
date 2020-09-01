using Dapper.Contrib.Extensions;

namespace AuditechAPI.Models
{

    [Table("CLINICAVIRTUAL")]
    public class ClinicaVirtual
    {
        public int idClinicaVirtual { get; set; }
        public string nomeClinica { get; set; }
        public string enderClinica { get; set; }
        public string compEnderClinica { get; set; }
        public string cepClinica { get; set; }
        public string ufClinica { get; set; }
        public string cnpjClinica { get; set; }
        public string dataAbertura { get; set; }
        public string dataEncerramento { get; set; }
        public bool statusClinica { get; set; }
    }
}