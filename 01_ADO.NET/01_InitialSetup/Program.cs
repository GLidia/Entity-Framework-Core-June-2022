using Microsoft.Data.SqlClient;
using System;

namespace _01_InitialSetup
{
    class Program
    {
        static void Main(string[] args)
        {
            //1) Creating a new Database called "MinionsDB"
            //string initialConnectionString = "SERVER=.\\SQLExpress;Database=master;Integrated Security=true;Encrypt=false";

            //SqlConnection connection = new SqlConnection(initialConnectionString);
            //connection.Open();

            //using (connection)
            //{
            //    SqlCommand command = new SqlCommand("CREATE DATABASE MinionsDB", connection);
            //    command.ExecuteNonQuery();
            //}

            //2) creating and filling tables
            string connectionString = "SERVER=.\\SQLExpress;Database=MinionsDB;Integrated Security=true;Encrypt=false";
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            using (connection)
            {
                string[] createTableQueries = new string[]
                {
                    "CREATE TABLE Countries (Id INT IDENTITY PRIMARY KEY, Name VARCHAR(50) NOT NULL)",
                    "CREATE TABLE Towns (Id INT IDENTITY PRIMARY KEY, Name VARCHAR(50) NOT NULL, CountryCode INT FOREIGN KEY REFERENCES Countries(Id))",
                    "CREATE TABLE Minions (Id INT IDENTITY PRIMARY KEY, Name VARCHAR(50) NOT NULL, Age INT, TownId INT FOREIGN KEY REFERENCES Towns(Id))",
                    "CREATE TABLE EvilnessFactors (Id INT IDENTITY PRIMARY KEY, Name VARCHAR(50) NOT NULL)",
                    "CREATE TABLE Villains (Id INT IDENTITY PRIMARY KEY, Name VARCHAR(50) NOT NULL, " +
                    "EvilnessFactorId INT FOREIGN KEY REFERENCES EvilnessFactors(Id))",
                    "CREATE TABLE MinionsVillains (MinionId INT, VillainId INT, CONSTRAINT PK_MinionsVillains PRIMARY KEY(MinionId, VillainId), " +
                    "CONSTRAINT FK_MinionsVillainsMinion FOREIGN KEY (MinionId) REFERENCES Minions(Id), CONSTRAINT FK_MinionsVillainsVillain FOREIGN KEY (VillainId) REFERENCES Villains(Id))"
                };

                foreach (string query in createTableQueries)
                {
                    SqlCommand command1 = new SqlCommand(query, connection);
                    command1.ExecuteNonQuery();
                }

                string[] populateTableQueries = new string[]
                {
                    "INSERT INTO Countries VALUES ('England'), ('Bulgaria'), ('Germany'), ('Spain'), ('Egypt')",
                    "INSERT INTO Towns (Name, CountryCode) VALUES ('London', 1), ('Sofia', 2), ('Berlin', 3), ('Madrid', 4), ('Cairo', 5)",
                    "INSERT INTO Minions (Name, Age, TownId) VALUES ('Minion1', 12, 2), ('Minion2', 3, 3), ('Minion3', 4, 1), ('Minion4', 29, 4), ('Minion5', 19, 5)",
                    "INSERT INTO EvilnessFactors (Name) VALUES ('super good'), ('good'), ('bad'), ('evil'), ('super evil')",
                    "INSERT INTO Villains (Name, EvilnessFactorId) VALUES ('Villain1', 1), ('Villain2', 4), ('Villain3', 2), ('Villain4', 3), ('Villain5', 5)",
                    "INSERT INTO MinionsVillains VALUES (1, 1), (2, 4), (3, 2), (4, 5), (2, 3)"
                };

                foreach (string query in populateTableQueries)
                {
                    SqlCommand command2 = new SqlCommand(query, connection);
                    command2.ExecuteNonQuery();
                }

            }

        }


    }
}
