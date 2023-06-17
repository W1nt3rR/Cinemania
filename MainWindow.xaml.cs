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
using MySql.Data.MySqlClient;

namespace Cinemania
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Movie> filteredMovies;

        public MainWindow()
        {
            InitializeComponent();
            
            Store.seatsDisplay = SeatsDisplay;

            // Initialize Database
            Store.connection = new MySqlConnection(Store.connectionString);

            // Load movies from Database
            Store.GetMoviesFromDatabase();

            // Copy all movies into filteredMovies
            filteredMovies = new ObservableCollection<Movie>(Store.allMovies);

            // Add filteredMovies to Movies items
            Movies.ItemsSource = filteredMovies;

            // Setup Event Handlers
            PickedDate.SelectedDateChanged += HandleDateChanged;

        }

        private void OpenMovieForm(object sender, RoutedEventArgs e)
        {
            // Create and Show Dialog Form
            MovieForm movieForm = new MovieForm();
            movieForm.ShowDialog();

            // Get movie Data
            string name = movieForm.MovieName.Text;

            // Validation
            if (name == "")
                return;

            // Return if no date provided
            DateTime date;
            try
            {
                date = (DateTime)movieForm.Date.SelectedDate;
            }
            catch (Exception)
            {
                return;
            }

            // Create movie based on given Data
            Movie movie = new Movie(0, name, date);

            // Store movie to databse to get ID
            movie.StoreToDatabase();

            // Get all movies including new movie
            Store.GetMoviesFromDatabase();

            // Update filteredMovies list to show new movie
            UpdateFilteredMovies();
        }

        private void UpdateFilteredMovies()
        {
            // Updates filtered list
            filteredMovies = new ObservableCollection<Movie>(Store.allMovies);
            Movies.ItemsSource = filteredMovies;
        }

        private void HandleDateChanged(object sender, RoutedEventArgs e)
        {
            // Return full list if date is not set
            if (PickedDate.SelectedDate == null)
            {
                UpdateFilteredMovies();
                return;
            }

            // Get selected date
            DateTime date = (DateTime)PickedDate.SelectedDate;

            // Clear the current filtered list of Movies
            filteredMovies.Clear();

            // Filter movies by date
            foreach (Movie movie in Store.allMovies)
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
