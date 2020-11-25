using Dapper.Contrib.Extensions;

namespace AuditechAPI.Models
{
    [Table("RESULTASOFASE")]
    public class ResultadosFase
    {
        public int idResultadoFase { get; set; }
        public double resultadoFase { get; set; }
        public string dataTermino { get; set; }
        public int faseIdFase { get; set; }
        public int pacienteIdPaciente { get; set; }
    }
}