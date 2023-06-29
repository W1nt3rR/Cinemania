using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;
using System.Text.Json;

namespace Cinemania
{
    public class Movie
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }

        public ObservableCollection<Seat> seats;

        public Movie(int id, string name, DateTime date)
        {
            this.ID = id;
            this.Name = name;
            this.Date = date;
        }

        public static Movie GetByID(int movieID)
        {
            string query = "SELECT * FROM movies WHERE id = @movieID";

            MySqlCommand command = new MySqlCommand(query, Store.connection);
            command.Parameters.AddWithValue("@movieID", movieID);

            Movie movie = null;

            Store.connection.Open();
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int ID = reader.GetInt32("ID");
                    string name = reader.GetString("name");
                    DateTime datetime = reader.GetDateTime("datetime");

                    movie = new Movie(ID, name, datetime);
                }
            }
            Store.connection.Close();

            return movie;
        }

        public void StoreToDatabase()
        {
            string query = "INSERT INTO movies (name, datetime) VALUES (@Name, @datetime)";

            MySqlCommand command = new MySqlCommand(query, Store.connection);
            command.Parameters.AddWithValue("@name", Name);
            command.Parameters.AddWithValue("@datetime", Date);

            Store.connection.Open();
            command.ExecuteNonQuery();
            Store.connection.Close();
        }

        public void RenderSeats(bool centerHorizontaly = true, bool centerVertically = false)
        {
            // Get seats canvas dimensions
            double canvasWidth = Store.seatsDisplay.ActualWidth;
            double canvasHeight = Store.seatsDisplay.ActualHeight;

            // Calculate candidate sizes
            double rectangleSizeByWidth = canvasWidth / Store.columns;
            double rectangleSizeByHeight = canvasHeight / Store.rows;

            // Choose size depending on the canvas dimensions, so it doesn't go outside of it
            double rectangleSize = (rectangleSizeByHeight * Store.columns > canvasWidth) ? rectangleSizeByWidth : rectangleSizeByHeight;

            // Calculate seat offset on X-axis to center them
            double seatsLeftOffset = centerHorizontaly ? (canvasWidth - Store.columns * rectangleSize) / 2 : 0;
            double seatsTopOffset = centerVertically ? (canvasHeight - Store.rows * rectangleSize) / 2 : 0;

            // Clear current seats
            Store.seatsDisplay.Children.Clear();

            // Render Seats
            foreach (Seat seat in seats)
            {
                // Set new size
                seat.SetSize(rectangleSize);

                // Set coordinates of a seat
                Canvas.SetLeft(seat.rectangle, seat.Column * rectangleSize + seatsLeftOffset);
                Canvas.SetTop(seat.rectangle, seat.Row * rectangleSize + seatsTopOffset);

                // Add seat to seat canvas
                Store.seatsDisplay.Children.Add(seat.rectangle);
            }

        }

        private void CreateEmptySeats()
        {
            seats = new ObservableCollection<Seat> { };

            for (int row = 0; row < Store.rows; row++)
            {
                for (int column = 0; column < Store.columns; column++)
                {
                    seats.Add(new Seat(row, column, false));
                }
            }

        }

        public void GetSeatsFromDatabase()
        {
            string query = "SELECT * FROM tickets WHERE movie_id = @movieID";

            MySqlCommand command = new MySqlCommand(query, Store.connection);
            command.Parameters.AddWithValue("@movieID", ID);

            CreateEmptySeats();

            Store.connection.Open();
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int Row = reader.GetInt32("row");
                    int Column = reader.GetInt32("column");

                    // Occupy Seats
                    foreach (Seat seat in seats)
                    {
                        if (seat.Column == Column && seat.Row == Row)
                        {
                            seat.OccupySeat();
                        }
                    }

                }
            }
            Store.connection.Close();

        }

    }

}
