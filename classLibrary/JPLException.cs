using System;

namespace JPL.classLibrary
{
    public class JPLException : Exception
    {
        public JPLException()
        {
            
        }

        public JPLException(string message) : base(message)
        {
            
        }

        public JPLException(string message, Exception inner) : base(message, inner)
        {
            
        }
    }
}
