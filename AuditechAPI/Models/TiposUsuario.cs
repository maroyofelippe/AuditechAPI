using Dapper.Contrib.Extensions;

namespace AuditechAPI.Models
{
    [Table("TIPOUSUARIO")]
    public class TipoUsuario
    {        
        public int idTipoUsuario { get; set; }
        public string tipoUsuario { get; set; }
        public string descricaoUsuario { get; set; }
    }
}