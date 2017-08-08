using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Entities;

namespace UserService.Interfaces
{
    public interface IUserService
    {
        IEnumerable<User> Search(Func<User, bool> searchPredicate);
    }
}
