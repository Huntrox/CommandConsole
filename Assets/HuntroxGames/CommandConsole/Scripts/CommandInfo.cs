using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace HuntroxGames.Utils
{
    public class CommandInfo<T> where T : MemberInfo
    {
        private readonly Dictionary<string, T> staticMemberInfos = new Dictionary<string, T>();

        private readonly Dictionary<Object, CommandData<T>> commandInfo =
            new Dictionary<Object, CommandData<T>>();

        private readonly Dictionary<string, ConsoleCommandAttribute> attributes =
            new Dictionary<string, ConsoleCommandAttribute>();

        public List<ConsoleCommandAttribute> CommandsDescription =>
            attributes.Where(a => a.Value.helpMenu)
                .Select(a => a.Value).ToList();

        public List<(Object key, T value)> ExecutableCommands(string command)
        {
            command = command.ToLower();
            var commands = new List<(Object, T)>();
            if (staticMemberInfos.TryGetValue(command, out var value))
                commands.Add((null, value));
            foreach (var inf in commandInfo)
            {
                if (inf.Key == null)
                {
                    CommandsHandler.rebuildRequired = true;
                    continue;
                }

                if (inf.Value.info.TryGetValue(command, out var v))
                    commands.Add((inf.Key, v));
            }

            return commands;
        }

        public void Add(Object objectKey, ConsoleCommandAttribute attribute, T memberInfoValue, bool isStatic = false)
        {
            var commandKey = attribute.command.ToLower();
            if (isStatic)
            {
                if (!staticMemberInfos.ContainsKey(commandKey))
                    staticMemberInfos.Add(commandKey, memberInfoValue);
            }
            else
            {
                if (commandInfo.ContainsKey(objectKey))
                    commandInfo[objectKey].Add(commandKey, memberInfoValue);
                else
                    commandInfo.Add(objectKey,
                        new CommandData<T>
                        {
                            info = new Dictionary<string, T>
                            {
                                {commandKey, memberInfoValue}
                            }
                        });
            }

            if (!attributes.ContainsKey(commandKey))
                attributes.Add(commandKey, attribute);
        }

        public void Clear()
        {
            //staticMemberInfos.Clear();
            commandInfo.Clear();
        }
    }
    public class CommandData<T> where T : MemberInfo
    {
        public Dictionary<string, T> info = new Dictionary<string, T>();
        public void Add(string commandKey, T memberValue)
        {
            if (info.ContainsKey(commandKey)) return;
            info.Add(commandKey, memberValue);
        }
    }
}