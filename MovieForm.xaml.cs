using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace Cinemania
{
    /// <summary>
    /// Interaction logic for MovieForm.xaml
    /// </summary>
    public partial class MovieForm : Window
    {
        public MovieForm()
        {
            InitializeComponent();
        }

        public void Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
