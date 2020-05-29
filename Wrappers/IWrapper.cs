using System;

namespace Handshake.Wrappers
{
    public abstract class IWrapper
    {
        public  abstract T GetResource<T>(string description, Tuple<string, string>[] parameters = null);
    }
}
