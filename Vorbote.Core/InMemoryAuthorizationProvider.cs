using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vorbote
{
    public class InMemoryAuthorizationProvider : IAuthProvider
    {
        private Dictionary<string, string> _usersDictionary;

        public InMemoryAuthorizationProvider()
        {
            _usersDictionary = new Dictionary<string, string>();
        }

        public bool AuthorizeUser(string username, string password)
        {
            if(_usersDictionary.ContainsKey(username) && _usersDictionary[username] == password)
            {
                return true;
            }
            {
                return false;
            }
        }

        private void AddUser(string username, string password)
        {
            _usersDictionary.Add(username, password);
        }
    }
}
