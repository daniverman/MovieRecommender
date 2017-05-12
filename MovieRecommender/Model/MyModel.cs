using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LumenWorks.Framework.IO.Csv;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;
using System.ComponentModel;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Collections.Concurrent;
using CsvHelper;
using System.Collections.ObjectModel;

namespace MovieRecommender.Model
{
    public class MyModel : INotifyPropertyChanged
    {
        private static Random rand = new Random();
        private List<string> genres = new List<string>();

        public List<string> Genres
        {
            get { return genres; }
            set
            {
                genres = value;
                notifyPropertyChanged("Genres");
            }
        }

        private Dictionary<string, ObservableCollection<Movie>> movieByGenreFull = new Dictionary<string, ObservableCollection<Movie>>();

        public Dictionary<string, ObservableCollection<Movie>> MovieByGenreFull
        {
            get { return movieByGenreFull; }
            set
            {
                movieByGenreFull = value;
                notifyPropertyChanged("MovieByGenreFull");
            }
        }

        private Dictionary<string, ObservableCollection<Movie>> movieByGenreSmall = new Dictionary<string, ObservableCollection<Movie>>();

        public Dictionary<string, ObservableCollection<Movie>> MovieByGenreSmall
        {
            get { return movieByGenreSmall; }
            set
            {
                movieByGenreSmall = value;
                notifyPropertyChanged("MovieByGenreSmall");
            }
        }

        private ObservableCollection<Movie> moviesList = new ObservableCollection<Movie>();

        public ObservableCollection<Movie> MoviesList
        {
            get { return moviesList; }
            set
            {
                moviesList = value;
                notifyPropertyChanged("MoviesList");
            }
        }

        internal List<Movie> GetSuggested(ObservableCollection<Movie> selctedMovies)
        {
            Dictionary<string, Movie> selctedMoviesDictionary = new Dictionary<string, Movie>();
            Dictionary<string, double> selctedMoviesRatingDictionary = new Dictionary<string, double>();

            foreach (Movie movie in selctedMovies)
            {
                double rating = rand.Next(1, 5);
                selctedMoviesDictionary.Add(movie.Id, movie);
                selctedMoviesRatingDictionary.Add(movie.Id, rating);
            }
            User user = new User();
            user.UserId = 999;
            user.UserMovies = selctedMoviesRatingDictionary;
            Recommender recommender = new Recommender(moviesDictionary);
            List<string> recMovieList = recommender.suggestList(user);
            List<Movie> ans = new List<Movie>();
            for (int i = 0; i < 10; i++)
            {
                ans.Add(moviesDictionary[recMovieList[i]]);
            }
            return ans;
        }

        public ObservableCollection<Movie> showGenre(string genre)
        {
            return movieByGenreSmall[genre];
        }

        internal bool searchMovie(string movieToSearch)
        {
            results.Clear();
            bool exist = false;
            foreach (Movie movie in moviesList)
            {
                if (movie.MovieTitle.ToLower().Contains(movieToSearch.ToLower()))
                {
                    results.Add(movie);
                    exist = true;
                }
            }
            if (exist)
            {
                return true;
            }
            return false;
        }

        private ObservableCollection<Movie> results = new ObservableCollection<Movie>();

        public ObservableCollection<Movie> Results
        {
            get { return results; }
            set
            {
                results = value;
                notifyPropertyChanged("Results");
            }
        }

        private ObservableCollection<Movie> smallMovieList = new ObservableCollection<Movie>();

        public ObservableCollection<Movie> MoviesListSmall
        {
            get { return smallMovieList; }
            set
            {
                smallMovieList = value;
                notifyPropertyChanged("MoviesListSmall");
            }
        }

        private static Dictionary<string, Movie> moviesDictionary; //key = movieId, value = movie object
        private const string IMDB_BASE_URL = "http://www.imdb.com/title/tt";
        private static ConcurrentQueue<Movie> moviesToGrab;

        public event PropertyChangedEventHandler PropertyChanged;

        public MyModel()
        {
            moviesDictionary = new Dictionary<string, Movie>();
            moviesToGrab = new ConcurrentQueue<Movie>();
            if (!File.Exists("movies.csv"))
            {
                //load the movie database
                loadDatabase();
                //load the movie details from the api
                loadMoviesFromApi();
            }
            else
            {
                //read movies file
                loadCompleteDatabase();
            }
            createMoviesListSmall(200);
            //Recommender rec = new Recommender(moviesDictionary);
            createGenreMoviesDictionary();
        }

        private void createGenreMoviesDictionary()
        {
            genres.Add("All");
            movieByGenreFull["All"] = smallMovieList;
            movieByGenreSmall["All"] = smallMovieList;
            //
            List<Movie> sortedListMovieRating = new List<Movie>();
            foreach (Movie movie in moviesList)
            {
                sortedListMovieRating.Add(movie);
            }
            sortedListMovieRating.Sort((y, x) => x.Rating.CompareTo(y.Rating));

            foreach (Movie movie in sortedListMovieRating)
            {
                foreach (string genre in movie.Genres)
                {
                    if (!movieByGenreFull.ContainsKey(genre))
                    {
                        movieByGenreFull[genre] = new ObservableCollection<Movie>();
                        movieByGenreSmall[genre] = new ObservableCollection<Movie>();
                        genres.Add(genre);
                    }
                    movieByGenreFull[genre].Add(movie);
                    if (movieByGenreSmall[genre].Count < 200)
                    {
                        if (movie.Rating <= 10)
                        {
                            movieByGenreSmall[genre].Add(movie);
                        }
                    }
                }
            }
        }

