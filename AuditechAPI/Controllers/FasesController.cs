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
    public class FasesController : ControllerBase
    {

        private readonly IConfiguration _config;
        public FasesController(IConfiguration config)
        {
            _config = config;
        }

        // Método utilizado para fazer uma consulta de todas as Fases cadastradas no BD.
        // GET - http://url:5000/fases
        // Este método será usado para manutenção do sistema não estará disponível para usuário final.

        [HttpGet]
        public ContentResult ConsultarAll([FromServices] IConfiguration config)
        {
            using var conexao = new SqlConnection(config.GetConnectionString("ConexaoSomee"));
            using var cmd = conexao.CreateCommand();

            StringBuilder sql = new StringBuilder();
            sql.Append("Select idFase as idFase, dataInicio as dataInicio, dataFinal as dataFinal, ");
            sql.Append("numDias as numDias, qtdeTreinoDia as qtdeTreinoDias, intervaloTreinoHora as intervaloTreinoHora, ");
            sql.Append("pesoTreino as pesoTreino, pesoDesafio as pesoDesafio, ");
            sql.Append("exercicioIDexercicio as exercicioIdExercicio, tratamentoIDtratamento as tratamentoIdTratamento ");
            sql.Append("FROM FASE for JSON PATH, ROOT('FASE') ");
            cmd.CommandText = sql.ToString();
            conexao.Open();
            string exercicioJSON = (string)cmd.ExecuteScalar();
            conexao.Close();
            return Content(exercicioJSON, "application/json");
        }

        // Método utilizado para fazer uma consulta de uma Fase com Id válido
        // GET - http://url:5000/fases/Id

        [HttpGet("{id}")]
        public ActionResult<Fase> ConsultarById(int id)
        {
            Fase f = null;

            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                sql.Append("Select idFase as idFase, dataInicio as dataInicio, dataFinal as dataFinal, ");
                sql.Append("numDias as numDias, qtdeTreinoDia as qtdeTreinoDia, intervaloTreinoHora as intervaloTreinoHora, ");
                sql.Append("pesoTreino as pesoTreino, pesoDesafio as pesoDesafio, ");
                sql.Append("exercicioIDexercicio as exercicioIdExercicio, tratamentoIDtratamento as tratamentoIdTratamento ");
                sql.Append("FROM FASE as fase where idFase = @idFase ");
                /*sql.Append("INNER JOIN TRATAMENTO as t ON t.idTratamento = fase.tratamentoIDtratamento ");*/
                    f = conexao.QueryFirstOrDefault<Fase>(sql.ToString(), new { idFase = id });
                if (f != null)
                    return f;
                else
                    return NotFound(string.Format("Fase com o ID: {0} não encontrado", id));
            }
        }

        // Método utilizado para fazer uma consulta de uma Fase com Id de tratamento válido
        // GET - http://url:5000/fases/tratamento/Id

        [HttpGet("tratamento/{id}")]
        public ActionResult<Fase> ConsultarByTratamentoId(int id)
        {
            Fase f = null;

            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                sql.Append("Select idFase as idFase, dataInicio as dataInicio, dataFinal as dataFinal, ");
                sql.Append("numDias as numDias, qtdeTreinoDia as qtdeTreinoDia, intervaloTreinoHora as intervaloTreinoHora, ");
                sql.Append("pesoTreino as pesoTreino, pesoDesafio as pesoDesafio, ");
                sql.Append("exercicioIDexercicio as exercicioIdExercicio, tratamentoIDtratamento as tratamentoIdTratamento ");
                sql.Append("FROM FASE where tratamentoIDtratamento = @tratamentoIdTratamento ");
                    f = conexao.QueryFirstOrDefault<Fase>(sql.ToString(), new { tratamentoIdTratamento = id });
                if (f != null)
                    return f;
                else
                    return NotFound(string.Format("Fase com o Tratamento ID: {0} não encontrado", id));
            }
        }

        // Método utilizado para fazer uma consulta de uma Fase com Id de tratamento válido
        // GET - http://url:5000/fases/tratamento/Id

        [HttpGet("usuario/{id}")]
        public ActionResult<Fase> ConsultarByUsuarioId(int id)
        {
            Fase f = null;

            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                sql.Append("Select idFase as idFase, dataInicio as dataInicio, dataFinal as dataFinal, ");
                sql.Append("numDias as numDias, qtdeTreinoDia as qtdeTreinoDia, intervaloTreinoHora as intervaloTreinoHora, ");
                sql.Append("pesoTreino as pesoTreino, pesoDesafio as pesoDesafio, ");
                sql.Append("exercicioIDexercicio as exercicioIdExercicio, tratamentoIDtratamento as tratamentoIdTratamento ");
                sql.Append("FROM FASE where tratamentoIDtratamento = (SELECT idTratamento FROM TRATAMENTO WHERE pacienteIDpaciente = ");
                sql.Append("(SELECT idPaciente FROM PACIENTE WHERE usuarioIdusuario = @usuarioIdUsuario)) ");
                    f = conexao.QueryFirstOrDefault<Fase>(sql.ToString(), new { usuarioIdUsuario = id });
                if (f != null)
                    return f;
                else
                    return NotFound(string.Format("Fase com o Tratamento ID: {0} não encontrado", id));
            }
        }

        // Método será utilizado para inserir uma nova Fase:
        // Para utilizar o método deverá ser usado:
        // POST - http://url:5000/fases - e no Body da mensagem:
        /*
        {
        "idFase": "xxx",
        "dataInicio": "xxx",
        "dataFinal": "xxx",
        "numDias": "xxx",
        "qtdeTreinoDia": "xxx",
        "intervaloTreinoHora": "xxx",
        "pesoTreino": "xxx",
        "pesoDesafio": "xxx",
        "exercicioIdExercicio": "xxx",
        "tratamentoIdTratamento": "xxx"
        }
        */
        [HttpPost]
        public IActionResult Cadastrar(Fase f)
        {
            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                sql.Append("INSERT INTO FASE (dataInicio, dataFinal, numDias, qtdeTreinoDia, intervaloTreinoHora, pesoTreino, pesoDesafio, exercicioIDexercicio, tratamentoIDtratamento) ");
                sql.Append("VALUES (@dataInicio, @dataFinal, @numDias, @qtdeTreinoDia, @intervaloTreinoHora, @pesoTreino, @pesoDesafio, @exercicioIdExercicio, @tratamentoIdTratamento ) ");
                sql.Append("SELECT CAST(SCOPE_IDENTITY() AS INT) ");
                object o = conexao.ExecuteScalar(sql.ToString(), f);
                if (o != null)
                    f.idFase = Convert.ToInt32(o);
            }
            return Ok(f.idFase);
        }


    }
}