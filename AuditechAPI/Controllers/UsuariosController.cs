using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Data;
using Dapper;
using AuditechAPI.Models;
using System.Collections.Generic;


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

        public ContentResult GetAll([FromServices]IConfiguration config)
        {
            using var conexao = new SqlConnection(config.GetConnectionString("ConexaoSomee"));
            using var cmd = conexao.CreateCommand();

             StringBuilder sql = new StringBuilder();
                sql.Append("Select IdUsuario as ID, tipoUsuarioIdTipoUsuario as 'Tipo de Usuário', ");
                sql.Append("nomeUsuario as Nome, cpfUsuario as CPF, dtNascimentoUsuario as 'Data Nascimento' ");
                sql.Append("from USUARIO for JSON PATH, ROOT('Usuario') ");

            cmd.CommandText = sql.ToString();

            conexao.Open();
            string valorJSON = (string)cmd.ExecuteScalar();
            conexao.Close();

            return Content(valorJSON, "application/json");
            
        }

         [HttpGet("{id}")]

        public ActionResult<Usuario> GetById(int id)
        {
            Usuario p = null;
            using(IDbConnection conexao = DAL.ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();

                StringBuilder sql = new StringBuilder();
                sql.Append("Select IdUsuario as ID, tipoUsuarioIdTipoUsuario as 'Tipo de Usuário', ");
                sql.Append("nomeUsuario as Nome, cpfUsuario as CPF, dtNascimentoUsuario as 'Data Nascimento' ");
                sql.Append("from USUARIO where IdUsuario = @id for JSON PATH, ROOT('Usuario') ");

                p = conexao.QueryFirstOrDefault<Usuario>(sql.ToString(), new {Id = id});
                             
                if(p != null)
                    return p;
                else
                    return NotFound("Usuário não encontrado");
            }
        }
    }
}