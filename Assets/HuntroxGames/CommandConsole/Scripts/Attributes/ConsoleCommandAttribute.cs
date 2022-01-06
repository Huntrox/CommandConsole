using System;

namespace HuntroxGames.Utils
{
    [AttributeUsage(AttributeTargets.Field|AttributeTargets.Method|AttributeTargets.Property , AllowMultiple = true, Inherited = true)]
    public class ConsoleCommandAttribute : Attribute
    {
        public string command = "";
        public string description = "";
        public bool helpMenu = true;
        public string[] parametersNames;
        public ConsoleCommandAttribute()
        {
            description = "";
            command = "";
            helpMenu = true;
        }      
        public ConsoleCommandAttribute(bool helpMenu)
        {
            description = "";
            command = "";
            this.helpMenu = helpMenu;
        }
        public ConsoleCommandAttribute(string command, string description = "",  bool helpMenu = true)
        {
            this.command = command;
            this.description = description;
            this.helpMenu = helpMenu;
        }
    }

}