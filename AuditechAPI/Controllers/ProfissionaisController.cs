using System;
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
    }
}