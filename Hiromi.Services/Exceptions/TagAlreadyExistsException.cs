using System;

namespace Hiromi.Services.Exceptions
{
    public class TagAlreadyExistsException : Exception
    {
        public TagAlreadyExistsException(string message) : base(message)
        {
        }
    }
}