using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace HuntroxGames.Utils
{
    /// <summary>
    /// CommandOptionsCallback holds a List of options and their callbacks used to execute selected option from the console
    /// </summary>
    [PublicAPI]
    public class CommandOptionsCallback
    {
        
        public readonly Dictionary<string,CommandOption> options;
        public readonly string onInvalidOption = "Invalid Option, Please try again";
        public bool firstArgIsIndex = true;
        
        public CommandOptionsCallback(params CommandOption[] options):this(true,options)
        {
        }
       
        public CommandOptionsCallback(bool firstArgIsIndex = false,params CommandOption[] options)
        {
            this.firstArgIsIndex = firstArgIsIndex;
            this.options = options.ToDictionary(x => x.optionName.ToUpper());
        }
        public CommandOptionsCallback(IEnumerable<(string command, Action callback)> options)
            => this.options = options.ToDictionary(x => x.command.ToUpper(),x => new CommandOption(x.command,x.callback));

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

        public void AddOption(string optionName, Action callback) 
            => options.Add(optionName.ToUpper(),new CommandOption(optionName.ToUpper(),callback));
    }

    public class CommandOption
    {
        public readonly string optionName;
        public readonly Action optionCallback;

        public CommandOption(string optionName, Action optionCallback)
        {
            this.optionName = optionName;
            this.optionCallback = optionCallback;
        }
        
    }
}