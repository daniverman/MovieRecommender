using MovieRecommender.Model;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MovieRecommender.View.Controls
{
    /// <summary>
    /// Interaction logic for MovieListItem.xaml
    /// </summary>
    public partial class MovieListItem : UserControl
    {
        public MovieListItem()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty MovieProperty =
            DependencyProperty.Register("movie", typeof(Movie), typeof(MovieListItem));

        public Movie movie
        {
            get { return (Movie)GetValue(MovieProperty); }
            set { SetValue(MovieProperty, value); }
        }


    }
}
