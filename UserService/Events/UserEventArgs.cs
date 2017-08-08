using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Entities;

namespace UserService.Events
{
    [Serializable]
    public class UserEventArgs
    {
        public List<User> Users { get; }

        public UserEventArgs(User user)
        {
            Users = new List<User> { user };
        }

        public UserEventArgs(IEnumerable<User> users)
        {
            Users = new List<User>(users);
        }
    }
}
