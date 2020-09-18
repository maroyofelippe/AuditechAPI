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
    public class ProfissionaisController : ControllerBase
    {

        private readonly IConfiguration _config;
        public ProfissionaisController(IConfiguration config)
        {
            _config = config;
        }


        //Método para inclusão de Profissional (Complemento de Infos para Usuário)
        // POST - http://url:5000/profissionais
        //No body carregar as informações.
        /*
        {
         "numOrdemProfissional": "CREFONO-001",
         "clinicaIdClinica": 5,
         "usuarioIdUsuario": 10		
        }
        */
        [HttpPost]
        public IActionResult Cadastrar(Profissional pProfissional)
        {
            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                sql.Append("insert into PROFISSIONAL (numOrdemProfissional, clinicaIDclinica, usuarioIdusuario) ");
                sql.Append("values (@numOrdemProfissional, @clinicaIdClinica, @usuarioIdUsuario) ");
                sql.Append("select cast(scope_identity() as int) ");
                object o = conexao.ExecuteScalar(sql.ToString(), pProfissional);

                if (o != null)
                    pProfissional.idProfissional = Convert.ToInt32(o);
            }
            return Ok(pProfissional.idProfissional);
        }

        //Método para Consulta de Todos os Profissionais no BD
        // GET - http://url:5000/profissionais
        // Será usada apenas para fins de admin da plataforma.
        [HttpGet]
        public ContentResult ConsultarAll([FromServices] IConfiguration config)
        {
            using var conexao = new SqlConnection(config.GetConnectionString("ConexaoSomee"));
            using var cmd = conexao.CreateCommand();

            StringBuilder sql = new StringBuilder();
            sql.Append("Select idProfissional as idProfissional, numOrdemProfissional as numOrdemProfissional, clinicaIDclinica as clinicaIdClinica, usuarioIdusuario as usuarioIdUsuario ");
            sql.Append("from PROFISSIONAL for JSON PATH, ROOT('PROFISSIONAL') ");
            cmd.CommandText = sql.ToString();
            conexao.Open();
            string profissionalJSON = (string)cmd.ExecuteScalar();
            conexao.Close();
            return Content(profissionalJSON, "application/json");
        }

        // Método utilizado para fazer uma consulta de profissional por Id válido
        // GET - http://url:5000/profissionais/Id
        [HttpGet("{id}")]
        public ActionResult<Profissional> ConsultarById(int id)
        {
            Profissional p = null;
            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                sql.Append("Select idProfissional as idProfissional, numOrdemProfissional as numOrdemProfissional, clinicaIDclinica as clinicaIdClinica, usuarioIdusuario as usuarioIdUsuario ");
                sql.Append("from PROFISSIONAL where idProfissional = @idProfissional ");
                p = conexao.QueryFirstOrDefault<Profissional>(sql.ToString(), new { idProfissional = id});

                if (p != null)
                    return p;
                else
                    return NotFound("Usuário não encontrado");
            }
        }

        // Método utilizado para fazer uma consulta de profissional por Id  de usuárioválido
        // GET - http://url:5000/profissionais/usuario/Id
        [HttpGet("usuario/{id}")]
        public ActionResult<Profissional> ConsultarByUserId(int id)
        {
            Profissional p = null;
            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                sql.Append("Select idProfissional as idProfissional, numOrdemProfissional as numOrdemProfissional, clinicaIDclinica as clinicaIdClinica, usuarioIdusuario as usuarioIdUsuario ");
                sql.Append("from PROFISSIONAL where usuarioIdusuario = @usuarioIdUsuario ");
                p = conexao.QueryFirstOrDefault<Profissional>(sql.ToString(), new { usuarioIdUsuario = id});

                if (p != null)
                    return p;
                else
                    return NotFound("Usuário não encontrado");
            }
        }

        //Método para alterar Num Ordem Profissional ou Id Clinica
        // PUT = http://url:5000/profissionais/usuario/Id
        //No body da requisição:
        /*
        {
        "numOrdemProfissional": "CREFONO-001",
        "clinicaIdClinica": 5
        }
        */
        [HttpPut("usuario/{id}")]
        public IActionResult Alterar(Profissional pProfissional, int id)
        {
            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                sql.Append("update PROFISSIONAL set ");
                sql.Append("numOrdemProfissional = @numOrdemProfissional, clinicaIDclinica = @clinicaIdClinica ");
                sql.Append("where usuarioIdusuario = @usuarioIdUsuario ");
                int linhasAfetadas = conexao.Execute(sql.ToString(), new {numOrdemProfissional = pProfissional.numOrdemProfissional, clinicaIDclinica = pProfissional.clinicaIdClinica, usuarioIdUsuario = id});
                return Ok(linhasAfetadas);
            }
        }
    }
}