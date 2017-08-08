using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UserService.Entities;
using UserService.Services.Interfaces;
using UserService.TCP;

namespace UserService.Services
{
    [Serializable]
    internal class UserServiceOfSlave : MarshalByRefObject, IUserServiceOfSlave
    {
        #region Fields & properties

        private ClientTcp tcpClient;
        private static readonly ReaderWriterLock Rwl = new ReaderWriterLock();
        private static readonly int time = 10000;

        #endregion

        public void ConnectToTcpClient(ClientTcp client)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<User> Search(Func<User, bool> searchPredicate)
        {
            throw new NotImplementedException();
        }
    }
}
