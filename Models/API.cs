using System.Data;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace eCode.Models
{
    public class API
    {
        #region Banco Dados Conexão

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
            List<eDesafios>? lista = new();
            string json = RetornarJSONQuerySelect("SELECT * FROM ecodedev.desafios WHERE (Visivel = 'S' AND Apoiador = 'N') ORDER BY Id DESC LIMIT 3");

            if (!string.IsNullOrEmpty(json))
            {
                lista = JsonConvert.DeserializeObject<List<eDesafios>?>(json);
            }

            return lista;
        }

        #endregion
    }
}