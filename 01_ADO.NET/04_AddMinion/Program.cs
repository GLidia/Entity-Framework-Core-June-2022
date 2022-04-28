using Microsoft.Data.SqlClient;
using System;

namespace _04_AddMinion
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] minionInfo = Console.ReadLine().Split(" ");
            string[] villainInfo = Console.ReadLine().Split(" ");

            string minionName = minionInfo[1];
            int minionAge = int.Parse(minionInfo[2]);
            string minionTown = minionInfo[3];

            string villainName = villainInfo[1];

            string connectionString = "SERVER=.\\SQLExpress;Database=MinionsDB;Integrated Security=true;Encrypt=false";
            SqlConnection connection = new SqlConnection(connectionString);


            using (connection)
            {
                SqlTransaction transaction = null;

                try
                {
                    connection.Open();
                    transaction = connection.BeginTransaction();

                    //checking whether town exists and getting its Id
                    string checkingForTownQuery = "SELECT Id FROM Towns WHERE Name = @townName";
                    SqlCommand command1 = new SqlCommand(checkingForTownQuery, connection, transaction);
                    command1.Parameters.AddWithValue("@townName", minionTown);
                    int? townId= command1.ExecuteScalar() as int?;

                    if (townId == null)
                    {
                        //adding town
                        string addingTownQuery = "INSERT INTO Towns(Name) VALUES (@townName)";
                        SqlCommand command2 = new SqlCommand(addingTownQuery, connection, transaction);
                        command2.Parameters.AddWithValue("@townName", minionTown);
                        command2.ExecuteNonQuery();
                        Console.WriteLine($"Town {minionTown} was added to the database.");

                        townId = (int)command1.ExecuteScalar();
                    }

                    //checking whether villain exists and getting his/her id
                    string checkingForVillainQuery = "SELECT Id FROM Villains WHERE Name=@villain";
                    SqlCommand command3 = new SqlCommand(checkingForVillainQuery, connection, transaction);
                    command3.Parameters.AddWithValue("@villain", villainName);
                    int? villainId = command3.ExecuteScalar() as int?;

                    if (villainId == null)
                    {
                        //adding villain
                        string addingVillainQuery = "INSERT INTO Villains(Name, EvilnessFactorId) " +
                            "VALUES (@villain, (SELECT Id FROM EvilnessFactors WHERE Name = 'evil'))";
                        SqlCommand command4 = new SqlCommand(addingVillainQuery, connection, transaction);
                        command4.Parameters.AddWithValue("@villain", villainName);
                        command4.ExecuteNonQuery();
                        Console.WriteLine($"Villain {villainName} was added to the database.");

                        villainId = (int)command3.ExecuteScalar();
                    }

                    //adding minion
                    string addingMinionQuery = "INSERT INTO Minions (Name, Age, TownId) VALUES (@mName, @mAge, @tId)";
                    SqlCommand command5 = new SqlCommand(addingMinionQuery, connection, transaction);
                    command5.Parameters.AddWithValue("@mName", minionName);
                    command5.Parameters.AddWithValue("@mAge", minionAge);
                    command5.Parameters.AddWithValue("@tId", townId);
                    command5.ExecuteNonQuery();

                    //setting minion as minion of the villain
                    string gettingMinionIdQuery = "SELECT Id FROM Minions WHERE Name=@mName";
                    SqlCommand command6 = new SqlCommand(gettingMinionIdQuery, connection, transaction);
                    command6.Parameters.AddWithValue("mName", minionName);
                    int minionId = (int)command6.ExecuteScalar();
                    string addingToMinionsVillainsQuery = "INSERT INTO MinionsVillains(MinionId, VillainId) VALUES(@mId, @vId)";
                    SqlCommand command7 = new SqlCommand(addingToMinionsVillainsQuery, connection, transaction);
                    command7.Parameters.AddWithValue("mId", minionId);
                    command7.Parameters.AddWithValue("vId", villainId);
                    command7.ExecuteNonQuery();
                    Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");

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
                        Console.WriteLine("Rollback failed " + oex.Message);
                    }
                }
            }
        }
    }
}
