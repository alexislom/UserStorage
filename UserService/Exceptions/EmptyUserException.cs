﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Exceptions
{
    [Serializable]
    public class EmptyUserException : Exception
    {
        public EmptyUserException()
        {
        }

        public EmptyUserException(string message)
            : base(message)
        {
        }

        public EmptyUserException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
