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
    class MyViewModel : INotifyPropertyChanged
    {
        MyModel model;

        private ObservableCollection<Movie> moviesList;

        public ObservableCollection<Movie> VM_MoviesList
        {
            get { return model.MoviesList; }
            set { moviesList = value;
                notifyPropertyChanged("VM_MoviesList");
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
    }
}
