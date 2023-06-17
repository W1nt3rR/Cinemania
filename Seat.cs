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

        public Rectangle rectangle;
        private bool reserved = false;
        private bool taken;

        public Seat(bool taken = false)
        {
            // Generate a random Number
            var num = GenerateRandomNumber(100);

            // Randomize taken seats
            if (num > 75)
                taken = true;

            this.taken = taken;

            // Set color based on if it the seat is taken
            Brush customColor = taken ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green);

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

        private int GenerateRandomNumber(int maxNumber)
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

        private void SeatClick(object sender, RoutedEventArgs e)
        {
            // Prevent reservation of already taken seats
            if (taken)
            {
                MessageBox.Show("Seat already taken by someone else", "Seat taken", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            Rectangle clickedRectangle = (Rectangle)sender;

            bool movieReserved = Store.GetSelectedMovieReservedStatus();

            // Allow only one reservation for User
            if (movieReserved && reserved)
            {
                Store.SetSelectedMovieReservedStatus(false);
                this.reserved = false;
                clickedRectangle.Fill = new SolidColorBrush(Colors.Green);
            }
            else if (!movieReserved && !reserved)
            {
                Store.SetSelectedMovieReservedStatus(true);
                this.reserved = true;
                clickedRectangle.Fill = new SolidColorBrush(Colors.Purple);
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
