using MovieRecommender.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

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

        private ObservableCollection<Movie> moviesListSmall;

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

        private ObservableCollection<Movie> results;

        public ObservableCollection<Movie> VM_Results
        {
            get { return model.Results; }
            set { results = value;
                notifyPropertyChanged("VM_Results");
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

        public ObservableCollection<Movie> showGenre(string genre)
        {
            return model.showGenre(genre);
        }

        internal bool searchMovie(string movieToSearch)
        {
            return model.searchMovie(movieToSearch);
        }
    }
}