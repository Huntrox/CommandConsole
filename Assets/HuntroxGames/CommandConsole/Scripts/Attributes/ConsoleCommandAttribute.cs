using System;

namespace HuntroxGames.Utils
{
    [AttributeUsage(AttributeTargets.Field|AttributeTargets.Method|AttributeTargets.Property , AllowMultiple = true, Inherited = true)]
    public class ConsoleCommandAttribute : Attribute
    {
        public string command = "";
        public string commandDescription = "";
        public bool helpMenu = true;
        public ConsoleCommandAttribute()
        {
            commandDescription = "";
            command = "";
            helpMenu = true;
        }      
        public ConsoleCommandAttribute(bool helpMenu)
        {
            commandDescription = "";
            command = "";
            this.helpMenu = helpMenu;
        }
        public ConsoleCommandAttribute(string command, string commandDescription = "",  bool helpMenu = true)
        {
            this.command = command;
            this.commandDescription = commandDescription;
            this.helpMenu = helpMenu;
        }
    }

}