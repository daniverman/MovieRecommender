using MovieRecommender.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRecommender.ViewModel
{
    public class MyViewModel : INotifyPropertyChanged
    {
        private MyModel model;

        private ObservableCollection<Movie> moviesList;

        public ObservableCollection<Movie> VM_MoviesList
        {
            get { return model.MoviesList; }
            set
            {
                moviesList = value;
                notifyPropertyChanged("VM_MoviesList");
            }
        }

        private ObservableCollection<Movie> moviesListSmal;

        public ObservableCollection<Movie> VM_MoviesListSmall
        {
            get { return model.MoviesListSmall; }
            set
            {
                moviesList = value;
                notifyPropertyChanged("VM_MoviesList");
            }
        }

        private List<string> genres;

        public List<string> VM_Genres
        {
            get { return model.Genres; }
            set
            {
                genres = value;
                notifyPropertyChanged("VM_Genres");
            }
        }

        public MyViewModel(MyModel model)
        {
            this.model = model;
            model.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e)
            {
                notifyPropertyChanged("VM_" + e.PropertyName);
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void notifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        internal List<Movie> GetSuggested(ObservableCollection<Movie> selctedMovies)
        {
            return model.GetSuggested(selctedMovies);
        }
    }
}