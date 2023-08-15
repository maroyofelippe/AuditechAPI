using Dapper.Contrib.Extensions;

namespace AuditechAPI.Models
{
    [Table("FATURAMENTO")]
    public class Faturamento
    
    {
        public int idFaturamento { get; set; }
        public string numNF { get; set; }
        public string dataFaturamento { get; set; }
        public int qtdePaciente { get; set; }
        public int qtdeTratamento { get; set; }
        public int qtdeTreinamento { get; set; }
        public double vlPaciente { get; set; }
        public double vlTratamento { get; set; }
        public double vlTreinamento { get; set; }
        public double vlClinica { get; set; }
        public double vlTotal { get; set; }
        public double aliqImposto { get; set; }
        public int clinicaIdClinica { get; set; }
    }
}