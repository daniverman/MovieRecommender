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
    }
}