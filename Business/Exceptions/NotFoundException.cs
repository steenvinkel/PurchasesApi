using System;

namespace Business.Exceptions
{
    public class NotFoundException(string message) : Exception(message) {}
}
