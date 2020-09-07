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
    public class ClinicasController : ControllerBase
    {

         private readonly IConfiguration _config;
        public ClinicasController(IConfiguration config){
            _config = config;
        }

        [HttpPost]
        public IActionResult Cadastrar(Clinica c)
        {
            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                    sql.Append("INSERT INTO CLINICAVIRTUAL (nomeClinica, enderecoClinica, complementoEnder, cepClinica, cidadeClinica, ufClinica, cnpjClinica, emailClinica, dataAbertura, statusClinica) ");
                    sql.Append("values (@nomeClinica, @enderClinica, @compEnderClinica, @cepClinica, @cidadeClinica, @ufClinica, @cnpjClinica, @emailClinica, @dataAbertura, @statusClinica) ");
                    sql.Append("SELECT CAST(SCOPE_IDENTITY() AS INT) ");
                object o = conexao.ExecuteScalar(sql.ToString(), c);

                if(o != null)
                    c.idClinicaVirtual = Convert.ToInt32(o);                
            }
            return Ok(c.idClinicaVirtual);
        }

    }
}