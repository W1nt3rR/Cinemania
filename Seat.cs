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

        public Seat(double rectangleSize, double marginSize, bool taken = false) {

            Brush customColor;

            var num = GenerateRandomNumber();

            if (num > 60 )
            {
                taken = true;
            }

            this.taken = taken;

            if (taken)
                customColor = new SolidColorBrush(Colors.Red);
            else
                customColor = new SolidColorBrush(Colors.Green);

            double size = rectangleSize - marginSize * 2;

            int columns = Store.columns;

            this.rectangle = new Rectangle
            {
                Height = size,
                Width = size,
                Fill = customColor,
                Stroke = new SolidColorBrush(Colors.Black),
                Margin = new Thickness(marginSize),

            };

            this.rectangle.MouseLeftButtonUp += SeatClick;

        }

        private int GenerateRandomNumber()
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
            int finalNumber = Math.Abs(randomInt % 100) + 1;

            return finalNumber;
        }

        private void SeatClick(object sender, RoutedEventArgs e)
        {
            if (taken)
            {
                MessageBox.Show("Seat already taken by someone else", "Seat taken", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            bool movieReserved = Store.GetMovieReserved();

            if (movieReserved && reserved)
            {
                Store.SetMovieReserved(false);
                this.reserved = false;
                Rectangle clickedRectangle = sender as Rectangle;
                clickedRectangle.Fill = new SolidColorBrush(Colors.Green);
            }
            else if (!movieReserved && !reserved)
            {
                Store.SetMovieReserved(true);
                this.reserved = true;
                Rectangle clickedRectangle = sender as Rectangle;
                clickedRectangle.Fill = new SolidColorBrush(Colors.Purple);
            } 
            else
            {
                MessageBox.Show("You can only Reserve one seat.", "Seat already reserved", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

        }

        public void SetSize(double rectangleSize)
        {
            rectangle.Width = rectangleSize - Store.marginSize * 2;
            rectangle.Height = rectangleSize - Store.marginSize * 2;
        }

    }

}
