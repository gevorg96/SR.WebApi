using System;

namespace SR.Domain
{
    public static class Guard
    {
        public static T Require<T>(T? value, long id, string message) where T : class
            => value ?? throw new ArgumentException(message + $", Id = {id}");
    }
}