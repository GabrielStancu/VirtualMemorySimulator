using System;

namespace Engine
{
    public class InvalidAddressException : Exception
    {
        public InvalidAddressException(string message) : base(message)
        {
        }
    }
}
