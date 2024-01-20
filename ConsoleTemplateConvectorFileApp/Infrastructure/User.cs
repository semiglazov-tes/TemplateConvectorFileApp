using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateWorkApp
{
    internal class User
    {
        private int _id;
        public int Id
        {
            get { return _id; }
        }

        public User(int userID)
        {
            _id= userID;
        }
    }
}
