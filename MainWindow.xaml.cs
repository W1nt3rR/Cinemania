using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Cinemania
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Movie> allMovies;
        public ObservableCollection<Movie> filteredMovies;

        public MainWindow()
        {
            InitializeComponent();

            Store.seatsDisplay = SeatsDisplay;

            // Load movies
            allMovies = new ObservableCollection<Movie>
            {
                new Movie("John Wick Chapter 1", new DateTime(2023, 6, 17, 18, 00, 0)),
                new Movie("John Wick Chapter 1", new DateTime(2023, 6, 17, 19, 30, 0)),
                new Movie("John Wick Chapter 2", new DateTime(2023, 6, 17, 19, 30, 0)),
                new Movie("John Wick Chapter 3", new DateTime(2023, 6, 17, 21, 0, 0)),
                new Movie("John Wick Chapter 4", new DateTime(2023, 6, 17, 22, 30, 0)),
                new Movie("Rambo 1", new DateTime(2023, 6, 17, 18, 00, 0)),
                new Movie("Rambo 2", new DateTime(2023, 6, 17, 19, 30, 0)),
                new Movie("Rambo 3", new DateTime(2023, 6, 16, 21, 00, 0)),
                new Movie("Rambo 4", new DateTime(2023, 6, 18, 22, 30, 0)),
            };

            PickedDate.SelectedDateChanged += HandleDateChanged;

            // Copy all movies into filteredMovies
            filteredMovies = new ObservableCollection<Movie>(allMovies);

            Movies.ItemsSource = filteredMovies;

        }

        private void HandleDateChanged(object sender, RoutedEventArgs e)
        {
            // Return full list if date is not set
            if (PickedDate.SelectedDate == null)
            {
                filteredMovies = new ObservableCollection<Movie>(allMovies);
                Movies.ItemsSource = filteredMovies;
                return;
            }

            // Get selected date
            DateTime date = (DateTime)PickedDate.SelectedDate;

            // Clear the current filtered list of Movies
            filteredMovies.Clear();

            // Filter movies by date
            foreach (Movie movie in allMovies)
            {
                if (movie.Date.Date == date.Date)
                {
                    filteredMovies.Add(movie);
                }
            }

        }

        private void HandleMovieClick(object sender, RoutedEventArgs e)
        {
            Border moveItemBorder = (Border)sender;

            // Return previously selected item back to normal, if set.
            if (Store.selectedMovieItem != null)
            {
                Store.selectedMovieItem.BorderBrush = new SolidColorBrush(Colors.Black);
                Store.selectedMovieItem.BorderThickness = new Thickness(2);
            }

            // Make movie item look selected
            Store.selectedMovieItem = moveItemBorder;
            moveItemBorder.BorderBrush = new SolidColorBrush(Colors.Magenta);
            moveItemBorder.BorderThickness = new Thickness(3);

            // Render seats of a selected movie
            Movie selectedMovie = (Movie)Store.selectedMovieItem.DataContext;
            selectedMovie.RenderSeats();
        }

        private void ItemMouseEnter(object sender, RoutedEventArgs e)
        {
            Border moveItemBorder = (Border)sender;

            // Skip selected Item
            if (Store.selectedMovieItem == moveItemBorder)
                return;

            // Emulate Hover Effect
            moveItemBorder.BorderBrush = new SolidColorBrush(Colors.Red);
        }

        private void ItemMouseLeave(object sender, RoutedEventArgs e)
        {
            Border moveItemBorder = (Border)sender;

            // Skip selected Item
            if (Store.selectedMovieItem == moveItemBorder)
                return;

            // Emulate Hover Effect
            moveItemBorder.BorderBrush = new SolidColorBrush(Colors.Black);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            // Do not re-render if nothing to re-render
            if (Store.selectedMovieItem == null)
                return;

            // Re-render seats to new size
            Movie selectedMovie = (Movie)Store.selectedMovieItem.DataContext;
            selectedMovie.RenderSeats();
        }

    }

}
