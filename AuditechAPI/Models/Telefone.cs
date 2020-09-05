using System;
using System.Collections.Generic;
using Dapper;
using Dapper.Contrib.Extensions;

namespace AuditechAPI.Models
{
    [Table("TELEFONE")]
    public class Telefone
    {
        public int idTelefone { get; set; }
        public int tipoTelefone { get; set; }
        public string numTelefone { get; set; }
        public int usuarioIdUsuario { get; set; }

    }
}