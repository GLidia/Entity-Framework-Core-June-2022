using Microsoft.Data.SqlClient;
using System;

namespace _03_MinionNames
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
                //execute the creation of the stored procedure once:
                //string storedProcString = "CREATE PROC usp_MinionNamesAgesForVillain (@villainId INT) " +
                //    "AS " +
                //    "SELECT m.Name, m.Age FROM Minions AS m " +
                //    "JOIN MinionsVillains AS mv ON m.Id = mv.MinionId " +
                //    "WHERE mv.VillainId = @villainId " +
                //    "ORDER BY m.Name";

                //SqlCommand command1 = new SqlCommand(storedProcString, connection);
                //command1.ExecuteNonQuery();

                Console.WriteLine("Please enter villain id: ");
                int villainId = int.Parse(Console.ReadLine());

                string findingVillainQuery = "SELECT Name FROM Villains WHERE Id = @ID";
                SqlCommand command2 = new SqlCommand(findingVillainQuery, connection);
                command2.Parameters.AddWithValue("@ID", villainId);

                SqlDataReader reader1 = command2.ExecuteReader();
                string villainName = string.Empty;

                using (reader1)
                {
                    while (reader1.Read())
                    {
                        villainName = reader1["Name"] as string;
                    }
                    
                }

                if (string.IsNullOrEmpty(villainName))
                {
                    Console.WriteLine($"No villain with ID {villainId} exists in the database.");
                }
                else
                {
                    Console.WriteLine($"Villain: {villainName}");

                    string execString = $"EXEC usp_MinionNamesAgesForVillain {villainId}";
                    SqlCommand command3 = new SqlCommand(execString, connection);
                    SqlDataReader reader2 = command3.ExecuteReader();

                    using (reader2)
                    {
                        int count = 0;
                        while (reader2.Read())
                        {
                            count++;
                            string minionName = (string)reader2["Name"];
                            int minionAge = (int)reader2["Age"];
                            Console.WriteLine($"{count}. {minionName} {minionAge}");
                        }

                    }

                }

            }
        }
    }
}
