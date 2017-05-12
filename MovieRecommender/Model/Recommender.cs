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
        private List<string> m_CommonlMovies;

        public Recommender(Dictionary<string, Movie> moviesDictionary)
        {
            m_CommonlMovies = new List<string>();
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

        public List<string> suggestList(User user)
        {
            List<string> ans = new List<string>();
            Dictionary<string, double> UserSimilarity = new Dictionary<string, double>();
            double w = 0;
            foreach (User neighbor in m_usersDict.Values)
            {
                m_CommonlMovies.Clear();
                w = PearsonCorrealation(user, neighbor);
                UserSimilarity.Add(neighbor.UserId.ToString(), w);
            }

            //new

            List<User> TopTenUser = TopTenUsers(UserSimilarity);

            ans = stageTwo(TopTenUser, UserSimilarity, user);

            //end of new

            // ans = topTenSim(UserSimilarity);
            return ans;
        }

        private List<string> stageTwo(List<User> topTenUser, Dictionary<string, double> UserSimilarity, User user)
        {
            Dictionary<string, double> MovieAndRating = new Dictionary<string, double>();
            foreach (string movieId in m_moviesDictionary.Keys)
            {
                double mona = 0;
                double mecna = 0;
                for (int i = 0; i < 10; i++)
                {
                    double ru = calculateR(topTenUser[i]);
                    if (topTenUser[i].UserMovies.ContainsKey(movieId))
                    {
                        mona += (topTenUser[i].UserMovies[movieId] - ru) *
                                UserSimilarity[topTenUser[i].UserId.ToString()];
                        mecna += UserSimilarity[topTenUser[i].UserId.ToString()];
                    }
                    else
                    {
                        mona += 0;
                    }
                }

                double Ra = calculateR(user);
                double Pa = Ra + (mona / mecna);
                MovieAndRating.Add(movieId, Pa);
            }

            var sortedDict = from entry in MovieAndRating orderby entry.Value descending select entry;
            List<string> topMovie = new List<string>();

            List<string> ans = new List<string>();
            int z = 0;
            for (int i = 0; i < 10; i++)
            {
                ans.Add(sortedDict.ElementAt(i).Key);
            }

            return ans;
        }

        private List<User> TopTenUsers(Dictionary<string, double> userSimilarity)
        {
            List<User> ans = new List<User>();
            var sortedDict = from entry in userSimilarity orderby entry.Value descending select entry;
            for (int i = 0; i < 10; i++)
            {
                ans.Add(m_usersDict[sortedDict.ElementAt(i).Key]);
            }
            return ans;
        }

        private List<string> topTenSim(Dictionary<string, double> userSimilarity)
        {
            var sortedDict = from entry in userSimilarity orderby entry.Value descending select entry;
            User first = m_usersDict[sortedDict.First().Key];
            List<string> ans = new List<string>();
            ans = first.getTopTenMovies();
            return ans;
        }

        private double PearsonCorrealation(User user, User neighbor)
        {
            double ans = 0;
            double m = calculateM(user, neighbor);
            double Ra = calculateR(user);
            double Ru = calculateR(neighbor);
            double numerator = calculateNumerator(user, neighbor, Ra, Ru, m);
            double denominator = calculateDenominator(user, neighbor, Ra, Ru, m);
            ans = numerator / denominator;
            return ans;
        }

        private double calculateDenominator(User user, User neighbor, double ra, double ru, double m)
        {
            double ans = 0;
            double left = 0;
            double right = 0;
            foreach (string movieId in m_CommonlMovies)
            {
                left += Math.Pow((user.UserMovies[movieId] - ra), 2);
            }
            foreach (string movieId in m_CommonlMovies)
            {
                right += Math.Pow((neighbor.UserMovies[movieId] - ru), 2);
            }
            ans = Math.Sqrt((left * right));
            return ans;
        }

        private double calculateNumerator(User user, User neighbor, double ra, double ru, double m)
        {
            double ans = 0;
            double userNu;
            double neighborNu;
            foreach (string movieId in m_CommonlMovies)
            {
                userNu = user.UserMovies[movieId] - ra;
                neighborNu = neighbor.UserMovies[movieId] - ru;
                ans += userNu * neighborNu;
            }
            return ans;
        }

        private double calculateR(User user)
        {
            double ans = 0;
            double sum = user.UserMovies.Values.Sum();
            double count = user.UserMovies.Keys.Count();
            ans = sum / count;
            return ans;
        }

        private double calculateM(User user, User neighbor)
        {
            double ans = 0;
            foreach (string moviId in user.UserMovies.Keys)
            {
                if (neighbor.UserMovies.ContainsKey(moviId))
                {
                    ans++;
                    m_CommonlMovies.Add(moviId);
                }
            }
            return ans;
        }
    }
}