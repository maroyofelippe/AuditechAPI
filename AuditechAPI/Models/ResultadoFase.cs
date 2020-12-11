using Dapper.Contrib.Extensions;

namespace AuditechAPI.Models
{
    [Table("RESULTADOFASE")]
    public class ResultadoFase
    {
        public int idResultadoFase { get; set; }
        public double resultadoFase { get; set; }
        public string dataTermino { get; set; }
        public int faseIdFase { get; set; }
        public int pacienteIdPaciente { get; set; }
        public int qtdeResultadoFase { get; set; } 
        public double resultadoErros { get; set; }
        public double resultadoNR { get; set; }
    }
}