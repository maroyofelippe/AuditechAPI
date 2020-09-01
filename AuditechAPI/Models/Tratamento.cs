using Dapper.Contrib.Extensions;

namespace AuditechAPI.Models
{
    [Table("TRATAMENTO")]
    public class Tratamento
    {
        public int idTratamento { get; set; }
        public string dataInicio { get; set; }
        public string dataTermino { get; set; }
        public string observacaoTratamento { get; set; }
        public bool statusTratamento { get; set; }
        public int profissionalIdProfissional { get; set; }
        public int pacienteIdPaciente { get; set; }
        public int clinicaIdClinica { get; set; }
    }
}