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

namespace MovieRecommender.View
{
    /// <summary>
    /// Interaction logic for MoviesViewer.xaml
    /// </summary>
    public partial class MoviesViewer : UserControl
    {
        MyViewModel vm;

        public MoviesViewer(MyViewModel vm)
        {
            InitializeComponent();
            this.DataContext = vm;
            this.vm = vm;
        }
    }
}
