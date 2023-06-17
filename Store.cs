using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Cinemania
{
    public static class Store
    {
        public static int rows = 5;
        public static int columns = 10;
        public static int marginSize = 4;
        public static Canvas seatsDisplay;
        public static Border selectedItem;

        public static bool GetMovieReserved()
        {
            Movie selectedMovie = (Movie)selectedItem.DataContext;
            return selectedMovie.reserved;
        }

        public static void SetMovieReserved(bool newValue)
        {
            Movie selectedMovie = (Movie)selectedItem.DataContext;
            selectedMovie.reserved = newValue;
        }

    }

}
