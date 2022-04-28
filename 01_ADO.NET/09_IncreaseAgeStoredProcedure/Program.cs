using Microsoft.Data.SqlClient;
using System;

namespace _09_IncreaseAgeStoredProcedure
{
    class Program
    {
        static void Main(string[] args)
        {

            //In Management Studio:
            //CREATE PROC usp_GetOlder(@minionId INT)
            //AS
            //UPDATE Minions
            //SET Age += 1
            //WHERE Id = @minionId
            //GO

            int id = int.Parse(Console.ReadLine());

            string connectionString = "SERVER=.\\SQLExpress;Database=MinionsDB;Integrated Security=true;Encrypt=false";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            using (connection)
            {
                SqlCommand command1 = new SqlCommand("EXEC usp_GetOlder @minionId", connection);
                command1.Parameters.AddWithValue("@minionId", id);
                command1.ExecuteNonQuery();
                
                SqlCommand command2 = new SqlCommand("SELECT Name, Age FROM Minions WHERE Id = @minionId", connection);
                command2.Parameters.AddWithValue("@minionId", id);
                SqlDataReader reader = command2.ExecuteReader();

                using (reader)
                {
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["Name"]} - {reader["Age"]} years old");
                    }
                }
            }
        }
    }
}
