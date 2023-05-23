using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HuntroxGames.Utils
{
    public class ConsoleHistory
    {
        private readonly List<string> commandHistory = new List<string>();
        private int index = 0;
        
        public void Clear() 
        {
            commandHistory.Clear();
            index = 0;
        }
        public void Add(string command)
        {
            if (command.IsNullOrEmpty() || command.IsNullOrWhiteSpace()) return;
            commandHistory.Add(command);
            index = commandHistory.Count;
        }
        public string Next(string currentCommand)
        {
            //get the next command in the history and loop back to the start if we reach the end
            if (commandHistory.IsNullOrEmpty())
                return currentCommand;
            index = (index + 1) % commandHistory.Count;
            return commandHistory[index];
        }
        public string Previous(string currentCommand)
        {
            if (commandHistory.IsNullOrEmpty())
                return currentCommand;
            //get the previous command in the history and loop back to the end if we reach the start
            index = index - 1 < 0 ? commandHistory.Count - 1 : index - 1;
            return commandHistory[index];
        }
    }
    
}