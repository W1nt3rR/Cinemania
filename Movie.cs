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
        public bool Reserved { get; set; }

        public ObservableCollection<Seat> seats;

        public Movie(int id, string name, DateTime date, Seat[] seats = null, bool reserved = false)
        {
            this.ID = id;
            this.Name = name;
            this.Date = date;
            this.Reserved = reserved;

            // Create new random seats
            if (seats == null)
                CreateRandomSeats();
            else
                this.seats = new ObservableCollection<Seat>(seats);
        }

        public void StoreToDatabase()
        {
            string query = "INSERT INTO movies (name, seats, datetime, user_reserved) VALUES (@Name, @seats, @datetime, @user_reserved)";

            MySqlCommand command = new MySqlCommand(query, Store.connection);
            command.Parameters.AddWithValue("@name", Name);
            command.Parameters.AddWithValue("@seats", JsonSerializer.Serialize(seats));
            command.Parameters.AddWithValue("@datetime", Date);
            command.Parameters.AddWithValue("@user_reserved", Reserved);

            Store.connection.Open();
            command.ExecuteNonQuery();
            Store.connection.Close();
        }

        public void UpdateInDatabase()
        {
            string query = "UPDATE movies SET name = @name, seats = @seats, datetime = @datetime, user_reserved = @user_reserved WHERE ID = @id";

            MySqlCommand command = new MySqlCommand(query, Store.connection);
            command.Parameters.AddWithValue("@id", ID);
            command.Parameters.AddWithValue("@name", Name);
            command.Parameters.AddWithValue("@seats", JsonSerializer.Serialize(seats));
            command.Parameters.AddWithValue("@datetime", Date);
            command.Parameters.AddWithValue("@user_reserved", Reserved);

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

        private void CreateRandomSeats()
        {
            seats = new ObservableCollection<Seat> { };

            for (int row = 0; row < Store.rows; row++)
            {
                for (int column = 0; column < Store.columns; column++)
                {
                    // Generate a random Number
                    var num = Store.GenerateRandomNumber(100);

                    // Randomize taken seats
                    bool taken = (num > 75) || false;

                    seats.Add(new Seat(row, column, taken));
                }
            }

        }

    }

}
