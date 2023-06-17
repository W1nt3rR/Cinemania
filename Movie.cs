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
        public bool reserved = false;

        public ObservableCollection<Seat> seats;

        public Movie(string name, DateTime date) 
        {
            this.Name = name;
            this.Date = date;
        }

        public void RenderSeats()
        {
            double canvasWidth = Store.seatsDisplay.ActualWidth;
            double canvasHeight = Store.seatsDisplay.ActualHeight;

            //double rectangleSize = canvasWidth / columns;
            double rectangleSize = canvasHeight / Store.rows;

            Store.seatsDisplay.Children.Clear();

            if (seats == null)
            {
                CreateEmptySeats();
            }

            for (int i = 0; i < Store.rows; i++)
            {
                for (int j = 0; j < Store.columns; j++)
                {

                    Seat seat = seats[j * Store.rows + i];

                    seat.SetSize(rectangleSize);

                    Canvas.SetLeft(seat.rectangle, j * rectangleSize);
                    Canvas.SetTop(seat.rectangle, i * rectangleSize);

                    Store.seatsDisplay.Children.Add(seat.rectangle);
                }

            }
        }

        private void CreateEmptySeats()
        {
            double canvasHeight = Store.seatsDisplay.ActualHeight;

            //double rectangleSize = canvasWidth / columns;
            double rectangleSize = canvasHeight / Store.rows;

            seats = new ObservableCollection<Seat> { };


            for (int i = 0; i < Store.rows; i++)
            {
                for (int j = 0; j < Store.columns; j++)
                {
                    Seat seat = new Seat(rectangleSize, Store.marginSize);
                    seats.Add(seat);
                }

            }

        }

    }

}
