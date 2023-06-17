using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using Syncfusion.Windows.Forms.Chart.SvgBase;
using System.Text.Json;
using System.Collections.ObjectModel;

namespace Cinemania
{
    public static class Store
    {
        // Store for public variables and functions accesible everywhere

        public static int rows = 6;
        public static int columns = 11;
        public static int marginSize = 4;
        public static Canvas seatsDisplay;
        public static Border selectedMovieItem;

        // Database
        private static string DB_Host = "localhost";
        private static string DB_Name = "cinemania";
        private static string DB_Username = "root";
        private static string DB_Password = "";
        public static string connectionString = "Server=localhost;Database=cinemania;Uid=root;Pwd='';";
        public static MySqlConnection connection;

        public static ObservableCollection<Movie> allMovies;


        public static bool GetSelectedMovieReservedStatus()
        {
            Movie selectedMovie = (Movie)selectedMovieItem.DataContext;
            return selectedMovie.Reserved;
        }

        public static void SetSelectedMovieReservedStatus(bool newValue)
        {
            Movie selectedMovie = (Movie)selectedMovieItem.DataContext;
            selectedMovie.Reserved = newValue;
        }

        public static void UpdateSelectedMovieInDatabase()
        {
            Movie selectedMovie = (Movie)selectedMovieItem.DataContext;
            selectedMovie.UpdateInDatabase();
        }

        public static int GenerateRandomNumber(int maxNumber)
        {
            // Create a byte array to store the random number
            byte[] randomNumber = new byte[4];

            // Create an instance of RandomNumberGenerator
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                // Fill the byte array with random numbers
                rng.GetBytes(randomNumber);
            }

            // Convert the byte array to an integer
            int randomInt = BitConverter.ToInt32(randomNumber, 0);

            // Generate a positive random number within a specific range
            int finalNumber = Math.Abs(randomInt % maxNumber) + 1;

            return finalNumber;
        }

        public static void GetMoviesFromDatabase()
        {
            string query = "SELECT * FROM movies";

            MySqlCommand command = new MySqlCommand(query, connection);

            allMovies = new ObservableCollection<Movie>();

            connection.Open();
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int ID = reader.GetInt32("ID");
                    string name = reader.GetString("name");
                    Seat[] seats = JsonSerializer.Deserialize<Seat[]>(reader.GetString("seats"));
                    DateTime datetime = reader.GetDateTime("datetime");
                    int reserved = reader.GetInt16("user_reserved");

                    Movie movie = new Movie(ID, name, datetime, seats, reserved == 1);
                    allMovies.Add(movie);
                }
            }
            connection.Close();

        }

    }

}
