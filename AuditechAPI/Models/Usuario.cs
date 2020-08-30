namespace AuditechAPI.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public int tipoIdUsuario { get; set; }
        public string nomeUsuario { get; set; }
        public string cpfUsuario { get; set; }
        public string dataNascimetoUsuario { get; set; }
    }
}