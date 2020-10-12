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
    public class ClinicasController : ControllerBase
    {

        private readonly IConfiguration _config;
        public ClinicasController(IConfiguration config)
        {
            _config = config;
        }


        //Método usado para inserir uma nova Clínica Virtual
        // POST - http://url:5000/clinicas
        //No body da requisição:
        /*
        {
        "nomeClinica": "Clinica Fonotech",
        "enderClinica": "Rua Alcantara, 113",
        "compEnderClinica": "Etec HAS",
        "cepClinica": "02160-000",
        "cidadeClinica": "São Paulo",
        "ufClinica": "SP",
        "cnpjClinica": "12.123.123/0001-00",
        "emailClinica": "fonotech@fonotech.com.br",
        "dataAbertura": "2020-07-09",
        "statusClinica": 1
        }
        */
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

                if (o != null)
                    c.idClinicaVirtual = Convert.ToInt32(o);
            }
            return Ok(c.idClinicaVirtual);
        }

        //Método para Encerraento de uma Clínica Virtual
        // PUT = http://url:5000/clinicas/encerramento/Id
        //No body da requisição:
        /*
        {
        "dataEncerramento": "2020-10-09",
        "statusClinica": false
        }
        */
        [HttpPut("encerramento/{id}")]
        public IActionResult Alterar(Clinica pClinica, int id)
        {
            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();

                StringBuilder sql = new StringBuilder();
                sql.Append("UPDATE CLINICAVIRTUAL SET ");
                sql.Append("dataEncerramento = @dataEncerramento, statusClinica = @statusClinica ");
                sql.Append("WHERE idClinicaVirtual = @Id ");
                int linhasAfetadas = conexao.Execute(sql.ToString(), new { dataEncerramento = pClinica.dataEncerramento, statusClinica = pClinica.statusClinica, Id = id });
                return Ok(linhasAfetadas);
            }
        }


        // Método utilizado para fazer uma consulta de todas as Clínicas Virtuais no BD.
        // GET - http://url:5000/clinicas
        // Este método será usado para manutenção do sistema não estará disponível para usuário final.

        [HttpGet]
        public ContentResult ConsultarAll([FromServices] IConfiguration config)
        {
            using var conexao = new SqlConnection(config.GetConnectionString("ConexaoSomee"));
            using var cmd = conexao.CreateCommand();

            StringBuilder sql = new StringBuilder();
            sql.Append("Select idClinicaVirtual as idClinicaVirtual, nomeClinica as nomeClinica, ");
            sql.Append("enderecoClinica as enderClinica, complementoEnder as compEnderClinica, cepClinica as cepClinica, ufClinica as ufClinica, ");
            sql.Append("cnpjClinica as cnpjClinica, emailClinica as emailClinica, ");
            sql.Append("dataAbertura as dataAbertura, dataEncerramento as dataEncerramento, statusClinica as statusClinica ");
            sql.Append("FROM CLINICAVIRTUAL for JSON PATH, ROOT('CLINICAVIRTUAL') ");
            cmd.CommandText = sql.ToString();
            conexao.Open();
            string valorJSON = (string)cmd.ExecuteScalar();
            conexao.Close();
            return Content(valorJSON, "application/json");
        }

        // Método utilizado para fazer uma consulta de Clínica Virtual por Id válido
        // GET - http://url:5000/clinicas/Id

        [HttpGet("{id}")]
        public ActionResult<Clinica> ConsultarById(int id)
        {
            Clinica pClinica = null;
            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                sql.Append("Select idClinicaVirtual as idClinicaVirtual, nomeClinica as nomeClinica, ");
                sql.Append("enderecoClinica as enderClinica, complementoEnder as compEnderClinica, cepClinica as cepClinica, ufClinica as ufClinica, ");
                sql.Append("cnpjClinica as cnpjClinica, emailClinica as emailClinica, ");
                sql.Append("dataAbertura as dataAbertura, dataEncerramento as dataEncerramento, statusClinica as statusClinica ");
                sql.Append("FROM CLINICAVIRTUAL where idClinicaVirtual = @Id ");
                pClinica = conexao.QueryFirstOrDefault<Clinica>(sql.ToString(), new { Id = id });

                if (pClinica != null)
                    return pClinica;
                else
                    return NotFound("Clínica Virtual não encontrada");
            }
        }


    }
}