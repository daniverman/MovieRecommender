using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LumenWorks.Framework.IO.Csv;
using HtmlAgilityPack;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;
using System.ComponentModel;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Collections.Concurrent;

namespace MovieRecommender.Model
{
    class MyModel : INotifyPropertyChanged
    {
        static Dictionary<string, Movie> moviesDictionary; //key = movieId, value = movie object
        const string IMDB_BASE_URL = "http://www.imdb.com/title/tt";
        static ConcurrentQueue<Movie> moviesToGrab;

        public event PropertyChangedEventHandler PropertyChanged;

        public MyModel()
        {
            moviesDictionary = new Dictionary<string, Movie>();
            moviesToGrab = new ConcurrentQueue<Movie>();
            //load the movie database
            loadDatabase();
            //load the movie details from the api
            loadMoviesFromApi();
        }

        private void writeToCsvFile(Movie movie, bool first)
        {
            StringBuilder sb = new StringBuilder();
            if (first)
            {
                sb.AppendLine("Id,Title,Year,Poster,Rating,Plot,Genres");
            }
            else
            {
                sb.AppendLine(String.Format("{0},{1},{2},{3},{4},{5},{6}", movie.Id, "\"" + movie.MovieTitle + "\"", movie.Year, movie.Poster, movie.Rating, "\""+movie.Plot+"\"", movie.Genres));
            }
            string csvpath = "movies.csv";
            File.AppendAllText(csvpath, sb.ToString());
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

        private void loadDatabase()
        {
            //read movies.csv
            using (var fs = File.OpenRead(@"db/movies.csv"))
            using (var reader = new StreamReader(fs))
            {
                using (var csvReader = new CsvReader(reader, true))
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
                using (var csvReader = new CsvReader(reader, true))
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

        private void notifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
    }
}
