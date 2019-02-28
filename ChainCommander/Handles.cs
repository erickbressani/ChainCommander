using System;

namespace ChainCommander
{
    public class HandlesAttribute : Attribute
    {
        public string CommandName { get; }

        public HandlesAttribute(object commandName)
            => CommandName = commandName.ToString();
    }
}
