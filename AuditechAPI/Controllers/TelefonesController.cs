using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using AuditechAPI.DAL;
using AuditechAPI.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AuditechAPI.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class TelefonesController : ControllerBase
    {
         private readonly IConfiguration _config;
        public TelefonesController(IConfiguration config){
            _config = config;
        }

        [HttpGet("{UserId}")]
        public IEnumerable<Telefone> ConsultarByUserId(int userId)
        //public ActionResult<Telefone> ConsultarById(int id)
        {
            using(IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                    sql.Append("select idTelefone as IdTelefone, tipoTelefone as tipoTelefone, numTelefone as numTelefone, usuarioIdUsuario as usuarioIdUsuario ");
                    sql.Append("from TELEFONE where usuarioIdusuario = @UserId ");

                return conexao.Query<Telefone>(sql.ToString(), new{UserId = userId});
           }
        }

        [HttpPost]
        public IActionResult Cadastrar(Telefone t)
        {
            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                    sql.Append("insert into TELEFONE (tipoTelefone, numTelefone, usuarioIdusuario) ");
                    sql.Append("values  (@tipoTelefone, @numTelefone, @usuarioIdUsuario) ");
                    sql.Append("select CAST(SCOPE_IDENTITY() as INT) ");
                object o = conexao.ExecuteScalar(sql.ToString(), t);

                if (o != null)
                    t.idTelefone = Convert.ToInt32(o);
            }
            return Ok(t.idTelefone);
            
            
        }




    }
}