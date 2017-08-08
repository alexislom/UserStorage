using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Entities;

namespace UserService.GeneratorId
{
    public interface IGeneratorId
    {
        /// <summary>
        /// Method that gives algorithm of counting of user id
        /// </summary>
        /// <param name="user">User to set id</param>
        void GenerateId(User user);
    }
}
