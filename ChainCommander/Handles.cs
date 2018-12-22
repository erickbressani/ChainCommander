using System;

namespace ChainCommander
{
    public class Handles : Attribute
    {
        public string CommandName { get; }

        public Handles(object commandName)
            => CommandName = commandName.ToString();
    }
}
