using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

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

    }

}
