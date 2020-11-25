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
    public class PacientesController : ControllerBase
    {
        private readonly IConfiguration _config;
        public PacientesController(IConfiguration config)
        {
            _config = config;
        }

        //Método para inclusão de Profissional (Complemento de Infos para Usuário)
        // POST - http://url:5000/pacientes
        //No body carregar as informações.
        /*
        {
        "statusPaciente": true,
        "nomePai": "Marco Antonio Royo Felippe",
        "cpfPai": "123.456.789-00",
        "nomeMae": "Ana Claudia Rescia Royo Felippe",
        "cpfMae": "321.654.987-99",
        "enderPaciente": "Rua Cecília Meireles, 320",
        "compEnderPaciente": "Casa 7",
        "cepPaciente": "02123-010",
        "cidadePaciente": "São Paulo",
        "ufPaciente": "SP",
        "clinicaIdClinica": 5,
        "usuarioIdusuario": 13
        }
        */
        [HttpPost]
        public IActionResult Cadastrar(Paciente pPaciente)
        {
            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                sql.Append("insert into PACIENTE (statusPaciente, nomePai, cpfPai, nomeMae, cpfMae, enderecoPaciente, complementoEnder, cepPaciente, cidadePaciente, ufPaciente, clinicaIDclinica, usuarioIdusuario) ");
                sql.Append("values (@statusPaciente, @nomePai, @cpfPai, @nomeMae, @cpfMae, @enderPaciente, @compEnderPaciente, @cepPaciente, @cidadePaciente, @ufPaciente, @clinicaIdClinica, @usuarioIdUsuario) ");
                sql.Append("select cast(scope_identity() as int) ");
                object o = conexao.ExecuteScalar(sql.ToString(), pPaciente);

                if (o != null)
                    pPaciente.idPaciente = Convert.ToInt32(o);
            }
            return Ok(pPaciente.idPaciente);
        }

        //Método para consultar todos os pacientes cadastrados.
        // GET - http://url:5000/pacientes
        // Será usado apenas para fins administrativos da plataforma.
        [HttpGet]
        public ContentResult ConsultarAll([FromServices] IConfiguration config)
        {
            using var conexao = new SqlConnection(config.GetConnectionString("ConexaoSomee"));
            using var cmd = conexao.CreateCommand();

            StringBuilder sql = new StringBuilder();
            sql.Append("select idPaciente as  idPaciente, statusPaciente as statusPaciente, ");
            sql.Append("nomePai as nomePai, cpfPai as cpfPai, nomeMae as nomeMae, cpfMae as cpfMae, ");
            sql.Append("enderecoPaciente as enderPaciente, complementoEnder as compEnderPaciente, cepPaciente as cepPaciente, cidadePaciente as cidadePaciente, ufPaciente as ufPaciente, ");
            sql.Append("clinicaIDclinica as clinicaIdClinica, usuarioIdusuario as usuarioIdususario ");
            sql.Append("from PACIENTE for JSON PATH, ROOT('PACIENTE') ");
            cmd.CommandText = sql.ToString();
            conexao.Open();
            string pacienteJSON = (string)cmd.ExecuteScalar();
            conexao.Close();
            return Content(pacienteJSON, "application/json");

        }

        // Método utilizado para fazer uma consulta de Pacientes por Id válido
        // GET - http://url:5000/pacientes/Id

        [HttpGet("{id}")]
        public ActionResult<Paciente> ConsultarById(int id)
        {
            Paciente p = null;

            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                sql.Append("select idPaciente as  idPaciente, statusPaciente as statusPaciente, ");
                sql.Append("nomePai as nomePai, cpfPai as cpfPai, nomeMae as nomeMae, cpfMae as cpfMae, ");
                sql.Append("enderecoPaciente as enderPaciente, complementoEnder as compEnderPaciente, cepPaciente as cepPaciente, cidadePaciente as cidadePaciente, ufPaciente as ufPaciente, ");
                sql.Append("clinicaIDclinica as clinicaIdClinica, usuarioIdusuario as usuarioIdusuario ");
                sql.Append("from PACIENTE where idPaciente = @idPaciente ");
                p = conexao.QueryFirstOrDefault<Paciente>(sql.ToString(), new { idPaciente = id });

                if (p != null)
                    return p;
                else
                    return NotFound("Paciente não encontrado");
            }
        }

        // Método utilizado para fazer uma consulta de Pacientes por Id de Usuário válido
        // GET - http://url:5000/pacientes/usuario/Id

        [HttpGet("usuario/{id}")]
        public ActionResult<Paciente> ConsultarByUserId(int id)
        {
            Paciente p = null;

            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                sql.Append("select idPaciente as  idPaciente, statusPaciente as statusPaciente, ");
                sql.Append("nomePai as nomePai, cpfPai as cpfPai, nomeMae as nomeMae, cpfMae as cpfMae, ");
                sql.Append("enderecoPaciente as enderPaciente, complementoEnder as compEnderPaciente, cepPaciente as cepPaciente, cidadePaciente as cidadePaciente, ufPaciente as ufPaciente, ");
                sql.Append("clinicaIDclinica as clinicaIdClinica, usuarioIdusuario as usuarioIdusuario ");
                sql.Append("from PACIENTE where usuarioIdusuario = @usuarioIdusuario ");
                p = conexao.QueryFirstOrDefault<Paciente>(sql.ToString(), new { usuarioIdusuario = id });

                if (p != null)
                    return p;
                else
                    return NotFound("Paciente não encontrado");
            }
        }

        // Após uma consulta, é possível fazer a alteração de um dado do Paciente e fazer o update
        // PUT - http://url:5000/pacientes
        // No body da requisição enviar:
        /*
        {
        "idPaciente": 1
        "statusPaciente": true,
        "nomePai": "Marco Antonio Royo Felippe",
        "cpfPai": "123.456.789-00",
        "nomeMae": "Ana Claudia Rescia Royo Felippe",
        "cpfMae": "321.654.987-99",
        "enderPaciente": "Rua Cecília Meireles, 320",
        "compEnderPaciente": "Casa 7",
        "cepPaciente": "02123-010",
        "cidadePaciente": "São Paulo",
        "ufPaciente": "SP",
        "clinicaIdClinica": 5,
        "usuarioIdusuario": 13
        }
        */
        [HttpPut]
        public IActionResult Alterar(Paciente p)
        {
            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();

                StringBuilder sql = new StringBuilder();
                sql.Append("UPDATE PACIENTE SET ");
                sql.Append("statusPaciente = @statusPaciente, ");
                sql.Append("nomePai = @nomePai, cpfPai = @cpfPai, nomeMae = @nomeMae, cpfMae = @cpfMae, ");
                sql.Append("enderecoPaciente = @enderPaciente, complementoEnder = @compEnderPaciente, cepPaciente = @cepPaciente, cidadePaciente = @cidadePaciente, ufPaciente = @ufPaciente, ");
                sql.Append("clinicaIDclinica = @clinicaIdClinica, usuarioIdusuario = @usuarioIdusuario ");
                sql.Append("where idPaciente = @idPaciente ");
                int linhasAfetadas = conexao.Execute(sql.ToString(), p);
                return Ok(linhasAfetadas);
            }
        }




    }
}