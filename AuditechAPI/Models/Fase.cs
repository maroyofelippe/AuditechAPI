using Dapper.Contrib.Extensions;

namespace AuditechAPI.Models
{
    [Table("FASE")]
    public class Fase
    {
        public int idFase { get; set; }
        public string dataInicio { get; set; }
        public string dataFinal { get; set; }
        public double numDias { get; set; }
        public double qtdeTreinoDia { get; set; }
        public double intervaloTreinoHora { get; set; }
        public double pesoTreino { get; set; }
        public double pesoDesafio { get; set; }
        public int exercicioIdExercicio { get; set; }
        public int tratamentoIdTratamento { get; set; }
        /*public int pacienteIdpaciente { get; set; }*/
    }
}