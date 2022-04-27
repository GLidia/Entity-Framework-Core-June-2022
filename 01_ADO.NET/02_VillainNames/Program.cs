using Microsoft.Data.SqlClient;
using System;

namespace _02_VillainNames
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "SERVER=.\\SQLExpress;Database=MinionsDB;Integrated Security=true;Encrypt=false";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            using (connection)
            {
                string queryString = "SELECT q.Name, q.Count " +
                    "FROM (SELECT v.Name, v.Id, COUNT(mv.MinionId) AS[Count] " +
                    "FROM Villains AS v JOIN MinionsVillains AS mv ON v.Id = mv.VillainId GROUP BY v.Name, v.Id) AS q " +
                    "WHERE q.Count > 3 " +
                    "ORDER BY q.Count DESC";
                SqlCommand command = new SqlCommand(queryString, connection);
                SqlDataReader reader = command.ExecuteReader();

                using (reader)
                {
                    while (reader.Read())
                    {
                        string villainName = (string)reader["Name"];
                        int count = (int)reader["Count"];
                        Console.WriteLine($"{villainName} - {count}");
                    }
                }
            }
        }
    }
}
