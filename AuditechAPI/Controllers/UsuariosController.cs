using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Data;
using Dapper;
using AuditechAPI.Models;
using AuditechAPI.DAL;
using System.Collections.Generic;
using System;

namespace AuditechAPI.Controllers
{

    [ApiController]
    [Route("[Controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IConfiguration _config;
        public UsuariosController(IConfiguration config)
        {
            _config = config;
        }

        // Método utilizado para fazer uma consulta de todo os usuários cadastrados no BD.
        // GET - http://url/usuarios
        // Este método será usado para manutenção do sistema não estará disponível para usuário final.

        [HttpGet]
        public ContentResult ConsultarAll([FromServices] IConfiguration config)
        {
            using var conexao = new SqlConnection(config.GetConnectionString("ConexaoSomee"));
            using var cmd = conexao.CreateCommand();

            StringBuilder sql = new StringBuilder();
            sql.Append("Select IdUsuario as Id, tipoUsuarioIdTipoUsuario as 'Tipo Usuário', ");
            sql.Append("nomeUsuario as Nome, cpfUsuario as CPF, dtNascimentoUsuario as 'Data Nascimento' ");
            sql.Append("FROM USUARIO for JSON PATH, ROOT('USUARIO') ");
            cmd.CommandText = sql.ToString();
            conexao.Open();
            string valorJSON = (string)cmd.ExecuteScalar();
            conexao.Close();
            return Content(valorJSON, "application/json");
        }

        // Método utilizado para fazer uma consulta de usuário por Id válido
        // GET - http://url:5000/usuarios/Id

        [HttpGet("{id}")]
        public ActionResult<Usuario> ConsultarById(int id)
        {
            Usuario u = null;

            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                sql.Append("Select IdUsuario as IdUsuario, tipoUsuarioIdTipoUsuario as idTipoUsuario, ");
                sql.Append("nomeUsuario as nome, cpfUsuario as cpf, dtNascimentoUsuario as dataNascimento ");
                sql.Append("from USUARIO where IdUsuario = @Id ");
                u = conexao.QueryFirstOrDefault<Usuario>(sql.ToString(), new { Id = id });

                if (u != null)
                    return u;
                else
                    return NotFound("Usuário não encontrado");
            }
        }

        // Método utilizado para fazer uma consulta de usuário válido (CPF e dtNascimento)
        // GET - http://url/usuarios/login/321.654.987-00/1974-06-11

        [HttpGet("login/{nCPF}/{nDTN}")]
        public ActionResult<Usuario> LoginByCPFdtN(string nCPF, string nDTN)
        {
            Usuario u = null;

            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                sql.Append("Select IdUsuario as IdUsuario, tipoUsuarioIdTipoUsuario as idTipoUsuario, ");
                sql.Append("nomeUsuario as nome, cpfUsuario as cpf, dtNascimentoUsuario as dataNascimento ");
                sql.Append("from USUARIO where cpfUsuario = @NCPF AND dtNascimentoUsuario = @NDTN ");
                u = conexao.QueryFirstOrDefault<Usuario>(sql.ToString(), new { NCPF = nCPF, NDTN = Convert.ToDateTime(nDTN) });

                if (u != null)
                    return u;
                else
                    return NotFound("Login Inválido");
            }
        }

        // Método será utilizado para inserir um novo usuário:
        // Para utilizar o método deverá ser usado:
        // POST - http://url/usuarios - e no Body da mensagem:
        /*
                {
                    "idTipoUsuario": 2,
                    "nome": "Ana Claudia Rescia Royo Felippe",
                    "cpf": "123.456.789-00",
                    "dataNascimento": "1979-03-23"
                }
        */
        [HttpPost]
        public IActionResult Cadastrar(Usuario u)
        {
            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                sql.Append("INSERT INTO USUARIO (tipoUsuarioIdTipoUsuario, nomeUsuario, cpfUsuario, dtNascimentoUsuario) ");
                sql.Append("values (@idTipoUsuario, @nome, @cpf, @dataNascimento) ");
                sql.Append("SELECT CAST(SCOPE_IDENTITY() AS INT) ");
                object o = conexao.ExecuteScalar(sql.ToString(), u);

                if (o != null)
                    u.IdUsuario = Convert.ToInt32(o);
            }
            return Ok(u.IdUsuario);
        }

        // Após uma consulta, é possível fazer a alteração de um dado do usuário e fazer o update
        // PUT - http://url:5000/usuarios
        [HttpPut]
        public IActionResult Alterar(Usuario u)
        {
            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();

                StringBuilder sql = new StringBuilder();
                sql.Append("UPDATE USUARIO SET ");
                sql.Append("tipoUsuarioIdTipoUsuario = @idTipoUsuario, nomeUsuario = @nome, cpfUsuario = @cpf, dtNascimentoUsuario = @dataNascimento ");
                sql.Append("WHERE IdUsuario = @IdUsuario ");
                int linhasAfetadas = conexao.Execute(sql.ToString(), u);
                return Ok(linhasAfetadas);
            }
        }

        // Apaga usuário baseado no Id encaminhado na requisição
        // DELETE - http://url/usuarios/Id
        // Onde Id é o Id válido do usuário.
        // Este requeste será usado apenas para manutenção do sistema. Não estará disponível para usuário final.
        [HttpDelete("{id}")]
        public IActionResult Deletar(int id)
        {
            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                sql.Append("DELETE FROM USUARIO ");
                sql.Append("WHERE idUsuario = @Id ");

                int linhasAfetadas = conexao.Execute(sql.ToString(), new { Id = id });
                return Ok(linhasAfetadas);
            }
        }


        // Valida credencial do usuário com CPF e Data de Nascimento
        // GET - http://url:5000/uauarios/ValidaUsuario
        // Enviar no Body da requisição:
        /*
        {
            "cpf": "123.456.789-01",
            "dataNascimento": "01/01/1990 00:00:00"
        }
        */
        [HttpGet("ValidaUsuario")]
        public ActionResult<Usuario> GetValidaUsuario(Usuario pCredencial)
        {

            Usuario u = new Usuario();

            using (IDbConnection conexao = ConnectionFactory.GetStringConexao(_config))
            {
                conexao.Open();
                StringBuilder sql = new StringBuilder();
                sql.Append("Select IdUsuario as IdUsuario, tipoUsuarioIdTipoUsuario as idTipoUsuario, ");
                sql.Append("nomeUsuario as nome, cpfUsuario as cpf, dtNascimentoUsuario as dataNascimento ");
                sql.Append("from USUARIO where cpfUsuario = @cpf and dtNascimentoUsuario = @dataNascimento ");
                u = conexao.QueryFirstOrDefault<Usuario>(sql.ToString(), new { cpf = pCredencial.cpf, dataNascimento = pCredencial.dataNascimento });

                if (u == null)
                    return NotFound("Usuário não encontrado");
                else
                    return u;
            }

        }
    }
}