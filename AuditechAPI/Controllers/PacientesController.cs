using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace AuditechAPI.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class PacientesController : ControllerBase
    {
        private readonly IConfiguration _config;
        public PacientesController(IConfiguration config){
            _config = config;
        }


        [HttpGet] 
        public ContentResult GetAll([FromServices]IConfiguration config)
        {
            using var conexao = new SqlConnection(config.GetConnectionString("ConexaoSomee"));
            using var cmd = conexao.CreateCommand();

            StringBuilder sql = new StringBuilder();
                sql.Append("select * from PACIENTE ");
            cmd.CommandText = sql.ToString();
            conexao.Open();
                string pacienteJSON = (string)cmd.ExecuteScalar();
            conexao.Close();
            return Content(pacienteJSON, "application/json");
            
        }



    }
}