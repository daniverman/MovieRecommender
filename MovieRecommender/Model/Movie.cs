using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRecommender.Model
{
    public class Movie
    {
        String id;
        String movieTitle;
        List<String> genres;
        int year;
        String urlLink;
        double rating;
        string poster;
        string plot;

        public Movie(String movieId)
        {
            this.id = movieId;
        }



        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public string MovieTitle
        {
            get
            {
                return movieTitle;
            }
            set
            {
                if (!value.Contains("("))
                {
                    movieTitle = value;
                    return;
                }
                movieTitle = value.Substring(0, value.IndexOf('(')).Trim();
                getYear(value);
            }
        }

        private void getYear(string value)
        {
            string yearStr = value.Substring(value.IndexOf('(') + 1, 4);
            int yearNum;
            if (Int32.TryParse(yearStr, out yearNum))
            {
                //this is a valid year
                year = yearNum;
            }
            else
            {
                //there are more the one parenthesis
                yearStr = value.Substring(value.IndexOf('(', value.Length - 6) + 1, 4);
                if (Int32.TryParse(yearStr, out yearNum))
                {
                    //this is a valid year
                    year = yearNum;
                }
            }
        }

        public List<string> Genres
        {
            get { return genres; }
            set { genres = value; }
        }
        public int Year
        {
            get { return year; }
            set { year = value; }
        }
        public string UrlLink
        {
            get { return urlLink; }
            set { urlLink = value; }
        }
        public double Rating
        {
            get { return rating; }
            set { rating = value; }
        }
        public string Poster
        {
            get { return poster; }
            set { poster = value; }
        }
        public string Plot
        {
            get { return plot; }
            set { plot = value; }
        }

        public static List<string> splitToGenres(string genresLine)
        {
            string[] genresArr = genresLine.Split('|');
            List<string> genresList = new List<string>();
            foreach (string genre in genresArr)
            {
                genresList.Add(genre);
            }
            return genresList;
        }

        public override string ToString()
        {
            return "Id: " + id + ", " + movieTitle + " - ";
        }

        internal static double extractRating(dynamic value)
        {
            string[] ratingArr = (value.Value as string).Split('/');
            string ratingStr = ratingArr[0];
            return Double.Parse(ratingStr);
        }
    }
}
