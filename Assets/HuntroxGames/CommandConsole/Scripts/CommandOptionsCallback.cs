using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HuntroxGames.Utils
{
    public class CommandOptionsCallback
    {
        public Dictionary<string,CommandOption> options;
        public string onInvalidOption = "Invalid Option, Please try again";
        public CommandOptionsCallback(params CommandOption[] options)
        {
            this.options = options.ToDictionary(x => x.optionName.ToUpper());
        }
        
        public bool TryExecuteOption(string key)
        {
            key = key.ToUpper();
            if (options.IsNullOrEmpty())
                return false;
            if (int.TryParse(key, out var index))
            {
                if (index < options.Count)
                {
                    options.ToArray()[index].Value.optionCallback?.Invoke();
                    return true;
                }
            }
            if (options.TryGetValue(key, out var option))
            {
                option.optionCallback?.Invoke();
                return true;
            }

            return false;
        }
    }

    public class CommandOption
    {
        public string optionName;
        public Action optionCallback;

        public CommandOption(string optionName, Action optionCallback)
        {
            this.optionName = optionName;
            this.optionCallback = optionCallback;
        }
        
    }
}