using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRecommender.Model
{
    public class User
    {
        private Dictionary<string, double> m_userMovies;
        private int m_userId;

        public Dictionary<string, double> UserMovies
        {
            get { return m_userMovies; }
            set { m_userMovies = value; }
        }

        public int UserId
        {
            get { return m_userId; }
            set { m_userId = value; }
        }

        public User()
        {
            m_userMovies = new Dictionary<string, double>();
        }

        public List<string> getTopTenMovies()
        {
            List<string> ans = new List<string>();
            var sortedDict = from entry in m_userMovies orderby entry.Value descending select entry;
            for (int i = 0; i < 10; i++)
            {
                string MovieId = sortedDict.ElementAt(i).Key;
                ans.Add(MovieId);
            }
            return ans;
        }
    }
}