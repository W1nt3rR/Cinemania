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
using System.Windows;
using System.Data;

namespace Cinemania
{
    public static class Store
    {
        // Store for public variables and functions accesible everywhere

        // Seat Variables, do not change
        public static int rows = 6;
        public static int columns = 11;
        public static int marginSize = 4;

        // XAML Elements
        public static Canvas seatsDisplay;
        public static Border selectedMovieItem;
        public static Seat selectedSeat;
        public static ItemsControl Movies;

        // Database Config
        private static readonly string DB_Host = "localhost";
        private static readonly string DB_Name = "cinemania";
        private static readonly string DB_Username = "root";
        private static readonly string DB_Password = "";

        // Database Connection
        public static string connectionString = $"Server={DB_Host};Database={DB_Name};Uid={DB_Username};Pwd={DB_Password};";
        public static MySqlConnection connection;

        // Collections
        public static ObservableCollection<Movie> allMovies;
        public static ObservableCollection<Movie> filteredMovies;

        public static void ReserveTicket()
        {
            if (selectedMovieItem == null)
            {
                MessageBox.Show("Please select movie", "Select movie", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (selectedSeat == null)
            {
                MessageBox.Show("Please select seat", "Select seat", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Movie selectedMovie = (Movie)selectedMovieItem.DataContext;

            // Create Ticket
            string createTicketQuery = "INSERT INTO tickets (`row`, `column`, `movie_id`) VALUES (@row, @column, @movieID)";

            MySqlCommand createTicketCommand = new MySqlCommand(createTicketQuery, connection);
            createTicketCommand.Parameters.AddWithValue("@row", selectedSeat.Row);
            createTicketCommand.Parameters.AddWithValue("@column", selectedSeat.Column);
            createTicketCommand.Parameters.AddWithValue("@movieID", selectedMovie.ID);

            connection.Open();

            createTicketCommand.ExecuteNonQuery();

            // Get ID of new ticket
            int ticketID = (int)createTicketCommand.LastInsertedId;

            connection.Close();

            // Show Info about Ticket
            ShowTicketMessageBox(ticketID);

            RefreshMovies();

        }

        public static void RefreshMovies()
        {
            // Get new info
            GetMoviesFromDatabase();
            UpdateFilteredMovies();

            // Reset
            seatsDisplay.Children.Clear();
            selectedMovieItem = null;
            selectedSeat = null;
        }

        public static void UpdateFilteredMovies()
        {
            // Copy all movies into filteredMovies
            filteredMovies = new ObservableCollection<Movie>(allMovies);

            // Add filteredMovies to Movies items
            Movies.ItemsSource = filteredMovies;
        }

        public static void ShowTicketMessageBox(int ticketID)
        {
            // Get info about ticket
            Ticket ticket = Ticket.GetByID(ticketID);

            MessageBox.Show($"Your ticket number is {ticketID}. Your ticket is valid for Movie: '{ticket.movie.Name}' at '{ticket.movie.Date}' at Row: '{ticket.Row}', Column: '{ticket.Column}'", "Ticket", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void GetMoviesFromDatabase()
        {
            string query = "SELECT * FROM movies WHERE datetime > @datenow ORDER BY datetime ASC";

            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@datenow", DateTime.Now);

            if (allMovies == null)
            {
                allMovies = new ObservableCollection<Movie>();
            }
            else
            {
                allMovies.Clear();
            }

            connection.Open();
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int ID = reader.GetInt32("ID");
                    string name = reader.GetString("name");
                    DateTime datetime = reader.GetDateTime("datetime");

                    Movie movie = new Movie(ID, name, datetime);
                    allMovies.Add(movie);
                }
            }
            connection.Close();

            // Get occupied seats from databse
            foreach (Movie movie in allMovies)
            {
                movie.GetSeatsFromDatabase();
            }

        }

        public static void CreateTableIfNotExists()
        {
            // Exported queries from Database
            string moviesQuery = "CREATE TABLE IF NOT EXISTS `movies` (\r\n  `ID` int unsigned NOT NULL AUTO_INCREMENT,\r\n  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL DEFAULT 'Movie Name',\r\n  `datetime` datetime DEFAULT NULL,\r\n  PRIMARY KEY (`ID`),\r\n  UNIQUE KEY `ID` (`ID`)\r\n)";
            string ticketsQuery = "CREATE TABLE IF NOT EXISTS `tickets` (\r\n  `id` int NOT NULL AUTO_INCREMENT,\r\n  `movie_id` int unsigned NOT NULL,\r\n  `row` int NOT NULL,\r\n  `column` int NOT NULL,\r\n  PRIMARY KEY (`id`),\r\n  UNIQUE KEY `id` (`id`),\r\n  KEY `FK_tickets_movies` (`movie_id`),\r\n  CONSTRAINT `FK_tickets_movies` FOREIGN KEY (`movie_id`) REFERENCES `movies` (`ID`) ON DELETE CASCADE ON UPDATE CASCADE\r\n)";

            MySqlCommand moviesCommand = new MySqlCommand(moviesQuery, connection);
            MySqlCommand ticketsCommand = new MySqlCommand(ticketsQuery, connection);

            connection.Open();
            moviesCommand.ExecuteNonQuery();
            ticketsCommand.ExecuteNonQuery();
            connection.Close();
        }

    }

}
