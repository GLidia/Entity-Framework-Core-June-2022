using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace _07_PrintAllMinionNames
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "SERVER=.\\SQLExpress;Database=MinionsDB;Integrated Security=true;Encrypt=false";
            SqlConnection connection = new SqlConnection(connectionString);

            using (connection)
            {
                connection.Open();

                string getAllMinionNamesQuery = "SELECT Name FROM Minions";
                SqlCommand command = new SqlCommand(getAllMinionNamesQuery, connection);
                SqlDataReader reader = command.ExecuteReader();

                using (reader)
                {
                    List<string> minionNames = new List<string>();

                    while (reader.Read())
                    {
                        minionNames.Add(reader["Name"] as string);
                    }

                    for (int i = 0; i < minionNames.Count / 2; i++)
                    {
                        Console.WriteLine(minionNames[i]);
                        Console.WriteLine(minionNames[minionNames.Count - 1 - i]);
                    }

                    if (minionNames.Count % 2 == 1)
                    {
                        Console.WriteLine(minionNames[minionNames.Count / 2]);
                    }
                }
            }
        }
    }
}
