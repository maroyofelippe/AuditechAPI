using Dapper.Contrib.Extensions;

namespace AuditechAPI.Models
{
    [Table("EXERCICIO")]
    public class Exercicio
    {
        public int idExercicio { get; set; }
        public string nomeExercicio { get; set; }
        public string descricaoExercicio { get; set; }
        public int padraoRespExercicio { get; set; }
        public int midiaIdmidia { get; set; }
        public int profissionalIdProfissional { get; set; }
    }
}