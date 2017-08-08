using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using UserService.AOP;
using UserService.Entities;
using UserService.Events;
using UserService.Exceptions;
using UserService.GeneratorId;
using UserService.Services.Interfaces;

namespace UserService.Services
{
    /// <summary>
    /// Class that provides service for master
    /// </summary>
    public class UserServiceOfMaster : MarshalByRefObject, IUserServiceOfMaster, IDisposable
    {
        #region Fields & properties

        private static readonly ReaderWriterLock Rwl = new ReaderWriterLock();
        private static readonly object lockObj = new object();
        private static readonly int time = 10000;
        private readonly List<User> _userList;
        private readonly IGeneratorId _generatorId;

        public event EventHandler<UserEventArgs> AddUser = delegate { };
        public event EventHandler<UserEventArgs> DeleteUser = delegate { };
        public event EventHandler<UserEventArgs> AddUserOnSlaveCreating = delegate { };

        #endregion

        #region Constructors

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="generatorId">Class that counts user id</param>
        [LogMethod]
        public UserServiceOfMaster(IGeneratorId generatorId)
        {
            _generatorId = generatorId ?? throw new ArgumentNullException(nameof(generatorId));
            _userList = new List<User>();
            Deserialize();
        }

        #endregion

        #region Public API for master 

        //[LogMethod]
        //public void ConnectToTcpServer(ServerTcp server)
        //{
        //    if (server == null)
        //        throw new ArgumentNullException();
        //    server.ConnectToMasterService(this);
        //}

        /// <summary>
        /// Adding new User 
        /// </summary>
        /// <param name="user">user to add</param>
        [LogMethod]
        public void Add(User user)
        {
            if (user == null)
                throw new ArgumentNullException();
            if (user.FirstName == null || user.LastName == null)
                throw new EmptyUserException();

            _generatorId.GenerateId(user);//////////////////???
            if (_userList.Exists(u => u.Id == user.Id))
                throw new UserExistsException();

            Rwl.AcquireWriterLock(time);
            _userList.Add(user);
            Rwl.ReleaseWriterLock();

            OnAddUser(new UserEventArgs(user));
        }

        /// <summary>
        /// Adding collection of users
        /// </summary>
        /// <param name="users"></param>
        [LogMethod]
        public void Add(IEnumerable<User> users)
        {
            if (users == null)
                throw new ArgumentNullException();

            var tempUsers = new List<User>();

            Rwl.AcquireWriterLock(time);
            foreach (var user in users)
            {
                if (user?.FirstName == null || user.LastName == null)
                    continue;
                _generatorId.GenerateId(user);///////////////////?????????????????
                if (_userList.Exists(u => u.Id == user.Id))
                    continue;
                _userList.Add(user);
                tempUsers.Add(user);
            }
            Rwl.ReleaseWriterLock();

            OnAddUser(new UserEventArgs(tempUsers));
        }

        /// <summary>
        /// Deleting an element or elements frome the sequnce
        /// </summary>
        /// <param name="deletePredicate">predicate</param>
        [LogMethod]
        public void Delete(Func<User, bool> deletePredicate)
        {
            if (deletePredicate == null)
                throw new ArgumentNullException();
            var tempUsers = Search(deletePredicate).ToList();

            Rwl.AcquireWriterLock(time);
            foreach (var user in Search(deletePredicate))
                _userList.Remove(user);
            Rwl.ReleaseWriterLock();

            OnDeleteUser(new UserEventArgs(tempUsers));
        }

        public IEnumerable<User> Search(Func<User, bool> searchPredicate)
        {
            if (searchPredicate == null)
                throw new ArgumentNullException();

            Rwl.AcquireReaderLock(time);
            var listUsers = _userList.Where(searchPredicate).ToList();
            Rwl.ReleaseReaderLock();
            return listUsers;
        }

        #endregion

        #region Working with XML

        private void Serialize()
        {
            var fileName = ConfigurationManager.AppSettings["XmlFileStorage"];
            var writer = new XmlTextWriter(fileName, Encoding.ASCII);
            lock (lockObj)
            {
                WriteXml(writer);
                writer.Close();
            }
        }

        private void Deserialize()
        {
            var fileName = ConfigurationManager.AppSettings["XmlFileStorage"];
            var reader = new XmlTextReader(fileName);
            if (File.Exists(fileName))
            {
                lock (lockObj)
                {
                    ReadXml(reader);
                }
            }
        }

        private void ReadXml(XmlReader reader)
        {
            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "UserStorage")
            {
                var tempUsers = new List<User>();
                if (reader.ReadToDescendant("User"))
                {
                    while (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "User")
                    {
                        User user = new User
                        {
                            FirstName = reader["FirstName"],
                            LastName = reader["LastName"],
                            Age = int.Parse(reader["Age"])
                        };
                        reader.Read();
                        tempUsers.Add(user);
                    }
                    this.Add(tempUsers);
                }
                reader.Read();
            }
        }

        private void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("UserStorage");
            foreach (var user in _userList)
            {
                writer.WriteStartElement("User");
                writer.WriteAttributeString("FirstName", user.FirstName);
                writer.WriteAttributeString("LastName", user.LastName);
                writer.WriteAttributeString("Age", user.Age.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        #endregion

        #region Event notification

        [LogMethod]
        protected virtual void OnAddUser(UserEventArgs e)
        {
            EventHandler<UserEventArgs> temp = AddUser;
            temp?.Invoke(this, e);
        }

        [LogMethod]
        protected virtual void OnAddUserOnCreatingSlave()
        {
            EventHandler<UserEventArgs> temp = AddUserOnSlaveCreating;
            temp?.Invoke(this, new UserEventArgs(_userList));
        }

        [LogMethod]
        protected virtual void OnDeleteUser(UserEventArgs e)
        {
            EventHandler<UserEventArgs> temp = DeleteUser;
            temp?.Invoke(this, e);
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    Serialize();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UserServiceOfMaster() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
