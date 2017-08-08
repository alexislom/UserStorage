using System;

namespace UserService.Entities
{
    /// <summary>
    /// Entity of user
    /// </summary>
    [Serializable]
    public class User : IEquatable<User>
    {
        #region Fields & properties

        public int Id { get; set; }
        //public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }

        #endregion

        #region Public methods

        public bool Equals(User other) => other != null && Id == other.Id 
                                            && FirstName == other.FirstName
                                            && LastName == other.LastName;

        #endregion

        #region Override methods of object

        public override string ToString() => $"User: {FirstName} {LastName}";

        public override bool Equals(object obj)
        {
            return Equals(obj as User);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)2166136261;
                hash = (hash * 16777619) ^ Id.GetHashCode();
                hash = (hash * 16777619) ^ CheckOnNull(FirstName);
                hash = (hash * 16777619) ^ CheckOnNull(LastName);
                hash = (hash * 16777619) ^ Age.GetHashCode();
                return hash;
            }
        }

        #endregion

        #region Private methods
        private int CheckOnNull<T>(T obj)
        {
            if (obj == null)
                return 0;
            return obj.GetHashCode();
        }
        #endregion
    }
}
