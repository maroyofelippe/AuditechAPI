using System;
using System.Data;
using System.Text;
using AuditechAPI.DAL;
using AuditechAPI.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace AuditechAPI.Controllers
{

    [ApiController]
    [Route("[Controller]")]
    public class MidiasController : ControllerBase
    {
                private readonly IConfiguration _config;
        public MidiasController(IConfiguration config)
        {
            _config = config;
        }

        // Método utilizado para fazer uma consulta de todas as Mídias cadastrados no BD.
        // GET - http://url:5000/midias
        // Este método será usado para manutenção do sistema não estará disponível para usuário final.

        [HttpGet]
        public ContentResult ConsultarAll([FromServices] IConfiguration config)
        {
            using var conexao = new SqlConnection(config.GetConnectionString("ConexaoSomee"));
            using var cmd = conexao.CreateCommand();

            StringBuilder sql = new StringBuilder();
            sql.Append("Select idMidia as idMidia, descricaoMIDIA as descricaoMidia, ");
            sql.Append("midiaPath as pathMidia ");
            sql.Append("FROM MIDIA for JSON PATH, ROOT('MIDIA') ");
            cmd.CommandText = sql.ToString();
            conexao.Open();
            string midiaJSON = (string)cmd.ExecuteScalar();
            conexao.Close();
            return Content(midiaJSON, "application/json");
        }

        // Método será utilizado para inserir uma nova Mídia:
        // Para utilizar o método deverá ser usado:
        // POST - http://url:5000/midias - e no Body da mensagem:
        /*
            {
            "pathMidia": "xxx:",
            "descricaoMidia": "xxx:"
            }
        */
        [HttpPost]
        public IActionResult Cadastrar(Midia m)
        {
            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                sql.Append("INSERT INTO MIDIA (midiaPath, descricaoMIDIA) ");
                sql.Append("values (@pathMidia, @descricaoMidia) ");
                sql.Append("SELECT CAST(SCOPE_IDENTITY() AS INT) ");
                object o = conexao.ExecuteScalar(sql.ToString(), m);

                if (o != null)
                    m.idMidia = Convert.ToInt32(o);
            }
            return Ok(m.idMidia);
        }

    }
}