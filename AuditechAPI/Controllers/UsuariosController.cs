using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using System.Data;
using System.Text;
using Microsoft.Extensions.Configuration;
using AuditechAPI.Models;
using AuditechAPI.DAL;

namespace AuditechAPI.Controllers
{

    [ApiController]
    [Route("[Controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IConfiguration _config;

        public UsuariosController (IConfiguration config){
            _config = config;
        }

        [HttpGet]
        public IEnumerable<Usuario> GetAll()
        {
            using(IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();

                StringBuilder sql = new StringBuilder();
                sql.Append("Select IdUsuario as ID, tipoUsuarioIdTipoUsuario as 'Tipo de Usuário', ");
                sql.Append("nomeUsuario as Nome, cpfUsuario as CPF, dtNascimentoUsuario as 'Data Nascimento' ");
                sql.Append("from USUARIO ");

                return conexao.Query<Usuario>(sql.ToString());
            }                       
        }
    }
}