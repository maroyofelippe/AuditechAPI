using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Data;
using Dapper;
using AuditechAPI.Models;
using AuditechAPI.DAL;
using System.Collections.Generic;
using System;

namespace AuditechAPI.Controllers
{

    [ApiController]
    [Route("[Controller]")]
    public class UsuariosController : ControllerBase
    {       
         private readonly IConfiguration _config;
        public UsuariosController(IConfiguration config){
            _config = config;
        }

        [HttpGet]
        public ContentResult GetAll([FromServices]IConfiguration config)
        {
            using var conexao = new SqlConnection(config.GetConnectionString("ConexaoSomee"));
            using var cmd = conexao.CreateCommand();

            StringBuilder sql = new StringBuilder();
                sql.Append("Select IdUsuario as Id, tipoUsuarioIdTipoUsuario as 'Tipo Usuário', ");
                sql.Append("nomeUsuario as Nome, cpfUsuario as CPF, dtNascimentoUsuario as 'Data Nascimento' ");
                sql.Append("FROM USUARIO for JSON PATH, ROOT('USUARIO') ");
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
            
            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                    sql.Append("Select IdUsuario as IdUsuario, tipoUsuarioIdTipoUsuario as idTipoUsuario, ");
                    sql.Append("nomeUsuario as nome, cpfUsuario as cpf, dtNascimentoUsuario as dataNascimento ");
                    sql.Append("from USUARIO where IdUsuario = @Id ");
                p = conexao.QueryFirstOrDefault<Usuario>(sql.ToString(), new {Id = id});
                             
                if(p != null)
                    return p;
                else
                    return NotFound("Usuário não encontrado");
            }
        }

        [HttpPost]
        public IActionResult Inserir(Usuario p)
        {
            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                    sql.Append("INSERT INTO USUARIO (tipoUsuarioIdTipoUsuario, nomeUsuario, cpfUsuario, dtNascimentoUsuario) ");
                    sql.Append("values (@idTipoUsuario, @nome, @cpf, @dataNascimento) ");
                    sql.Append("SELECT CAST(SCOPE_IDENTITY() AS INT) ");
                object o = conexao.ExecuteScalar(sql.ToString(), p);

                if(o != null)
                    p.IdUsuario = Convert.ToInt32(o);                
            }
            return Ok(p.IdUsuario);
        }

        [HttpPut]
        public IActionResult Alterar(Usuario p)
        {
            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();

                StringBuilder sql = new StringBuilder();
                    sql.Append("UPDATE USUARIO SET ");
                    sql.Append("tipoUsuarioIdTipoUsuario = @idTipoUsuario, nomeUsuario = @nome, cpfUsuario = @cpf, dtNascimentoUsuario = @dataNascimento ");
                    sql.Append("WHERE IdUsuario = @IdUsuario ");
                int linhasAfetadas = conexao.Execute(sql.ToString(), p);
                return Ok(linhasAfetadas);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                    sql.Append("DELETE FROM USUARIO ");
                    sql.Append("WHERE idUsuario = @Id ");

                int linhasAfetadas = conexao.Execute(sql.ToString(), new {Id = id});
                return Ok(linhasAfetadas);
            }
        }                
    }
}