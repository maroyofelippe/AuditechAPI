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
    public class ResultadoFasesController : ControllerBase
    {

        private readonly IConfiguration _config;
        public ResultadoFasesController(IConfiguration config)
        {
            _config = config;
        }

        // Método utilizado para fazer uma consulta do Resultado de uma fase pelo Id da Fase
        // GET - http://url:5000/resultadofases/fase/Id


        [HttpGet("fase/{id}")]
        public ActionResult<ResultadoFase> ConsultarByFaseId(int id)
        {
            ResultadoFase rf = null;

            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                sql.Append("Select idResultadoFase as idResultadoFase, dataTermino as dataTermino, ");
                sql.Append("faseIDfase as faseIdFase, pacienteIDpaciente as pacienteIdPaciente, qtdeResultadoFase as qtdeResultadoFase, ");
                sql.Append("resultadoFase as resultadoFase, resultadoErros as resultadoErros, resultadoNR as resultadoNR ");
                sql.Append("from RESULTADOFASE where faseIDfase = @faseIdFase ");
                rf = conexao.QueryFirstOrDefault<ResultadoFase>(sql.ToString(), new { faseIdFase = id });

                if (rf != null)
                    return rf;
                else
                    return NotFound("Não existe Nenhum resultado para esta Fase Id");
            }
        }
    }
}