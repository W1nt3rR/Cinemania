using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;
using System.Security.Cryptography;

namespace Cinemania
{
    public class Seat
    {
        // Rectnagle representing a seat
        public Rectangle rectangle;
        public bool Reserved { get; set; } = false;
        public bool Taken { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }

        public Seat(int row, int column, bool taken, bool reserved = false)
        {
            this.Row = row;
            this.Column = column;
            this.Taken = taken;
            this.Reserved = reserved;

            // Set color based on if it the seat is taken
            Brush customColor = this.Reserved ? new SolidColorBrush(Colors.Purple) : this.Taken ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green);

            // Create a rectangle representing a seat
            this.rectangle = new Rectangle
            {
                Height = 0,
                Width = 0,
                Fill = customColor,
                Stroke = new SolidColorBrush(Colors.Black),
                Margin = new Thickness(Store.marginSize),

            };

            // Setup event listeners
            // Click register
            this.rectangle.MouseLeftButtonUp += SeatClick;
            // Hover emulation
            this.rectangle.MouseLeave += SeatLeave;
            this.rectangle.MouseEnter += SeatEnter;

        }

        private void SeatClick(object sender, RoutedEventArgs e)
        {
            // Prevent reservation of already taken seats
            if (Taken)
            {
                MessageBox.Show("Seat already taken by someone else", "Seat taken", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Rectangle clickedRectangle = (Rectangle)sender;

            bool movieReserved = Store.GetSelectedMovieReservedStatus();

            // Allow only one reservation for User
            if (movieReserved && Reserved)
            {
                Store.SetSelectedMovieReservedStatus(false);
                this.Reserved = false;
                clickedRectangle.Fill = new SolidColorBrush(Colors.Green);
                Store.UpdateSelectedMovieInDatabase();
            }
            else if (!movieReserved && !Reserved)
            {
                Store.SetSelectedMovieReservedStatus(true);
                this.Reserved = true;
                clickedRectangle.Fill = new SolidColorBrush(Colors.Purple);
                Store.UpdateSelectedMovieInDatabase();
            } 
            else
            {
                MessageBox.Show("You can only Reserve one seat.", "Seat already reserved", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

        }

        private void SeatEnter(object sender, RoutedEventArgs e)
        {
            // Create hover effect
            Rectangle clickedRectangle = sender as Rectangle;
            clickedRectangle.StrokeThickness = 3;
            clickedRectangle.Stroke = new SolidColorBrush(Colors.Magenta);
        }

        private void SeatLeave(object sender, RoutedEventArgs e)
        {
            // Remove hover effect
            Rectangle clickedRectangle = sender as Rectangle;
            clickedRectangle.StrokeThickness = 1;
        }

        public void SetSize(double rectangleSize)
        {
            // Set seat size
            rectangle.Width = rectangleSize - Store.marginSize * 2;
            rectangle.Height = rectangleSize - Store.marginSize * 2;
        }

    }

}
