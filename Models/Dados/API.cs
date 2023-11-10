using System.Data;
using System.Reflection;
using System.Text;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace eCode.Models
{
    public class API
    {
        #region Metodos Privado

        private string ObterConnectionString()
        {
            return new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build().GetConnectionString("MySqlConnection");
        }

        private string RetornarJSONQueryDelete<T>(T entidade, string query) where T : class
        {
           DataTable dt = new DataTable();
            int executou = 0;

            using (MySqlConnection conexao = new MySqlConnection(ObterConnectionString()))
            {
                conexao.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conexao))
                {
                    PropertyInfo[] propriedades = typeof(T).GetProperties();
                    foreach (PropertyInfo propriedade in propriedades)
                    {
                        cmd.Parameters.AddWithValue("@" + propriedade.Name, propriedade.GetValue(entidade));
                    }

                    try
                    {
                        executou = cmd.ExecuteNonQuery();
                    }
                    catch
                    {
                        executou = 0;
                    }

                    dt.Columns.Add("Campo", typeof(string));
                    dt.Rows.Add(executou > 0 ? "Sucesso" : "Erro");
                }
            }

            return JsonConvert.SerializeObject(dt, Formatting.Indented);
        }

        private string RetornarJSONQueryInsert<T>(T entidade, string query) where T : class
        {
            DataTable dt = new DataTable();
            using (MySqlConnection conexao = new MySqlConnection(ObterConnectionString()))
            {
                conexao.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conexao))
                {
                    PropertyInfo[] propriedades = typeof(T).GetProperties();
                    foreach (PropertyInfo propriedade in propriedades)
                    {
                        cmd.Parameters.AddWithValue("@" + propriedade.Name, propriedade.GetValue(entidade));
                    }

                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "SELECT LAST_INSERT_ID()";

                    int id = Convert.ToInt32(cmd.ExecuteScalar());

                    dt.Columns.Add("Id", typeof(int));
                    dt.Rows.Add(id);
                }
            }

            return JsonConvert.SerializeObject(dt, Formatting.Indented);
        }

        private string RetornarJSONQuerySelect(string query)
        {
            DataTable dt = new DataTable();
            using (MySqlConnection conexao = new MySqlConnection(ObterConnectionString()))
            {
                conexao.Open();
                using (MySqlCommand command = new MySqlCommand(query, conexao))
                {
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        adapter.Fill(dt);
                    }
                }
            }

            return JsonConvert.SerializeObject(dt, Formatting.Indented);
        }

        private string RetornarJSONQueryUpdate<T>(T entidade, string query) where T : class
        {
            DataTable dt = new DataTable();
            int executou = 0;

            using (MySqlConnection conexao = new MySqlConnection(ObterConnectionString()))
            {
                conexao.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, conexao))
                {
                    PropertyInfo[] propriedades = typeof(T).GetProperties();
                    foreach (PropertyInfo propriedade in propriedades)
                    {
                        cmd.Parameters.AddWithValue("@" + propriedade.Name, propriedade.GetValue(entidade));
                    }

                    try
                    {
                        executou = cmd.ExecuteNonQuery();
                    }
                    catch
                    {
                        executou = 0;
                    }

                    dt.Columns.Add("Campo", typeof(string));
                    dt.Rows.Add(executou > 0 ? "Sucesso" : "Erro");
                }
            }

            return JsonConvert.SerializeObject(dt, Formatting.Indented);
        }

        #endregion


        #region Metodos

        public string? AlterarCliente(eCliente e)
        {
            List<eGenericoCampos>? lista = new List<eGenericoCampos>();
            StringBuilder sb = new StringBuilder();

            sb.Append("UPDATE ecodedev.clientes SET ");
            sb.Append(string.Format("Nome = '{0}', ", e.Nome));
            sb.Append(string.Format("CPF = '{0}', ", e.CPF));
            sb.Append(string.Format("Email = '{0}', ", e.Email));
            sb.Append(string.Format("Senha = '{0}', ", e.Senha));
            sb.Append(string.Format("Telefone = '{0}', ", e.Telefone));
            sb.Append(string.Format("Visivel = '{0}', ", e.Visivel));
            sb.Append(string.Format("Apoiador = '{0}', ", e.Apoiador));
            sb.Append(string.Format("Perfil = '{0}' ", e.Perfil));
            sb.Append(string.Format("WHERE (Id = '{0}');", e.Id));

            string json = RetornarJSONQueryUpdate(e, sb.ToString());

            if (!string.IsNullOrEmpty(json))
            {
                lista = JsonConvert.DeserializeObject<List<eGenericoCampos>?>(json);
            }

            return lista?.Count > 0 ? lista[0].Campo : string.Empty;
        }

        public void AlterarClienteApoiador(eGenericoCampos e)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("UPDATE ecodedev.clientes SET ");
            sb.Append(string.Format("Apoiador = '{0}' ", e.Campo));
            sb.Append(string.Format("WHERE (Id = '{0}');", e.Id));

            RetornarJSONQueryUpdate(e, sb.ToString());
        }

        public void DeletarPlano(eGenericoCampos e)
        {
            RetornarJSONQueryDelete(e, string.Format("DELETE FROM ecodedev.assinaturas WHERE (Id = '{0}');", e.Id));
        }

        public string? GravarAssinatura(eAssinatura e)
        {
            List<eGenericoCampos>? lista = new List<eGenericoCampos>();
            StringBuilder sb = new StringBuilder();

            sb.Append("INSERT INTO ecodedev.assinaturas ");
            sb.Append("(IdCliente, Ativo, DataHora, DataPagamento, Expirar, TipoPagamento, TipoPlano, ValorPago)");
            sb.Append(" VALUES ");
            sb.Append(string.Format("('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}');", e.IdCliente, e.Ativo, e.DataHora.ToString(@"yyyy-MM-dd HH:mm:ss"), e.DataPagamento.ToString(@"yyyy-MM-dd HH:mm:ss"), e.Expirar.ToString(@"yyyy-MM-dd HH:mm:ss"), e.TipoPagamento, e.TipoPlano, e.ValorPago.ToString().Replace(",", ".")));

            string json = RetornarJSONQueryInsert(e, sb.ToString());
            if (!string.IsNullOrEmpty(json))
            {
                lista = JsonConvert.DeserializeObject<List<eGenericoCampos>?>(json);
            }

            return lista?.Count > 0 && lista[0].Id > 0 ? "Sucesso" : "Erro";
        }

        public int GravarCliente(eCliente e)
        {
            List<eGenericoCampos>? lista = new List<eGenericoCampos>();

            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO ecodedev.clientes (Nome, CPF, Email, Senha, Telefone, DataHora, Visivel, Apoiador, Perfil) ");
            sb.Append(string.Format("VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}'); ", e.Nome, e.CPF, e.Email, e.Senha, e.Telefone, e.DataHora.ToString("yyyy-MM-dd HH:mm:ss"), e.Visivel, e.Apoiador, e.Perfil));

            string json = RetornarJSONQueryInsert(e, sb.ToString());

            if (!string.IsNullOrEmpty(json))
            {
                lista = JsonConvert.DeserializeObject<List<eGenericoCampos>?>(json);
            }

            return lista?.Count > 0 ? lista[0].Id : 0;
        }

        public List<eDesafios>? ListarDesafios(string filtro, string? apoiador, string? perfil)
        {
            List<eDesafios>? lista = new List<eDesafios>();
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrEmpty(perfil) && string.Equals(perfil, "A"))
            {
                sb.Append(string.Format("SELECT * FROM ecodedev.desafios WHERE (Stack = '{0}') ORDER BY DataHora DESC, Titulo ASC LIMIT 250;", filtro));
            }
            else
            {
                if (string.Equals(apoiador, "S"))
                {
                    sb.Append("SELECT * FROM ecodedev.desafios ");
                    sb.Append(string.Format("WHERE (Visivel = 'S' AND Stack = '{0}') ORDER BY DataHora DESC, Titulo ASC LIMIT 200;", filtro));
                }
                else
                {
                    sb.Append("SELECT * FROM ecodedev.desafios ");
                    sb.Append(string.Format("WHERE (Visivel = 'S' AND Stack = '{0}' AND Apoiador = '{1}') ORDER BY DataHora DESC, Titulo ASC LIMIT 50;", filtro, apoiador));
                }
            }

            string json = RetornarJSONQuerySelect(sb.ToString());

            if (!string.IsNullOrEmpty(json))
            {
                lista = JsonConvert.DeserializeObject<List<eDesafios>?>(json);
            }

            return lista;
        }

        public List<eDesafios>? ListarDesafiosHome()
        {
            List<eDesafios>? lista = new List<eDesafios>();
            string json = RetornarJSONQuerySelect("SELECT * FROM ecodedev.desafios WHERE (Visivel = 'S' AND Apoiador = 'N') ORDER BY Id DESC LIMIT 3");

            if (!string.IsNullOrEmpty(json))
            {
                lista = JsonConvert.DeserializeObject<List<eDesafios>?>(json);
            }

            return lista;
        }

        public eAssinatura? ObterAssinatura(int idCliente)
        {
            List<eAssinatura>? lista = new List<eAssinatura>();
            string json = RetornarJSONQuerySelect(string.Format("SELECT A.Id, A.Ativo, A.Expirar, A.TipoPlano, A.Ativo from ecodedev.clientes C INNER JOIN ecodedev.assinaturas A on A.IdCliente = C.Id WHERE (C.Apoiador = 'S' AND C.Id = '{0}');", idCliente));

            if (!string.IsNullOrEmpty(json))
            {
                lista = JsonConvert.DeserializeObject<List<eAssinatura>?>(json);
            }

            return lista?.Count > 0 ? lista[0] : null;
        }

        public eCliente? ObterDadosCliente(int idCliente)
        {
            List<eCliente>? lista = new List<eCliente>();
            string json = RetornarJSONQuerySelect(string.Format("SELECT Id, Nome, CPF, Email, Senha, Telefone, DataHora, Visivel, Apoiador, Perfil FROM ecodedev.clientes WHERE (Id = {0} AND Visivel = 'S')", idCliente));

            if (!string.IsNullOrEmpty(json))
            {
                lista = JsonConvert.DeserializeObject<List<eCliente>?>(json);
            }

            return lista?.Count > 0 ? lista[0] : null;
        }

        public eCliente? ObterPerfil(int idCliente)
        {
            List<eCliente>? lista = new List<eCliente>();
            string json = RetornarJSONQuerySelect(string.Format("SELECT Id, Nome, Perfil, Apoiador FROM ecodedev.clientes WHERE (Id = {0} AND Visivel = 'S')", idCliente));

            if (!string.IsNullOrEmpty(json))
            {
                lista = JsonConvert.DeserializeObject<List<eCliente>?>(json);
            }

            return lista?.Count > 0 ? lista[0] : null;
        }

        public eGenericoCampos? VerificarExisteUsuario(string email, string senha)
        {
            List<eGenericoCampos>? lista = new List<eGenericoCampos>();
            string json = RetornarJSONQuerySelect(string.Format("SELECT Id, Nome AS Campo FROM ecodedev.clientes WHERE (Email = '{0}' AND Senha = '{1}' AND Visivel = 'S')", email, senha));

            if (!string.IsNullOrEmpty(json))
            {
                lista = JsonConvert.DeserializeObject<List<eGenericoCampos>?>(json);
            }

            return lista?.Count > 0 ? lista[0] : null;
        }

        #endregion
    }
}