        private void createMoviesListSmall(int numOfMovies)
        {
            List<Movie> normalList = new List<Movie>();

            foreach (Movie m in moviesList)
            {
                normalList.Add(m);
            }
            normalList.Sort((y, x) => x.Rating.CompareTo(y.Rating));
            //
            int i = 0;
            foreach (Movie movie in normalList)
            {
                if (movie.Rating < 10)
                {
                    if (i > numOfMovies)
                    {
                        break;
                    }
                    smallMovieList.Add(movie);
                    i++;
                }
            }
        }

        private void loadMoviesFromApi()
        {
            writeToCsvFile(null, true);
            int i = 0;
            Movie movie;
            while (i < moviesDictionary.Count)
            {
                moviesToGrab.TryDequeue(out movie);
                extractMovieDetails(movie);
                writeToCsvFile(movie, false);
                Console.WriteLine(i);
                i++;
            }
        }

        private void loadDatabase()
        {
            //read movies.csv
            using (var fs = File.OpenRead(@"db/movies.csv"))
            using (var reader = new StreamReader(fs))
            {
                using (var csvReader = new LumenWorks.Framework.IO.Csv.CsvReader(reader, true))
                {
                    while (csvReader.ReadNextRecord())
                    {
                        //the first line is the headers line (movieID,Title,genres)
                        Movie movie = new Movie(csvReader[0]);
                        movie.MovieTitle = csvReader[1];
                        movie.Genres = Movie.splitToGenres(csvReader[2]);
                        moviesDictionary[csvReader[0]] = movie;
                        moviesToGrab.Enqueue(movie);
                    }
                }
            }

            using (var fs = File.OpenRead(@"db/links.csv"))
            using (var reader = new StreamReader(fs))
            {
                using (var csvReader = new LumenWorks.Framework.IO.Csv.CsvReader(reader, true))
                {
                    while (csvReader.ReadNextRecord())
                    {
                        if (moviesDictionary.ContainsKey(csvReader[0]))
                        {
                            moviesDictionary[csvReader[0]].UrlLink = "tt" + csvReader[1]; //add the IMDB link
                        }
                    }
                }
            }
        }

        private void loadCompleteDatabase()
        {
            using (var fs = File.OpenRead(@"movies.csv"))
            using (var reader = new StreamReader(fs))
            {
                using (var csvReader = new CsvHelper.CsvReader(reader))
                {
                    while (csvReader.Read())
                    {
                        //the first line is the headers line (movieID,Title,genres)
                        Movie movie = new Movie(csvReader[0]);
                        movie.MovieTitle = csvReader[1];
                        movie.Year = csvReader.GetField<int>(2);
                        movie.Poster = csvReader.GetField<string>(3);
                        movie.Rating = csvReader.GetField<double>(4);
                        movie.Plot = csvReader.GetField<string>(5);
                        moviesDictionary[csvReader[0]] = movie;
                    }
                }
            }
            using (var fs = File.OpenRead(@"db/movies.csv"))
            using (var reader = new StreamReader(fs))
            {
                using (var csvReader = new CsvHelper.CsvReader(reader))
                {
                    while (csvReader.Read())
                    {
                        if (moviesDictionary.ContainsKey(csvReader[0]))
                        {
                            moviesDictionary[csvReader[0]].Genres = Movie.splitToGenres(csvReader[2]);
                            moviesList.Add(moviesDictionary[csvReader[0]]);
                        }
                    }
                }
            }
        }

        private void writeToCsvFile(Movie movie, bool first)
        {
            StringBuilder sb = new StringBuilder();
            if (first)
            {
                sb.AppendLine("Id,Title,Year,Poster,Rating,Plot");
            }
            else
            {
                sb.AppendLine(String.Format("{0},{1},{2},{3},{4},{5}", movie.Id, "\"" + movie.MovieTitle + "\"", movie.Year, movie.Poster, movie.Rating, "\"" + movie.Plot + "\"", movie.Genres));
            }
            string csvpath = "movies.csv";
            File.AppendAllText(csvpath, sb.ToString());
        }

        private static void extractMovieDetails(Movie movie)
        {
            string apiUrl = "http://www.omdbapi.com/?i=" + movie.UrlLink;
            HttpWebRequest request = WebRequest.Create(apiUrl) as HttpWebRequest;
            request.Timeout = 1000 * 60 * 30;
            request.KeepAlive = true;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            var encoding = ASCIIEncoding.ASCII;
            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
            {
                string responseText = reader.ReadToEnd();
                parseJson(responseText, movie);
            }
        }

        private static void parseJson(string responseText, Movie movie)
        {
            JObject json = JsonConvert.DeserializeObject<dynamic>(responseText);
            JArray movieRating = (JArray)json.GetValue("Ratings");
            try
            {
                movie.Rating = Movie.extractRating(((JObject)movieRating[0]).GetValue("Value"));
            }
            catch (Exception e)
            {
                movie.Rating = -1;
            }
            try
            {
                movie.Poster = (string)json.GetValue("Poster");
            }
            catch (Exception e)
            {
                movie.Poster = "";
            }
            movie.Plot = (string)json.GetValue("Plot");
        }

        private void notifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}