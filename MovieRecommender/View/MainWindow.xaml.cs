using MovieRecommender.Model;
using MovieRecommender.View;
using MovieRecommender.ViewModel;
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

namespace MovieRecommender
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MyViewModel vm;

        public MainWindow()
        {
            InitializeComponent();
            vm = new MyViewModel(new MyModel());
            this.DataContext = vm;
        }

        /// <summary>
        /// open moviesViewer panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mainGrid.Children.Clear();
            MoviesViewer mv = new MoviesViewer(vm);
            mainGrid.Children.Add(mv);
        }
    }
}