using System;

namespace ChainCommander
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class HandlesAttribute : Attribute
    {
        public string CommandName { get; }

        public HandlesAttribute(object commandName)
            => CommandName = commandName.ToString();
    }
}