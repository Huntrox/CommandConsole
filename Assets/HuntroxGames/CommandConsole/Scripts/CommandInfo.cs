using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace HuntroxGames.Utils
{
    public class CommandInfo<T> where T : MemberInfo
    {
        private readonly Dictionary<string, T> staticMemberInfos = new Dictionary<string, T>();

        private readonly Dictionary<Object, CommandDataContainer<T>> commandInfo =
            new Dictionary<Object, CommandDataContainer<T>>();

        private readonly Dictionary<string, ConsoleCommandAttribute> attributes =
            new Dictionary<string, ConsoleCommandAttribute>();

        /// <summary>
        /// 
        /// </summary>
        public List<ConsoleCommandAttribute> CommandsDescription =>
            attributes.Where(a => a.Value.helpMenu)
                .Select(a => a.Value).ToList();


        public List<ExecutableCommand<T>> ExecutableStaticCommands(string command)
        {
            command = command.ToLower();
            var commands = new List<ExecutableCommand<T>>();
            if (staticMemberInfos.TryGetValue(command, out var value))
                commands.Add(new ExecutableCommand<T>(null, new CommandData<T>(value,MonoObjectExecutionType.All)));
            return commands;
        }

    public List<ExecutableCommand<T>> ExecutableCommands(string command)
        {
            command = command.ToLower();
            var commands = new List<ExecutableCommand<T>>();
            
            foreach (var inf in commandInfo)
            {
                if (inf.Key == null)
                {
                    CommandsHandler.rebuildRequired = true;
                    continue;
                }
                
                if (inf.Value.info.TryGetValue(command, out var v))
                {
                    var executableCommand = new ExecutableCommand<T>(inf.Key, v);
                    if (v.objectExecutionType == MonoObjectExecutionType.FirstInHierarchy)
                    {
                        commands.Add(executableCommand);
                        break;
                    }
                    if (v.objectExecutionType == MonoObjectExecutionType.Option)
                        executableCommand.isOptions = true;
                    commands.Add(executableCommand);
                }
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
                    commandInfo[objectKey].Add(commandKey, new CommandData<T>(memberInfoValue, attribute.objectExecutionType));
                else
                    commandInfo.Add(objectKey,
                        new CommandDataContainer<T>
                        {
                            info = new Dictionary<string, CommandData<T>>
                            {
                                {commandKey, new CommandData<T>(memberInfoValue, attribute.objectExecutionType)}
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
    public class CommandDataContainer<T> where T : MemberInfo
    {
        public Dictionary<string, CommandData<T>> info = new Dictionary<string, CommandData<T>>();
        public void Add(string commandKey, CommandData<T> memberValue)
        {
            if (info.ContainsKey(commandKey)) return;
            info.Add(commandKey,memberValue);
        }
    }

    public struct CommandData<T> where T : MemberInfo
    {
        public T memberInfo;
        public MonoObjectExecutionType objectExecutionType;

        public CommandData(T memberInfo, MonoObjectExecutionType objectExecutionType)
        {
            this.memberInfo = memberInfo;
            this.objectExecutionType = objectExecutionType;
        }
    }

    public struct ExecutableCommand<T> where T : MemberInfo
    {
        public Object key;
        public CommandData<T> value;
        public bool isOptions;

        public ExecutableCommand(Object key, CommandData<T> value, bool isOptions = false)
        {
            this.key = key;
            this.value = value;
            this.isOptions = isOptions;
        }
    }
}