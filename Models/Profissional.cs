using Dapper.Contrib.Extensions;

namespace AuditechAPI.Models
{
    [Table("PROFISSIONAL")]
    public class Profissional
    {
        public int idProfissional { get; set; }
        public string numOrdemProfissional { get; set; }
        public int clinicaIdClinica { get; set; }
        public int usuarioIdUsuario { get; set; }
    }

}