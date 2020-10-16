using System;
using System.Collections.Generic;
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
    public class TreinamentoFasesController : ControllerBase
    {

        private readonly IConfiguration _config;
        public TreinamentoFasesController(IConfiguration config)
        {
            _config = config;
        }

        // Método será utilizado para inserir o resultado de um treinamento em uma Fase:
        // Para utilizar o método deverá ser usado:
        // POST - http://url:5000/treinamentofases - e no Body da mensagem:
        /*
        {
        "idTreinamentoFase": "xxx",
        "respostaTreino": "xxx",
        "dataExecucao": "xxx",
        "faseIdFase": "xxx"
        }
        */
        [HttpPost]
        public IActionResult Cadastrar(TreinamentoFase tf)
        {
            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                sql.Append("INSERT INTO TREINAMENTOFASE (repostaTreino, dataExecucao, faseIDfase) ");
                sql.Append("VALUES (@respostaTreino, @dataExecucao, @faseIdFase ) ");
                sql.Append("SELECT CAST(SCOPE_IDENTITY() AS INT) ");
                object o = conexao.ExecuteScalar(sql.ToString(), tf);
                if (o != null)
                    tf.idTreinamentoFase = Convert.ToInt32(o);
            }
            return Ok(tf.idTreinamentoFase);
        }

        // Método utilizado para fazer uma consulta de todas as Fases cadastradas no BD.
        // GET - http://url:5000/treinamentofases
        // Este método será usado para manutenção do sistema não estará disponível para usuário final.

        [HttpGet]
        public ContentResult ConsultarAll([FromServices] IConfiguration config)
        {
            using var conexao = new SqlConnection(config.GetConnectionString("ConexaoSomee"));
            using var cmd = conexao.CreateCommand();

            StringBuilder sql = new StringBuilder();
            sql.Append("Select idTreinamentoFase as idTreinamentoFase, respostaTreino as respostaTreino, dataExecucao as dataExecucao, faseIDfase as faseIdFase, ");
            sql.Append("FROM TREINAMENTOFASE for JSON PATH, ROOT('TREINAMENTOFASE') ");
            cmd.CommandText = sql.ToString();
            conexao.Open();
            string exercicioJSON = (string)cmd.ExecuteScalar();
            conexao.Close();
            return Content(exercicioJSON, "application/json");
        }

        // Método utilizado para fazer uma consulta de um treinamento em uma Fase com Id válido
        // GET - http://url:5000/treinamentofases/fase/Id

        [HttpGet("fase/{id}")]
        public IEnumerable<TreinamentoFase> ConsultarByFaseId(int id)
        {
            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                sql.Append("Select idTreinamentoFase as idTreinamentoFase, respostaTreino as respostaTreino, dataExecucao as dataExecucao, faseIDfase as faseIdFase, ");
                sql.Append("FROM TREINAMENTOFASE where faseIDfase = @faseIdFase ");

                return conexao.Query<TreinamentoFase>(sql.ToString(), new { faseIdFase = id});
            }
        }
    }
}