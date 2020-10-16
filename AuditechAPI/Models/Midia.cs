using Dapper.Contrib.Extensions;

namespace AuditechAPI.Models
{
    [Table("MIDIA")]
    public class Midia
    {
        public int idMidia { get; set; }
        public string descricaoMidia { get; set; }
        public string pathMidia { get; set; }
    }
}