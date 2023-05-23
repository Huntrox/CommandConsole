using System;
using System.Collections.Generic;

namespace HuntroxGames.Utils
{
    public class CommandSuggestion
    {
        private string commandInput = "";
        private int index = 0;

        /// <summary>
        /// Returns the current completed auto complete suggestion
        /// </summary>
        public string AutoCompleteSuggestion => AutoComplete();
        
        /// <summary>
        /// Returns the current auto complete suggestions matching the input
        /// </summary>
        public List<string> Suggestions => AutoCompleteSuggestions();
        
        public int CurrentIndex => index;
        
        
        

        public void SetInput(string input)
        {
            if (input == commandInput)
                return;
            commandInput = input;
            index = 0;
        }

        public void Next()
        {
            var count = AutoCompleteSuggestions().Count;
            if (count != 0)
                index = (index + 1) % count;
        }

        public void Previous()
            => index = index - 1 < 0 ? AutoCompleteSuggestions().Count - 1 : index - 1;

        public string AutoComplete(bool removeInput = true)
        {
            if (AutoCompleteSuggestions().IsNullOrEmpty())
                return "";
            var autoComplete = AutoCompleteSuggestions()[index];
            var removedChar = "";
            var replaceChar = "";
            for (var i = 0; i < commandInput.Length; i++)
            {
                var atChar = autoComplete[i];
                var chr = commandInput[i];

                if (char.ToUpperInvariant(atChar) == char.ToUpperInvariant(chr))
                {
                    removedChar += atChar;
                    replaceChar += chr;
                }
            }

            if (!removedChar.IsNullOrEmpty())
                autoComplete = autoComplete.Replace(removedChar, removeInput? "" : commandInput);
            if (removeInput)
                foreach (var chr in commandInput)
                    autoComplete = autoComplete.Insert(0, " ");

            return autoComplete;
        }

        private List<string> AutoCompleteSuggestions()
        {
            var list = new List<string>();
            foreach (var command in CommandsHandler.GetConsoleCommandDescription())
            {
                if (command.command.StartsWith(commandInput, StringComparison.CurrentCultureIgnoreCase))
                    list.Add(command.command);
            }

            return list;
        }
    }
}