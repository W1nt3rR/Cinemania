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
        public MainWindow()
        {
            InitializeComponent();
            
            Store.seatsDisplay = SeatsDisplay;
            Store.Movies = Movies;

            // Initialize Database Connection
            Store.connection = new MySqlConnection(Store.connectionString);

            // Initialize Tables
            Store.CreateTableIfNotExists();

            // Load movies from Database
            Store.GetMoviesFromDatabase();

            // Update filtered Movies
            Store.UpdateFilteredMovies();

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
            int hours;
            int minutes;

            try
            {
                hours = int.Parse(movieForm.hours.Text);
                minutes = int.Parse(movieForm.minutes.Text);
            }
            catch (Exception)
            {
                return;
            }

            // Validation
            if (name == "" || hours < 0 || hours > 23 || minutes < 0 || minutes > 59)
                return;

            // Return if no date provided
            DateTime date;
            try
            {
                // Get selected Date
                date = (DateTime)movieForm.Date.SelectedDate;
            }
            catch (Exception)
            {
                return;
            }

            // Add hours and minutes
            date = date.Add(new TimeSpan(hours, minutes, 0));

            // Create movie based on given Data
            Movie movie = new Movie(0, name, date);

            // Store movie
            movie.StoreToDatabase();

            // Refresh movies
            Store.RefreshMovies();
        }

        private void HandleDateChanged(object sender, RoutedEventArgs e)
        {
            // Return full list if date is not set
            if (PickedDate.SelectedDate == null)
            {
                Store.UpdateFilteredMovies();
                return;
            }

            // Get selected date
            DateTime date = (DateTime)PickedDate.SelectedDate;

            // Clear the current filtered list of Movies
            Store.filteredMovies.Clear();

            // Filter movies by date
            foreach (Movie movie in Store.allMovies)
            {
                if (movie.Date.Date == date.Date)
                {
                    Store.filteredMovies.Add(movie);
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

            if (Store.selectedSeat != null)
            {
                Store.selectedSeat.UnselectSeat();
                Store.selectedSeat = null;
            }

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

        public void ReserveTicket(object sender, RoutedEventArgs e)
        {
            Store.ReserveTicket();
        }

        public void CancelTicket(object sender, RoutedEventArgs e)
        {
            // Open form
            CancelTicketForm cancelTicketForm = new CancelTicketForm();
            cancelTicketForm.ShowDialog();

            int ticketID;

            // Get ticket id from user
            try
            {
                ticketID = int.Parse(cancelTicketForm.TicketID.Text);
            }
            catch (Exception)
            {
                return;
            }

            // Remove ticket from DB
            Ticket.CancelTicket(ticketID);

            // Update movies
            Store.RefreshMovies();
        }

    }

}
