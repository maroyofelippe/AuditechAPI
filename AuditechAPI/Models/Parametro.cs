using Dapper.Contrib.Extensions;

namespace AuditechAPI.Models
{
    [Table("PARAMETRO")]
    public class Parametro
    {
        public int idParametro { get; set; }
        public string nomeParametro { get; set; }
        public string descricaoParametro { get; set; }
        public double vlMinParametro { get; set; }
        public double vlMaxParametro { get; set; }
        public double vlDefaultParametro { get; set; }
    }
}