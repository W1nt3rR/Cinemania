using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Cinemania
{
    public class Movie
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public bool reserved { get; set; } = false;

        public ObservableCollection<Seat> seats;

        public Movie(string name, DateTime date)
        {
            this.Name = name;
            this.Date = date;
        }

        public void RenderSeats()
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
            double seatsLeftOffset = (canvasWidth - Store.columns * rectangleSize) / 2;
            double seatsTopOffset = (canvasHeight - Store.rows * rectangleSize) / 2;

            // Clear current seats
            Store.seatsDisplay.Children.Clear();

            // Create new random seats
            if (seats == null)
            {
                CreateEmptySeats();
            }

            // Render Seats
            for (int i = 0; i < Store.rows; i++)
            {
                for (int j = 0; j < Store.columns; j++)
                {
                    // Get seat from array
                    Seat seat = seats[j * Store.rows + i];

                    // Set seats size
                    seat.SetSize(rectangleSize);

                    // Set coordinates of a seat
                    Canvas.SetLeft(seat.rectangle, j * rectangleSize + seatsLeftOffset);
                    Canvas.SetTop(seat.rectangle, i * rectangleSize);

                    // Add seat to seat canvas
                    Store.seatsDisplay.Children.Add(seat.rectangle);
                }
            }

        }

        private void CreateEmptySeats()
        {
            seats = new ObservableCollection<Seat> { };

            for (int i = 0; i < Store.rows; i++)
            {
                for (int j = 0; j < Store.columns; j++)
                {
                    Seat seat = new Seat();
                    seats.Add(seat);
                }

            }

        }

    }

}
