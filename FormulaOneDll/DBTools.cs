using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormulaOneDll
{
    public class DbTools
    {
        public const string WORKINGPATH = @"C:\Users\a.ouammi.0459\Desktop\FormulaOneSolution\Dati\";
        private const string CONNECTION_STRING = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\a.ouammi.0459\Desktop\FormulaOneSolution\Dati\FormulaOne.mdf;Integrated Security=True";
        private Dictionary<int, Driver> drivers;
        private Dictionary<string, Country> countries;

        public Dictionary<int, Driver> Drivers { get => drivers; set => drivers = value; }
        public Dictionary<string, Country> Countries { get => countries; set => countries = value; }
        public void ExecuteSqlScript(string sqlScriptName)
        {
            var fileContent = File.ReadAllText(WORKINGPATH + sqlScriptName);
            fileContent = fileContent.Replace("\r\n", "");
            fileContent = fileContent.Replace("\r", "");
            fileContent = fileContent.Replace("\n", "");
            fileContent = fileContent.Replace("\t", "");
            var sqlqueries = fileContent.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

            var con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + WORKINGPATH + "FormulaOne.mdf;Integrated Security=True");
            var cmd = new SqlCommand("query", con);
            con.Open(); int i = 0;
            foreach (var query in sqlqueries)
            {
                cmd.CommandText = query; i++;
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException err)
                {
                    Console.WriteLine("Errore in esecuzione della query numero: " + i);
                    Console.WriteLine("\tErrore SQL: " + err.Number + " - " + err.Message);
                }
            }
            con.Close();
        }

        public void DropTable(string tableName)
        {
            var con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + WORKINGPATH + "FormulaOne.mdf;Integrated Security=True");
            var cmd = new SqlCommand("Drop Table " + tableName + ";", con);
            con.Open();
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SqlException err)
            {
                Console.WriteLine("\tErrore SQL: " + err.Number + " - " + err.Message);
            }
            con.Close();
        }
        public void GetCountries()
        {
            if (this.Countries == null)
            {
                this.Countries = new Dictionary<string, Country>();
                var con = new SqlConnection(CONNECTION_STRING);
                using (con)
                {
                    con.Open();
                    var command = new SqlCommand("SELECT * FROM Countries;", con);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string countryIsoCode = reader.GetString(0);
                        Country c = new Country()
                        {
                            CountryCode = countryIsoCode,
                            CountryName = reader.GetString(1)
                        };
                        this.Countries.Add(countryIsoCode, c);
                    }
                    con.Close();
                    con.Dispose();
                }
                SqlConnection.ClearAllPools();
            }
        }


        public void GetDrivers(bool forceReload = false)
        {
            if (forceReload || this.Drivers == null)
            {
                this.Drivers = new Dictionary<int, Driver>();
                var con = new SqlConnection(CONNECTION_STRING);
                using (con)
                {
                    con.Open();
                    var command = new SqlCommand("SELECT * FROM Drivers;", con);
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        int driverIsoCode = reader.GetInt32(0);
                        Driver d = new Driver(driverIsoCode)
                        {
                            Firstname = reader.GetString(1),
                            Lastname = reader.GetString(2),
                            Dob = reader.GetDateTime(3),
                            PlaceOfBirthday = reader.GetString(4),
                            Country = Countries[reader.GetString(5)]
                        };
                        this.Drivers.Add(driverIsoCode, d);
                    }
                    con.Close();
                    con.Dispose();
                }
                SqlConnection.ClearAllPools();
            }
        }

        public BindingList<Team> LoadTeams()
        {
            BindingList<Team> retVal = new BindingList<Team>();
            var con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + WORKINGPATH + "FormulaOne.mdf;Integrated Security=True");
            using (con)
            {
                SqlCommand command = new SqlCommand(
                  "SELECT * FROM Teams",
                  con);
                con.Open();

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Team t = new Team(
                        reader.GetInt32(0),
                        reader.GetString(1),
                        reader.GetString(2),
                        countries[reader.GetString(3)],
                        reader.GetString(4),
                        reader.GetString(5),
                        reader.GetString(6),
                        drivers[reader.GetInt32(7)],
                        drivers[reader.GetInt32(8)]
                    );  
                    retVal.Add(t);
                }
                reader.Close();
            }
            return retVal;
        }
    }
}
