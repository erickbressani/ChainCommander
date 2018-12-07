using System;

namespace Commander
{
    public class Executes : Attribute
    {
        public string CommandType { get; }

        public Executes(object commandType)
            => CommandType = commandType.ToString();
    }
}
