using System;

namespace Hiromi.Services.Tags.Exceptions
{
    public class TagAlreadyExistsException : Exception
    {
        public TagAlreadyExistsException(string message = null) : base(message)
        {
        }
    }
}