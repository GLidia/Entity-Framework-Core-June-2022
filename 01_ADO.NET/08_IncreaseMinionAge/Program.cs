using Microsoft.Data.SqlClient;
using System;
using System.Linq;

namespace _08_IncreaseMinionAge
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] minionIds = Console.ReadLine().Split(" ").Select(int.Parse).ToArray();

            string connectionString = "SERVER=.\\SQLExpress;Database=MinionsDB;Integrated Security=true;Encrypt=false";
            SqlConnection connection = new SqlConnection(connectionString);

            using (connection)
            {
                connection.Open();

                string incrementAgeAndCapitalizeFirstLetterForMinions = "UPDATE Minions " +
                    "SET Name = UPPER(LEFT(Name, 1)) + LOWER(SUBSTRING(Name, 2, LEN(Name)))," +
                    "Age += 1" +
                    "WHERE Id = @minionId";
                
                foreach (int id in minionIds)
                {
                    SqlCommand command1 = new SqlCommand(incrementAgeAndCapitalizeFirstLetterForMinions, connection);
                    command1.Parameters.AddWithValue("@minionId", id);
                    command1.ExecuteNonQuery();
                }

                string getNameAndAgeFromMinionsQuery = "SELECT Name, Age FROM Minions";
                SqlCommand command2 = new SqlCommand(getNameAndAgeFromMinionsQuery, connection);
                SqlDataReader reader = command2.ExecuteReader();

                using (reader)
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(reader["Name"] as string + " " + reader["Age"] as string);
                    }
                }

            }
        }
    }
}
