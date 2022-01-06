using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HuntroxGames.Utils
{
    public static class CommandsHandler
    {
        private static readonly CommandInfo<FieldInfo> FieldCommands = new CommandInfo<FieldInfo>();
        private static readonly CommandInfo<MethodInfo> MethodCommands = new CommandInfo<MethodInfo>();
        private static readonly CommandInfo<PropertyInfo> PropertyCommands = new CommandInfo<PropertyInfo>();

        public static bool rebuildRequired = false;
        private static BindingFlags bindingFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;

        internal static List<ConsoleCommandAttribute> GETConsoleCommandDescription()
        {
            var list = new List<ConsoleCommandAttribute>(FieldCommands.CommandsDescription);
            list.AddRange(MethodCommands.CommandsDescription);
            list.AddRange(PropertyCommands.CommandsDescription);
            return list;
        }

        public static void FetchCommandAttributes()
        {
            var behaviours = Object.FindObjectsOfType<MonoBehaviour>();
            Clear();
            GetStaticClass();
            foreach (var behaviour in behaviours)
            {
                GetFields(behaviour);
                GetProperties(behaviour);
                GetMethods(behaviour);
            }

            rebuildRequired = false;
        }
        
        private static void Clear()
        {
            FieldCommands.Clear();
            PropertyCommands.Clear();
            MethodCommands.Clear();
        }

        public static void ExecuteCommand(string command, object[] commandArguments,
            Action<string, bool> onGetValueCallback)
        {
            ExecuteFieldsCommands(command, commandArguments,onGetValueCallback);
            ExecutePropertiesCommands(command, commandArguments, onGetValueCallback);
            InvokeMethodsCommands(command, commandArguments, onGetValueCallback);
            if(rebuildRequired)
                FetchCommandAttributes();
        }
        //Cannot set a constant field
        private static void ExecuteFieldsCommands(string fieldName, object[] arguments,
            Action<string, bool> onGetValueCallback)
        {
            foreach (var field in FieldCommands.ExecutableCommands(fieldName))
            {
                if (arguments.IsNullOrEmpty())
                {
                    var log = $"{field.value.DeclaringType?.Name}.{field.value.Name} : <b>{field.value.GetValue(field.key)}</b>";
                    onGetValueCallback?.Invoke(log, false);
                    return;
                }

                if (!field.value.IsLiteral)
                {
                    var value = ConsoleCommandHelper.ParseArgumentValue((string) arguments[0], field.value.FieldType);
                    field.value.SetValue(field.key, value);
                }
                else
                {
                    var log = $"Cannot set a constant field {field.value.Name}";
                    onGetValueCallback?.Invoke(log, false);
                }
            }
        }

        private static void ExecutePropertiesCommands(string propertyName, object[] arguments,
            Action<string, bool> onGetValueCallback)
        {
            foreach (var property in PropertyCommands.ExecutableCommands(propertyName))
            {
                //if no arguments where provided get the value 
                if (arguments.IsNullOrEmpty())
                {
                    if (property.value.CanRead)
                    {
                        var log =
                            $"{property.value.DeclaringType?.Name}.{property.value.Name} : <b>{property.value.GetValue(property.key)}</b>";
                        onGetValueCallback?.Invoke(log, false);
                    }
                    else
                        Debug.LogWarning(
                            $"property {property.value.DeclaringType?.Name}.{property.value.Name} does not have a getter!");
                }
                else//otherwise set it
                {
                    if (property.value.CanWrite)
                    {
                        var value = ConsoleCommandHelper.ParseArgumentValue((string) arguments[0],
                            property.value.PropertyType);
                        property.value.SetValue(property.key, value);
                    }
                    else
                        Debug.LogWarning(
                            $"property {property.value.DeclaringType?.Name}.{property.value.Name} does not have a setter!");
                }
            }
        }

        private static void InvokeMethodsCommands(string methodName, object[] arguments,
            Action<string, bool> onGetValueCallback)
        {
            foreach (var method in MethodCommands.ExecutableCommands(methodName))
            {
                object[] param = ConsoleCommandHelper.ToMethodParams(method.value, arguments);
                var returnValue = method.value.Invoke(method.key, param);
                if (method.value.ReturnType != typeof(void))
                {
                    var log =
                        $"{method.value.DeclaringType?.Name}.{method.value.Name} : <b>{returnValue}</b>";
                    onGetValueCallback?.Invoke(log, false);
                }
            }
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
                foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
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
            object[] attrs = member.GetCustomAttributes(true);
            List<ConsoleCommandAttribute> attributes = new List<ConsoleCommandAttribute>();
            foreach (var attr in attrs)
            {
                if (attr is ConsoleCommandAttribute devCom)
                {
                    devCom.command = (devCom.command.IsNullOrEmpty() ? member.Name : devCom.command)
                        .Replace(" ", string.Empty);
                    if (member is MethodInfo method)
                        devCom.parametersNames = GetMethodParametersName(method.GetParameters());
                    attributes.Add(devCom);
                }
            }

            return attributes.ToArray();
        }

        private static string[] GetMethodParametersName(ParameterInfo[] getParameters)
        {
            if (getParameters.IsNullOrEmpty())
                return null;
            var names = new string[getParameters.Length];
            for (int i = 0; i < getParameters.Length; i++)
                names[i] = getParameters[i].Name;
            return names;
        }
    }
}