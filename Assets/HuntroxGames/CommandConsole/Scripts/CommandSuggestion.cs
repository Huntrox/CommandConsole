using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HuntroxGames.Utils
{
    public class CommandSuggestion
    {
        private string commandInput = "";
        private int index = 0;

        public string AutoCompleteSuggestion => AutoComplete();

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

        public string AutoComplete()
        {
            if (AutoCompleteSuggestions().IsNullOrEmpty())
                return "";
            string autoComplete = AutoCompleteSuggestions()[index];
            var removedChar = "";
            for (int i = 0; i < commandInput.Length; i++)
            {
                char atChar = autoComplete[i];
                char chr = commandInput[i];
                if (char.ToUpperInvariant(atChar) == char.ToUpperInvariant(chr))
                    removedChar += atChar;
            }

            if (!removedChar.IsNullOrEmpty())
                autoComplete = autoComplete.Replace(removedChar, "");
            foreach (var chr in commandInput)
                autoComplete = autoComplete.Insert(0, " ");

            return autoComplete;
        }

        private List<string> AutoCompleteSuggestions()
        {
            var list = new List<string>();
            foreach (var command in CommandsHandler.GETConsoleCommandDescription())
            {
                if (command.command.StartsWith(commandInput, StringComparison.CurrentCultureIgnoreCase))
                    list.Add(command.command);
            }

            return list;
        }
    }
}