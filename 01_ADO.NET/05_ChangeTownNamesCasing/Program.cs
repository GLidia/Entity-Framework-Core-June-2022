using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace _05_ChangeTownNamesCasing
{
    class Program
    {
        static void Main(string[] args)
        {
            string countryName = Console.ReadLine();

            string connectionString = "SERVER=.\\SQLExpress;Database=MinionsDB;Integrated Security=true;Encrypt=false";
            SqlConnection connection = new SqlConnection(connectionString);

            using (connection)
            {
                connection.Open();

                //get countryId
                string getCountryIdQuery = "SELECT Id FROM Countries WHERE Name = @cName";
                SqlCommand command1 = new SqlCommand(getCountryIdQuery, connection);
                command1.Parameters.AddWithValue("@cName", countryName);
                int? countryId = command1.ExecuteScalar() as int?;

                if (countryId == null)
                {
                    Console.WriteLine("No town names were affected.");
                }
                else
                {
                    //change case of town names
                    string changingCaseQuery = "UPDATE Towns SET Name = UPPER(NAME) WHERE CountryCode = @cId";
                    SqlCommand command2 = new SqlCommand(changingCaseQuery, connection);
                    command2.Parameters.AddWithValue("cId", countryId);
                    int? rowsAffected = command2.ExecuteNonQuery();

                    Console.WriteLine($"{rowsAffected} town names were affected.");

                    //get town names
                    string gettingTownNamesQuery = "SELECT Name FROM Towns WHERE CountryCode = @cId";
                    SqlCommand command3 = new SqlCommand(gettingTownNamesQuery, connection);
                    command3.Parameters.AddWithValue("@cId", countryId);
                    SqlDataReader reader = command3.ExecuteReader();

                    using (reader)
                    {
                        List<string> townNames = new List<string>();

                        while (reader.Read())
                        {
                            townNames.Add(reader["Name"] as string);
                        }

                        Console.WriteLine($"[{string.Join(", ", townNames)}]");
                    }
                }
            }
        }
    }
}
