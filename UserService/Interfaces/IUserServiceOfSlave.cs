using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.TCP;

namespace UserService.Interfaces
{
    public interface IUserServiceOfSlave : IUserService
    {
        void ConnectToTcpClient(ClientTcp client);
    }
}
