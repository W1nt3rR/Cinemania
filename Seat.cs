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
        public bool Taken { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }

        public Seat(int row, int column, bool taken)
        {
            this.Row = row;
            this.Column = column;
            this.Taken = taken;

            // Set color based on if it the seat is taken
            Brush customColor = this.Taken ? new SolidColorBrush(Colors.Red) : new SolidColorBrush(Colors.Green);

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

            // Emulate selecion of a seat
            if (Store.selectedSeat == null)
            {
                Store.selectedSeat = this;
                SelectSeat();
            }
            else
            {
                Store.selectedSeat.UnselectSeat();
                Store.selectedSeat = this;
                SelectSeat();
            }

        }

        private void SetColor(Color color)
        {
            this.rectangle.Fill = new SolidColorBrush(color);
        }

        public void OccupySeat()
        {
            Taken = true;
            SetColor(Colors.Red);
        }

        private void SelectSeat()
        {
            SetColor(Colors.Purple);
        }

        private void UnselectSeat()
        {
            SetColor(Colors.Green);
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
