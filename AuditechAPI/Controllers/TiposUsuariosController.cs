using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using System;
using AuditechAPI.Models;
using AuditechAPI.DAL;
using System.Data;

namespace AuditechAPI.Controllers
{

    [ApiController]
    [Route("[Controller]")]
    public class TiposUsuariosController : ControllerBase
    {
        private readonly IConfiguration _config;
        public TiposUsuariosController(IConfiguration config){
            _config = config;
        }

        [HttpGet]
        public ContentResult ConsultarAll([FromServices]IConfiguration config)
        {
            using var conexao = new SqlConnection(config.GetConnectionString("ConexaoSomee"));
            using var cmd = conexao.CreateCommand();

            StringBuilder sql = new StringBuilder();
                sql.Append("select idTipoUsuario as Id, tipoUsuario as 'Tipo de Usuario', descricaoUsuario as 'Descricao Usuario' ");
                sql.Append("from TIPOUSUARIO for JSON PATH, ROOT('TIPOUSUARIO') ");
            cmd.CommandText = sql.ToString();
            conexao.Open();
                string tipoUsuarioJSON = (string)cmd.ExecuteScalar();
            conexao.Close();
            return Content(tipoUsuarioJSON, "application/json");            
        }

        [HttpPost]
        public IActionResult Cadastrar(TipoUsuario p)
        {
            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                    sql.Append("insert into TIPOUSUARIO (tipoUsuario, descricaoUsuario) ");
                    sql.Append("values  (@tipoUsuario, @descricaoUsuario) ");
                    sql.Append("select CAST(SCOPE_IDENTITY() AS INT) ");
                object o = conexao.ExecuteScalar(sql.ToString(), p);

                if(o != null)
                    p.idTipoUsuario = Convert.ToInt32(o);
            }
            return Ok(p.idTipoUsuario);
        }

        [HttpPut]
        public IActionResult Alterar(TipoUsuario p)
        {
            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();

                StringBuilder sql = new StringBuilder();
                    sql.Append("update TIPOUSUARIO SET ");
                    sql.Append("tipoUsuario = @tipoUsuario, descricaoUsuario = @descricaoUsuario ");
                    sql.Append("where idTipoUsuario = @idTipoUsuario ");
                int linhasAfetadas = conexao.Execute(sql.ToString(), p);
                return Ok(linhasAfetadas);
            }
        }

    }
}