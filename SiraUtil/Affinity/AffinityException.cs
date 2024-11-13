using System;

namespace SiraUtil.Affinity
{
    internal class AffinityException : Exception
    {
        public AffinityException(string message) : base(message) { }

        public AffinityException(string message, Exception innerException) : base(message, innerException) { }
    }
}
