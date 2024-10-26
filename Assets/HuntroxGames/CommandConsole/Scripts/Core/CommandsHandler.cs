using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HuntroxGames.Utils
{
    [PublicAPI]
    public static class CommandsHandler
    {
        private static readonly CommandInfo<FieldInfo> FieldCommands = new CommandInfo<FieldInfo>();
        private static readonly CommandInfo<MethodInfo> MethodCommands = new CommandInfo<MethodInfo>();
        private static readonly CommandInfo<PropertyInfo> PropertyCommands = new CommandInfo<PropertyInfo>();

        /// <summary>
        /// Setting this to true will force the console to rebuild and re-fetch all the commands in next command execution.
        /// Manual Command fetching done by calling <see cref="FetchCommandAttributes"/> method will set this to false.
        /// </summary>
        public static bool rebuildRequired;
        
        private static BindingFlags bindingFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        private static Func<CommandOptionsCallback,string> optionsCallbackFormatter;

        private static CommandOptionsCallback optionsCallback;
        
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void OnInitialize() 
            => GetStaticClass();

        
        internal static List<ConsoleCommandAttribute> GetConsoleCommandDescription()
        {
            var list = new List<ConsoleCommandAttribute>(FieldCommands.CommandsDescription);
            list.AddRange(MethodCommands.CommandsDescription);
            list.AddRange(PropertyCommands.CommandsDescription);
            return list;
        }

        
        public static void SetOptionsFormatter(Func<CommandOptionsCallback,string> formatter)
            => optionsCallbackFormatter = formatter;
        /// <summary>
        /// Fetches all the commands from the static classes and the MonoBehaviours in the scene.
        /// This can be called manually if you want to update the commands in the console.
        /// This is Expensive use it with caution.
        /// </summary>
        public static void FetchCommandAttributes()
        {
            var behaviours = Object.FindObjectsOfType<MonoBehaviour>()
                .OrderBy(m => m.transform.GetSiblingIndex()).ToArray();
            Clear();
   
            foreach (var behaviour in behaviours)
            {
                GetFields(behaviour);
                GetProperties(behaviour);
                GetMethods(behaviour);
            }

            rebuildRequired = false;
        }
        
        /// <summary>
        /// Executes the command with the given arguments if the command is found.
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="commandArguments">The arguments to pass to the command</param>
        /// <param name="executeLogCallback">Command Execution Log Callback</param>
        public static void ExecuteCommand(string command, object[] commandArguments,
            Action<string, bool> executeLogCallback)
        {
            if (optionsCallback != null)
            {
                if (!optionsCallback.TryExecuteOption(command))
                    executeLogCallback?.Invoke(optionsCallback.onInvalidOption,false);
                optionsCallback = null;
                return;
            }
            ExecuteFieldsCommands(command, commandArguments,executeLogCallback);
            ExecutePropertiesCommands(command, commandArguments, executeLogCallback);
            InvokeMethodsCommands(command, commandArguments, executeLogCallback);
            
            //check if rebuild is required to fetch the commands from the scene.
            //this is used when a MonoBehaviour with commands is removed from the scene.
            if(rebuildRequired)
                FetchCommandAttributes();
        }

        private static void ExecuteFieldsCommands(string fieldName, object[] arguments,
            Action<string, bool> executeLogCallback)
        {
            foreach (var field in FieldCommands.ExecutableStaticCommands(fieldName))
                Execute(field);

            var options = new List<CommandOption>();
            foreach (var field in FieldCommands.ExecutableCommands(fieldName))
            {
                if (!field.isOptions)
                    Execute(field);
                else
                    options.Add(new CommandOption(field.key.name, () => Execute(field)));
            }
            if (!options.IsNullOrEmpty())
                SetupOptionsCallback(new CommandOptionsCallback(options.ToArray()), executeLogCallback,arguments);

            void Execute(ExecutableCommand<FieldInfo> field)
            {
                var fieldValue = field.value.memberInfo;
                if (arguments.IsNullOrEmpty())
                {
                    var memberInfo =
                        ConsoleCommandHelper.GetMemberInfo(field, CommandConsole.Instance.ObjectNameDisplay);
                    var log =
                        $"{memberInfo} : <b>{fieldValue.GetValue(field.key)}</b>";
                    executeLogCallback?.Invoke(log, true);
                    return;
                }

                if (!fieldValue.IsLiteral)
                {
                    var value = ConsoleCommandHelper.ParseArgumentValue((string) arguments[0], fieldValue.FieldType);
                    fieldValue.SetValue(field.key, value);
                }
                else
                {
                    var log = $"Cannot set a constant field {fieldValue.Name}";
                    executeLogCallback?.Invoke(log, true);
                }
            }
        }

        private static void ExecutePropertiesCommands(string propertyName, object[] arguments,
            Action<string, bool> executeLogCallback)
        {
            void Execute(ExecutableCommand<PropertyInfo> property)
            {
                //if no arguments where provided get the value 
                var propertyValue = property.value.memberInfo;
                var memberInfo =
                    ConsoleCommandHelper.GetMemberInfo(property, CommandConsole.Instance.ObjectNameDisplay);
                if (arguments.IsNullOrEmpty())
                {
 
                    if (propertyValue.CanRead)
                    {
                        var log =
                            $"{memberInfo} : <b>{propertyValue.GetValue(property.key)}</b>";
                        executeLogCallback?.Invoke(log, true);
                    }
                    else
                        Debug.LogWarning(
                            $"property {propertyValue.DeclaringType?.Name}.{propertyValue.Name} does not have a getter!");
                }
                else//otherwise set it
                {
                    if (propertyValue.CanWrite)
                    {
                        var value = ConsoleCommandHelper.ParseArgumentValue((string) arguments[0],
                            propertyValue.PropertyType);
                        propertyValue.SetValue(property.key, value);
                    }
                    else
                        Debug.LogWarning(
                            $"property {propertyValue.DeclaringType?.Name}.{propertyValue.Name} does not have a setter!");
                }
            }
            
            foreach (var property in PropertyCommands.ExecutableStaticCommands(propertyName))
                Execute(property);
            var options = new List<CommandOption>();
            foreach (var property in PropertyCommands.ExecutableCommands(propertyName))
            {
                if (!property.isOptions)
                    Execute(property);
                else
                    options.Add(new CommandOption(property.key.name, () => Execute(property)));
            }
            if (!options.IsNullOrEmpty())
                SetupOptionsCallback(new CommandOptionsCallback(options.ToArray()), executeLogCallback,arguments);
            
        }

        private static void InvokeMethodsCommands(string methodName, object[] arguments,
            Action<string, bool> executeLogCallback)
        {
            void Execute(ExecutableCommand<MethodInfo> method)
            {
                var methodValue = method.value.memberInfo;
                var param = ConsoleCommandHelper.ToMethodParams(methodValue, arguments);
                var returnValue = method.value.memberInfo.Invoke(method.key, param);

                if (methodValue.ReturnType == typeof(void)) return;
                
                if (returnValue is CommandOptionsCallback callback)
                {
                    SetupOptionsCallback(callback, executeLogCallback, arguments);
                    return;
                }

                var memberInfo =
                    ConsoleCommandHelper.GetMemberInfo(method, CommandConsole.Instance.ObjectNameDisplay);
                var log =
                    $"{memberInfo} : <b>{returnValue}</b>";
                executeLogCallback?.Invoke(log, true);
            }

            //Execute all static members
            foreach (var method in MethodCommands.ExecutableStaticCommands(methodName))
                Execute(method);

            var options = new List<CommandOption>();
            foreach (var method in MethodCommands.ExecutableCommands(methodName))
            {
                if (!method.isOptions)
                    Execute(method);
                else
                    options.Add(new CommandOption(method.key.name, () => Execute(method)));
            }

            if (!options.IsNullOrEmpty())
                SetupOptionsCallback(new CommandOptionsCallback(options.ToArray()), executeLogCallback, arguments);
        }

        private static void SetupOptionsCallback(CommandOptionsCallback optnsCallback,
            Action<string, bool> executeLogCallback , object[] arguments)
        {
     
            if (optnsCallback.firstArgIsIndex && arguments != null && arguments.Length > 0)
            {
                if (!optnsCallback.TryExecuteOption((string)arguments[0]))
                    executeLogCallback?.Invoke(optnsCallback.onInvalidOption, false);
                
                return;
            }
            
            optionsCallback = optnsCallback;
            
            if (optionsCallbackFormatter != null)
            {
                var log = optionsCallbackFormatter(optnsCallback);
                executeLogCallback?.Invoke(log, false);
                return;
            }
            
            
            var optionsLog = "Options: ";
            var index = 0;
            foreach (var option in optionsCallback.options)
            {
                optionsLog += $"{option.Value.optionName}[{index}] ";
                index++;
            }
            executeLogCallback?.Invoke(optionsLog, false);
        }

        private static void GetFields(Object behaviour)
        {
            var fields = behaviour.GetType().GetFields(bindingFlags)
                .Where(f => f.IsDefined(typeof(ConsoleCommandAttribute), false)).ToArray();
            foreach (var field in fields)
            {
                var attrs = GetAttribute(field);
                foreach (var attr in attrs)
                    FieldCommands.Add(behaviour, attr, field, field.IsStatic);
            }
        }

        private static void GetProperties(Object behaviour)
        {
            var properties = behaviour.GetType().GetProperties(bindingFlags)
                .Where(p => p.IsDefined(typeof(ConsoleCommandAttribute), false)).ToArray();
            foreach (var property in properties)
            {
                var attrs = GetAttribute(property);
                foreach (var attr in attrs)
                {
                    var isStatic = property.CanRead && property.GetMethod.IsStatic ||
                                   property.CanWrite && property.SetMethod.IsStatic;
                    PropertyCommands.Add(behaviour, attr, property, isStatic);
                }
            }
        }

        private static void GetMethods(Object behaviour)
        {
            var methods = behaviour.GetType().GetMethods(bindingFlags)
                .Where(m => m.IsDefined(typeof(ConsoleCommandAttribute), false)).ToArray();
            foreach (var method in methods)
            {
                var attrs = GetAttribute(method);
                foreach (var attr in attrs)
                    MethodCommands.Add(behaviour, attr, method, method.IsStatic);
            }
        }

        private static void GetStaticClass()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    var flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
                    var members = type.GetMembers(flags)
                        .Where(m => m.IsDefined(typeof(ConsoleCommandAttribute), false));
                    foreach (var member in members)
                    {
                        if (member.CustomAttributes.ToArray().Length > 0)
                        {
                            var attrs = GetAttribute(member);
                            foreach (var attr in attrs)
                            {
                                switch (member)
                                {
                                    case FieldInfo field:
                                        FieldCommands.Add(null, attr, field, true);
                                        break;
                                    case PropertyInfo property:
                                        PropertyCommands.Add(null, attr, property, true);
                                        break;
                                    case MethodInfo method:
                                        MethodCommands.Add(null, attr, method, true);
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private static ConsoleCommandAttribute[] GetAttribute(MemberInfo member)
        {
            var attrs = member.GetCustomAttributes(true);
            var attributes = new List<ConsoleCommandAttribute>();
            foreach (var attr in attrs)
            {
                if (attr is ConsoleCommandAttribute devCom)
                {
                    devCom.command = (devCom.command.IsNullOrEmpty() ? member.Name : devCom.command)
                        .Replace(" ", string.Empty);
                    if (member is MethodInfo method)
                        devCom.parametersNames = ConsoleCommandHelper.GetMethodParametersName(method.GetParameters());
                    attributes.Add(devCom);
                }
            }

            return attributes.ToArray();
        }
        
        
        private static void Clear()
        {
            FieldCommands.Clear();
            PropertyCommands.Clear();
            MethodCommands.Clear();
        }
    }
}