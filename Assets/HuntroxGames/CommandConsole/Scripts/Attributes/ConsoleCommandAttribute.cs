using System;

namespace HuntroxGames.Utils
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true,
        Inherited = true)]
    public class ConsoleCommandAttribute : Attribute
    {
        public string command = "";
        public string description = "";
        public readonly bool helpMenu = true;
        public string[] parametersNames;
        public readonly MonoObjectExecutionType objectExecutionType = MonoObjectExecutionType.FirstInHierarchy;

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


        public ConsoleCommandAttribute(string command)
        {
            this.command = command;
        }
        public ConsoleCommandAttribute(string command,MonoObjectExecutionType objectExecutionType = MonoObjectExecutionType.FirstInHierarchy)
        {
            this.command = command;
            this.objectExecutionType = objectExecutionType;
        }
        public ConsoleCommandAttribute(string command,string description = "",MonoObjectExecutionType objectExecutionType = MonoObjectExecutionType.FirstInHierarchy)
        {
            this.command = command;
            this.description = description;
            this.objectExecutionType = objectExecutionType;
        }
        public ConsoleCommandAttribute(string command, string description, 
            bool helpMenu ,MonoObjectExecutionType objectExecutionType)
        {
            this.command = command;
            this.description = description;
            this.helpMenu = helpMenu;
            this.objectExecutionType = objectExecutionType;
        }
    }

    public enum MonoObjectExecutionType
    {
        FirstInHierarchy,
        All,
        Option
    }
}