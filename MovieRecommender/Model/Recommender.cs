using LumenWorks.Framework.IO.Csv;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRecommender.Model
{
    internal class Recommender
    {
        private Dictionary<string, Movie> m_moviesDictionary;
        private Dictionary<string, User> m_usersDict = new Dictionary<string, User>();

        public Recommender(Dictionary<string, Movie> moviesDictionary)
        {
            this.m_moviesDictionary = moviesDictionary;
            loadUsers();
        }

        private void loadUsers()
        {
            using (var fs = File.OpenRead(@"ratings.csv"))
            using (var reader = new StreamReader(fs))
            {
                using (var csvReader = new CsvReader(reader, true))
                {
                    User newUser;
                    string currentUser = "";
                    newUser = new User();

                    while (csvReader.ReadNextRecord())
                    {
                        if (currentUser != csvReader[0])
                        {
                            currentUser = csvReader[0];
                            newUser = new User();
                            m_usersDict[currentUser] = newUser;
                            m_usersDict[currentUser].UserId = Int32.Parse(csvReader[0]);
                        }
                        m_usersDict[currentUser].UserMovies.Add(csvReader[1], Double.Parse(csvReader[2]));
                    }
                }
            }
        }
    }
}