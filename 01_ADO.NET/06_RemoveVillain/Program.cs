using Microsoft.Data.SqlClient;
using System;

namespace _06_RemoveVillain
{
    class Program
    {
        static void Main(string[] args)
        {
            int villainId = int.Parse(Console.ReadLine());

            string connectionString = "SERVER=.\\SQLExpress;Database=MinionsDB;Integrated Security=true;Encrypt=false";
            SqlConnection connection = new SqlConnection(connectionString);

            using (connection)
            {
                SqlTransaction transaction = null;

                try
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    //get villain Name
                    string getVillainNameQuery = "SELECT Name FROM Villains WHERE Id = @vId";
                    SqlCommand command1 = new SqlCommand(getVillainNameQuery, connection, transaction);
                    command1.Parameters.AddWithValue("vId", villainId);
                    string villainName = command1.ExecuteScalar() as string;

                    if (string.IsNullOrEmpty(villainName))
                    {
                        Console.WriteLine("No such villain was found.");
                    }
                    else
                    {

                        //delete villainId from MinionsVillains
                        string deleteVillianIdQuery = "DELETE FROM MinionsVillains WHERE VillainId = @vId";
                        SqlCommand command2 = new SqlCommand(deleteVillianIdQuery, connection, transaction);
                        command2.Parameters.AddWithValue("@vId", villainId);
                        int affectedRows = command2.ExecuteNonQuery();

                        //delete villain
                        string deleteVillainQuery = "DELETE FROM Villains WHERE Id = @vId";
                        SqlCommand command3 = new SqlCommand(deleteVillainQuery, connection, transaction);
                        command3.Parameters.AddWithValue("@vId", villainId);
                        command3.ExecuteNonQuery();

                        Console.WriteLine($"{villainName} was deleted.");
                        Console.WriteLine($"{affectedRows} Minions were released.");
                    }

                    transaction.Commit();
                }
                catch(Exception ex)
                {
                    try
                    {
                        transaction.Rollback();
                        Console.WriteLine(ex.Message);
                    }
                    catch(Exception oex)
                    {
                        Console.WriteLine("Rollback failed. " + oex.Message);
                    }

                }
            }
        }
    }
}
