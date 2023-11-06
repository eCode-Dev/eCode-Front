using System.Data;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;

namespace eCode.Models
{
    public class API
    {
        #region Metodos Privado

        private string RetornarJSONQuerySelect(string query)
        {
            DataTable dt = new DataTable();
            using (MySqlConnection connection = new MySqlConnection(ObterConnectionString()))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
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