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
        public ObservableCollection<Seat> allSeats;
        public ObservableCollection<Movie> allMovies;
        public ObservableCollection<Movie> filteredMovies;
        public Border selectedItem;

        public MainWindow()
        {
            InitializeComponent();

            Store.seatsDisplay = SeatsDisplay;

            allMovies = new ObservableCollection<Movie>
            {
                new Movie("John Wick Chapter 1", new DateTime(2023, 6, 17, 18, 00, 0)),
                new Movie("John Wick Chapter 1", new DateTime(2023, 6, 17, 19, 30, 0)),
                new Movie("John Wick Chapter 2", new DateTime(2023, 6, 17, 19, 30, 0)),
                new Movie("John Wick Chapter 3", new DateTime(2023, 6, 17, 21, 0, 0)),
                new Movie("John Wick Chapter 4", new DateTime(2023, 6, 17, 22, 30, 0)),
                new Movie("Rambo 1", new DateTime(2023, 6, 17, 18, 00, 0)),
                new Movie("Rambo 2", new DateTime(2023, 6, 17, 19, 30, 0)),
                new Movie("Rambo 3", new DateTime(2023, 6, 17, 21, 00, 0)),
                new Movie("Rambo 4", new DateTime(2023, 6, 17, 22, 30, 0)),
            };

            PickedDate.SelectedDateChanged += HandleDateChanged;

            MovieSelect.ItemsSource = allMovies;
            MovieSelect.SelectionChanged += SelectChanged;

            filteredMovies = new ObservableCollection<Movie>(allMovies);

            Movies.ItemsSource = filteredMovies;

        }

        private void HandleDateChanged(object sender, RoutedEventArgs e)
        {
            if (PickedDate.SelectedDate == null)
            {
                filteredMovies = new ObservableCollection<Movie>(allMovies);
                Movies.ItemsSource = filteredMovies;
                return;
            }

            DateTime date = (DateTime)PickedDate.SelectedDate;

            filteredMovies.Clear();

            foreach (var movie in allMovies)
            {
                if (DateTime.MinValue == date)
                {
                    filteredMovies.Add(movie);
                    return;
                }

                if (movie.Date.Date == date.Date)
                {
                    filteredMovies.Add(movie);
                }
            }

        }

        private void HandleMovieClick(object sender, RoutedEventArgs e)
        {
            Border border = (Border)sender;

            if (this.selectedItem != null)
            {
                this.selectedItem.BorderBrush = new SolidColorBrush(Colors.Black);
                this.selectedItem.BorderThickness = new Thickness(2);
            }

            this.selectedItem = border;
            border.BorderBrush = new SolidColorBrush(Colors.Magenta);
            border.BorderThickness = new Thickness(3);


            Movie selectedMovie = (Movie)this.selectedItem.DataContext;
            selectedMovie.RenderSeats();
        }

        private void SelectChanged(object sender, RoutedEventArgs e)
        {
            ComboBox select = sender as ComboBox;
            Movie selectedMovie = select.SelectedItem as Movie;
            MessageBox.Show(selectedMovie.Name);
        }

        //protected override void OnContentRendered(EventArgs e)
        //{
        //    base.OnContentRendered(e);
        //    RenderSquares();
        //}

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            if (selectedItem == null)
            {
                return;
            }

            Movie selectedMovie = (Movie)selectedItem.DataContext;
            selectedMovie.RenderSeats();
        }

    }

}
