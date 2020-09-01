using Dapper.Contrib.Extensions;

namespace AuditechAPI.Models
{
    [Table("TREINAMENTOFASE")]
    public class TreinamentoFase
    {
        public int idTreinamentoFase { get; set; }
        public int respostaTreino { get; set; }
        public string dataExecucao { get; set; }
        public int faseIdFase { get; set; }
    }
}