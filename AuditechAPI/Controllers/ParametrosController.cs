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
    public class ParametrosController : ControllerBase
    {
                private readonly IConfiguration _config;
        public ParametrosController(IConfiguration config)
        {
            _config = config;
        }

        // Método utilizado para fazer uma consulta de todos os Parâmetros cadastrados no BD.
        // GET - http://url:5000/parametros
        // Este método será usado para manutenção do sistema não estará disponível para usuário final.

        [HttpGet]
        public ContentResult ConsultarAll([FromServices] IConfiguration config)
        {
            using var conexao = new SqlConnection(config.GetConnectionString("ConexaoSomee"));
            using var cmd = conexao.CreateCommand();

            StringBuilder sql = new StringBuilder();
            sql.Append("Select idParametro as idParametro, nomeParametro as nomeParametro, ");
            sql.Append("descricaoParametro as descricaoParametro, valMinParametro as vlMinParametro, valMaxParametro as vlMaxParametro, valDefaultParametro as vlDefaultParametro ");
            sql.Append("from PARAMETRO for JSON PATH, ROOT('PARAMETRO') ");
            cmd.CommandText = sql.ToString();
            conexao.Open();
            string parametroJSON = (string)cmd.ExecuteScalar();
            conexao.Close();
            return Content(parametroJSON, "application/json");
        }

                [HttpGet("{id}")]
        public ActionResult<Parametro> ConsultarById(int id)
        {
            Parametro param = null;

            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                sql.Append("Select idParametro as idParametro, nomeParametro as nomeParametro, ");
                sql.Append("descricaoParametro as descricaoParametro, valMinParametro as vlMinParametro, valMaxParametro as vlMaxParametro, valDefaultParametro as vlDefaultParametro ");
                sql.Append("from PARAMETRO where idParametro = @idParametro ");
                param = conexao.QueryFirstOrDefault<Parametro>(sql.ToString(), new { idParametro = id });

                if (param != null)
                    return param;
                else
                    return NotFound("Parametro não encontrado");
            }
        }

        // Método será utilizado para inserir um novo parâmetro:
        // Para utilizar o método deverá ser usado:
        // POST - http://url:5000/parametros - e no Body da mensagem:
        /*
            {
            "nomeParametro": "xxx:",
            "descricaoParametro": "xxx:",
            "vlMinParametro": "xxx:",
            "vlMaxParametro": "xxx:",
            "vlDefaultParametro": "xxx:"
            }
        */
        [HttpPost]
        public IActionResult Cadastrar(Parametro param)
        {
            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                sql.Append("INSERT INTO PARAMETRO (nomeParametro, descricaoParametro, valMinParametro, valMaxParametro, valDefaultParametro) ");
                sql.Append("values (@nomeParametro, @descricaoParametro, @vlMinParametro, @vlMaxParametro, @vlDefaultParametro) ");
                sql.Append("SELECT CAST(SCOPE_IDENTITY() AS INT) ");
                object o = conexao.ExecuteScalar(sql.ToString(), param);
                if (o != null)
                param.idParametro = Convert.ToInt32(o);
            }
            return Ok(param.idParametro);
        }

        // Após uma consulta, é possível fazer a alteração de um dado do Parâmetro e fazer o update
        // PUT - http://url:5000/parametros
        // No body deve-se passar os parâmetros:
        /*
            {
            "idParametro": "xxx:",
            "nomeParametro": "xxx:",
            "descricaoParametro": "xxx:",
            "vlMinParametro": "xxx:",
            "vlMaxParametro": "xxx:",
            "vlDefaultParametro": "xxx:"
            }
        */
        [HttpPut]
        public IActionResult Alterar(Parametro param)
        {
            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();

                StringBuilder sql = new StringBuilder();
                sql.Append("UPDATE PARAMETRO SET ");
                sql.Append("nomeParametro = @nomeParametro, descricaoParametro = @descricaoParametro, valMinParametro = @vlMinParametro, valMaxParametro = @vlMaxParametro, valDefaultParametro = @vlDefaultParametro ");
                sql.Append("WHERE idParametro = @idParametro ");
                int linhasAfetadas = conexao.Execute(sql.ToString(), param);
                return Ok(linhasAfetadas);
            }
        }

    }
}