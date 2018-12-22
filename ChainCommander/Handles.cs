using System;

namespace Commander
{
    public class Handles : Attribute
    {
        public string CommandName { get; }

        public Handles(object commandName)
            => CommandName = commandName.ToString();
    }
}
