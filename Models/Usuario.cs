using Dapper.Contrib.Extensions;

namespace AuditechAPI.Models
{
    [Table("USUARIO")]

    public class Usuario
    {
        public int IdUsuario { get; set; }
        public int idTipoUsuario { get; set; }
        public string nome { get; set; }
        public string cpf { get; set; }
        public string dataNascimento { get; set; }
    }
}