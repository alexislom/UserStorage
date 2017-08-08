using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Entities;
using UserService.Exceptions;

namespace UserService.GeneratorId
{
    [Serializable]
    public class GeneratorId : IGeneratorId
    {
        /// <summary>
        /// Method that gives algorithm of counting of user id
        /// </summary>
        /// <param name="user">User to set id</param>
        public void GenerateId(User user)
        {
            if (user == null)
                throw new ArgumentNullException();
            if (user.FirstName == null || user.LastName == null)
                throw new EmptyUserException();
            user.Id = Guid.NewGuid();
        }
    }
}
