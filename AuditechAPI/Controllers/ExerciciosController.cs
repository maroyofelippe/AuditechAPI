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
    public class ExerciciosController : ControllerBase
    {

        
        private readonly IConfiguration _config;
        public ExerciciosController(IConfiguration config)
        {
            _config = config;
        }

        // Método utilizado para fazer uma consulta de todo os Exercícios cadastrados no BD.
        // GET - http://url:5000/exercicios
        // Este método será usado para manutenção do sistema não estará disponível para usuário final.

        [HttpGet]
        public ContentResult ConsultarAll([FromServices] IConfiguration config)
        {
            using var conexao = new SqlConnection(config.GetConnectionString("ConexaoSomee"));
            using var cmd = conexao.CreateCommand();

            StringBuilder sql = new StringBuilder();
            sql.Append("Select idExercicio as idExercicio, nomeExercicio as nomeExercicio, ");
            sql.Append("descricaoExercicio as descricaoExercicio, padraoResposta as padraoRespExercicio, midiaIDmidia as midiaIdMidia ");
            sql.Append("FROM EXERCICIO for JSON PATH, ROOT('EXERCICIO') ");
            cmd.CommandText = sql.ToString();
            conexao.Open();
            string exercicioJSON = (string)cmd.ExecuteScalar();
            conexao.Close();
            return Content(exercicioJSON, "application/json");
        }

        // Método utilizado para fazer uma consulta de um Exercíciocom Id válido
        // GET - http://url:5000/exercicios/Id

        [HttpGet("{id}")]
        public ActionResult<Exercicio> ConsultarById(int id)
        {
            Exercicio e = null;

            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                sql.Append("Select idExercicio as idExercicio, nomeExercicio as nomeExercicio, ");
                sql.Append("descricaoExercicio as descricaoExercicio, padraoResposta as padraoRespExercicio, midiaIDmidia as midiaIdMidia ");
                sql.Append("from EXERCICIO where idExercicio = @idExercicio ");
                e = conexao.QueryFirstOrDefault<Exercicio>(sql.ToString(), new { idExercicio = id });

                if (e != null)
                    return e;
                else
                    return NotFound(string.Format("Exercício com o ID: {0} não encontrado", id));
            }
        }

        // Método será utilizado para inserir um novo Exercício:
        // Para utilizar o método deverá ser usado:
        // POST - http://url:5000/exercicios - e no Body da mensagem:
        /*
            {
            "nomeExercicio": "xxx:",
            "descricaoExercicio": "xxx:",
            "padraoRespExercicio": "xxx:",
            "midiaIdmidia": "xxx:"
            }
        */
        [HttpPost]
        public IActionResult Cadastrar(Exercicio e)
        {
            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                sql.Append("INSERT INTO EXERCICIO (nomeExercicio, descricaoExercicio, padraoResposta, midiaIDmidia) ");
                sql.Append("values (@nomeExercicio, @descricaoExercicio, @padraoRespExercicio, @midiaIdMidia) ");
                sql.Append("SELECT CAST(SCOPE_IDENTITY() AS INT) ");
                object o = conexao.ExecuteScalar(sql.ToString(), e);

                if (o != null)
                    e.idExercicio = Convert.ToInt32(o);
            }
            return Ok(e.idExercicio);
        }

        // Após uma consulta, é possível fazer a alteração de um dado do Exercício e fazer o update
        // PUT - http://url:5000/exercicios
        // E no body da requisição deve-se enviar:
        /*
            {
            "idExercicio": "xxx:",
            "nomeExercicio": "xxx:",
            "descricaoExercicio": "xxx:",
            "padraoRespExercicio": "xxx:",
            "midiaIdmidia": "xxx:"
            }
            */
        [HttpPut]
        public IActionResult Alterar(Exercicio e)
        {
            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();

                StringBuilder sql = new StringBuilder();
                sql.Append("UPDATE EXERCICIO SET ");
                sql.Append("nomeExercicio = @nomeExercicio, descricaoExercicio = @descricaoExercicio, padraoResposta = @padraRespExercicio, midiaIDmidia = @midiaIdMidia ");
                sql.Append("WHERE idExercicio = @idExercicio ");
                int linhasAfetadas = conexao.Execute(sql.ToString(), e);
                return Ok(linhasAfetadas);
            }
        }

    }
}