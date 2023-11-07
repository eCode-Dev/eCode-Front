using System;
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

        private string ObterConnectionString()
        {
            return new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build().GetConnectionString("MySqlConnection");
        }

        #endregion


        #region Metodos

        public int GravarCliente(eCliente e)
        {
            List<eGenericoCampos>? lista = new List<eGenericoCampos>();

            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO ecodedev.cliente (Nome, CPF, Email, Senha, Telefone, DataHora, Visivel, Apoiador, Perfil) ");
            sb.Append(string.Format("VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}'); ", e.Nome, e.CPF, e.Email, e.Senha, e.Telefone, DateTime.Now, e.Visivel, e.Apoiador, e.Perfil));

            string json = RetornarJSONQueryInsert(e, sb.ToString());

            if (!string.IsNullOrEmpty(json))
            {
                lista = JsonConvert.DeserializeObject<List<eGenericoCampos>?>(json);
            }

            return lista?.Count > 0 ? lista[0].Id : 0;
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

        public eCliente? ObterUsuario(int idCliente)
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