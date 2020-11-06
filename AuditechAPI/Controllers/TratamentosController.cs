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
    public class TratamentosController : ControllerBase
    {
        private readonly IConfiguration _config;
        public TratamentosController(IConfiguration config)
        {
            _config = config;
        }

        // Método utilizado para fazer uma consulta de todo os tratamentos cadastrados no BD.
        // GET - http://url:5000/tratamentos
        // Este método será usado para manutenção do sistema não estará disponível para usuário final.

        [HttpGet]
        public ContentResult ConsultarAll([FromServices] IConfiguration config)
        {
            using var conexao = new SqlConnection(config.GetConnectionString("ConexaoSomee"));
            using var cmd = conexao.CreateCommand();

            StringBuilder sql = new StringBuilder();
            sql.Append("Select idTratamento as idTratamento, dataInicio as dataInicio, dataTermino as dataTermino, ");
            sql.Append("observacaoTratamento as observacaoTratamento, ");
            sql.Append("profissionalIDprofissional as profissionalIdProfissional, pacienteIDpaciente as pacienteIdpaciente, clinicaIDclinica as clinicaIdClinica ");
            sql.Append("FROM TRATAMENTO for JSON PATH, ROOT('TRATAMENTO') ");
            cmd.CommandText = sql.ToString();
            conexao.Open();
            string exercicioJSON = (string)cmd.ExecuteScalar();
            conexao.Close();
            return Content(exercicioJSON, "application/json");
        }

        // Método utilizado para fazer uma consulta de um Tratamentos com Id válido
        // GET - http://url:5000/tratamentos/Id

        [HttpGet("{id}")]
        public ActionResult<Tratamento> ConsultarById(int id)
        {
            Tratamento t = null;

            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                sql.Append("Select idTratamento as idTratamento, dataInicio as dataInicio, dataTermino as dataTermino, ");
                sql.Append("observacaoTratamento as observacaoTratamento, ");
                sql.Append("profissionalIDprofissional as profissionalIdProfissional, pacienteIDpaciente as pacienteIdpaciente, clinicaIDclinica as clinicaIdClinica ");
                sql.Append("FROM TRATAMENTO where idTratamento = @idTratamento ");
                    t = conexao.QueryFirstOrDefault<Tratamento>(sql.ToString(), new { idTratamento = id });

                if (t != null)
                    return t;
                else
                    return NotFound(string.Format("Tratamento com o ID: {0} não encontrado", id));
            }
        }

        // Método utilizado para fazer uma consulta de um Tratamentos com Id de Paciente válido
        // GET - http://url:5000/tratamentos/usuario/Id

        [HttpGet("usuario/{id}")]
        public ActionResult<Tratamento> ConsultarByUsuarioId(int id)
        {
            Tratamento t = null;

            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                sql.Append("Select idTratamento as idTratamento, dataInicio as dataInicio, dataTermino as dataTermino, ");
                sql.Append("observacaoTratamento as observacaoTratamento, ");
                sql.Append("profissionalIDprofissional as profissionalIdProfissional, pacienteIDpaciente as pacienteIdpaciente, clinicaIDclinica as clinicaIdClinica ");
                sql.Append("FROM TRATAMENTO where pacienteIDpaciente = (SELECT idPaciente FROM PACIENTE WHERE usuarioIdusuario = @usuarioIdUsuario) ");
                /*sql.Append("FROM TRATAMENTO where pacienteIDpaciente = @pacienteIdPaciente ");*/
                    t = conexao.QueryFirstOrDefault<Tratamento>(sql.ToString(), new { usuarioIdUsuario = id });

                if (t != null)
                    return t;
                else
                    return NotFound(string.Format("Tratamento para Paciente com o UsuárioID: {0} não encontrado", id));
            }
        }

        // Método será utilizado para inserir um novo Tratamento:
        // Para utilizar o método deverá ser usado:
        // POST - http://url:5000/tratamentos - e no Body da mensagem:
        /*
        {
        "dataInicio": "xxx",
        "observacaoTratamento": "xxx",
        "statusTratamento": true,
        "profissionalIdProfissional": "xxx",
        "pacienteIdPaciente": "xxx",
        "clinicaIdClinica": "xxx"
        }
        */
        [HttpPost]
        public IActionResult Cadastrar(Tratamento t)
        {
            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                sql.Append("INSERT INTO TRATAMENTO (dataInicio, observacaoTratamento, statusTratamento, profissionalIDprofissional, pacienteIDpaciente, clinicaIDclinica) ");
                sql.Append("values (@dataInicio, @observacaoTratamento, @statusTratamento, @profissionalIdProfissional, @pacienteIdPaciente, @clinicaIdClinica) ");
                sql.Append("SELECT CAST(SCOPE_IDENTITY() AS INT) ");
                object o = conexao.ExecuteScalar(sql.ToString(), t);

                if (o != null)
                    t.idTratamento = Convert.ToInt32(o);
            }
            return Ok(t.idTratamento);
        }

        // Após uma consulta, é possível fazer a alteração nas observações que o profissional faz em um determinado Tratamento e fazer o update
        // PUT - http://url:5000/tratamentos/observa
        // E no body da requisição deve-se enviar:
        /*
        {
        "idTratamento": "xxx",
        "observacaoTratamento": "xxx",
        }
        */
        [HttpPut("observa")]
        public IActionResult ObservarTratamento(Tratamento t)
        {
            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();

                StringBuilder sql = new StringBuilder();
                sql.Append("UPDATE TRATAMENTO SET ");
                sql.Append("observacaoTratamento = @observacaoTratamento ");
                sql.Append("WHERE idTratamento = @idTratamento ");
                int linhasAfetadas = conexao.Execute(sql.ToString(), t);
                return Ok(linhasAfetadas);
            }
        }

        // Após o término de um tratamento, é necessário que o profissional, atualize os dados do tratamento referente a um paciente ID.
        // PUT - http://url:5000/tratamentos/encerra
        // E no body da requisição deve-se enviar:
        /*
        {
        "idTratamento": "xxx",
        "dataTermino": "xxx",
        "observacaoTratamento": "xxx",
        "statusTratamento": false,
        "pacienteIdPaciente": "xxx"
        }
        */
        [HttpPut("encerra")]
        public IActionResult EncerrarTratamento(Tratamento t)
        {
            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();

                StringBuilder sql = new StringBuilder();
                sql.Append("UPDATE TRATAMENTO SET ");
                sql.Append("observacaoTratamento = @observacaoTratamento, dataTermino = @dataTermino, statusTratamento = @statusTratamento ");
                sql.Append("WHERE idTratamento = @idTratamento AND pacienteIDpaciente = @pacienteIdPaciente");
                int linhasAfetadas = conexao.Execute(sql.ToString(), t);
                return Ok(linhasAfetadas);
            }
        }

    }
}