using Dapper.Contrib.Extensions;

namespace AuditechAPI.Models
{
    [Table("PACIENTE")]
    public class Paciente
    {
        public int idPaciente { get; set; }
        public bool statusPaciente { get; set; }
        public string nomePai { get; set; }
        public string cpfPai { get; set; }
        public string nomeMae { get; set; }
        public string cpfMae { get; set; }
        public string enderPaciente { get; set; }
        public string compEnderPaciente { get; set; }
        public string cepPaciente { get; set; }
        public string cidadePaciente { get; set; }
        public string ufPaciente { get; set; }
        public int clinicaIdClinica { get; set; }
        public int usuarioIdusuario { get; set; }
    }
}