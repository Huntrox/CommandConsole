using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using UnityEngine;

namespace HuntroxGames.Utils
{
    [PublicAPI]
    internal static class ConsoleCommandHelper
    {
        
        //internal const string COMMA_SEPARATOR =@"[\-]?\d*\.?\d+?";
        internal const string COMMA_SEPARATOR =@"[\-]?\d*\.?\d+?";
        internal const string VECTOR_INT_PATTERN =@"([\-]?\d*\.?\d+?:int)$";
        internal const string DATE_PREFIX =@"\[<color=yellow>\d+\:\d+\:\d+</*color>\] ";
        internal const string VECTOR_PATTERN =@"([\-]?\d*\.?\d+?:float)$";
        private static readonly Dictionary<Type, Func<string,object>> TypesDictionary= new Dictionary<Type, Func<string,object>>
        {
            {typeof(Vector3),ToVector},
            {typeof(Vector3Int),ToVector},
            {typeof(Vector2Int),x=>(Vector2Int)ToVector(x)},
            {typeof(Vector2),x=>(Vector2)ToVector(x)},
            {typeof(Color),ToColor},
        };
        /// <summary>
        /// Split command and parameters from input string and return them as tuple (command,parameters) 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        internal static (string cmd ,string[] param) SplitCommand(string input)
        {
            var s = input.Split(' ');
            var command = s[0];
            var list = s.ToList();
            list.Remove(command);
            var param = list.ToArray();
            return  (command ,param);
        }
        internal static object[] ToMethodParams(MethodInfo method, object[] valueParams)
        {
            var methodParameters = method.GetParameters();
            if (valueParams.IsNullOrEmpty() || methodParameters.IsNullOrEmpty())
                return null;
            
            var parameters = new object[methodParameters.Length];
            for (var i = 0; i < parameters.Length; i++)
            {
                var isArray = methodParameters[i].GetCustomAttribute(typeof(ParamArrayAttribute)) != null;
                if (isArray)
                {
                    var startIndex = i;
                    var end = valueParams.Length;
                    var length = end - startIndex;
                    var valueLeft = valueParams.Skip(startIndex).Take(length).ToArray();
                    return  ValueToParamsArray(methodParameters[i], valueLeft);
                }

                if (i >= valueParams.Length)
                    parameters[i] = null;
                else
                    parameters[i] = ParseArgumentValue((string) valueParams[i], methodParameters[i].ParameterType);

            }
            return parameters;
        }

        private static object[] ValueToParamsArray(ParameterInfo parameterInfo, object[] valueParams)
        {
            var parameters = new object[valueParams.Length];
            var arrayElementType = parameterInfo.ParameterType.GetElementType();
            if (arrayElementType == typeof(int))
            {
                object[] intArray = {ValueToIntArray(valueParams)};
                return intArray;
            }
            for (var i = 0; i < parameters.Length; i++)
                    parameters[i] = ParseArgumentValue((string) valueParams[i], arrayElementType);

            return parameters;
        }

        private static int[] ValueToIntArray(object[] valueParams)
        {
            int[] intParams = new int[valueParams.Length];
            for (var i = 0; i < intParams.Length; i++)
                intParams[i] = (int)ParseArgumentValue((string) valueParams[i], typeof(int));
            return intParams;
        }
        internal static object ParseArgumentValue(string value, Type type)
        {
            //Debug.Log(type.Name);
            if (value.IsNullOrEmpty() || value.IsNullOrWhiteSpace())
                return null;
            
            if (type.IsEnum)
                return ToEnum(value, type);
            
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return value.ToUpper() == "TRUE";
                case TypeCode.String:
                    return value;
                case TypeCode.Int32:
                    if (int.TryParse(value, out var intValue))
                        return intValue;
                    return null;
                case TypeCode.Single:
                    if (float.TryParse(value, out var floatValue))
                        return floatValue;
                    return null;
            }
            
            return TypesDictionary.TryGetValue(type, out var funcValue) 
                ? funcValue.Invoke(value) 
                : null;
        }
        internal static string[] GetMethodParametersName(ParameterInfo[] getParameters)
        {
            if (getParameters.IsNullOrEmpty())
                return null;
            var names = new string[getParameters.Length];
            for (var i = 0; i < getParameters.Length; i++)
                names[i] = getParameters[i].Name;
            return names;
        }
        private static object ToEnum(string value, Type type)
        {
            try
            {
                return  Enum.Parse(type, value, true);
            }
            catch
            {
                // ignored
            }

            return null;
        }

        private static object ToColor(string value)
        {
            var matches = Regex.Matches(value, ConsoleCommandHelper.COMMA_SEPARATOR).Cast<Match>();
            var values = matches.Where( m =>float.TryParse(m.Value ,out var f))
                .Select(m=> float.Parse(m.Value)).ToArray();
            switch (values.Length)
            {
                case 4:
                    return new Color(values[0],values[1],values[2],values[3]);
                case 3:
                    return new Color(values[0],values[1],values[2],1);
                case 2:
                    return new Color(values[0],values[1],1,1);
                case 1:
                    return new Color(values[0],1,1,1);
            }

            var color = Color.white;
            ColorUtility.TryParseHtmlString(value.ToLower(), out color);
            return color;
        } 
        internal static object ToVector(string value)
        {
            var matches = Regex.Matches(value, COMMA_SEPARATOR).Cast<Match>();
            return Regex.IsMatch(value, VECTOR_INT_PATTERN) ? ParseVectorInt(matches) : ParseVector(matches);
        }
        private static object ParseVector(IEnumerable<Match> matches)
        {
            var coordinates = matches.Where( m =>float.TryParse(m.Value ,out var f))
                .Select(m=> float.Parse(m.Value)).ToArray();
            return coordinates.Length switch
            {
                3 => new Vector3(coordinates[0], coordinates[1], coordinates[2]),
                2 => new Vector3(coordinates[0], coordinates[1], 0),
                _ => null
            };
        }
        private static object ParseVectorInt(IEnumerable<Match> matches)
        {
            var coordinates = matches.Where( m =>int.TryParse(m.Value ,out var i))
                .Select(m=> int.Parse(m.Value)).ToArray();
            return coordinates.Length switch
            {
                3 => new Vector3Int(coordinates[0], coordinates[1], coordinates[2]),
                2 => new Vector3Int(coordinates[0], coordinates[1], 0),
                _ => null
            };
        }
        
        public static string GetMemberInfo<T>(ExecutableCommand<T> member,CommandConsole.ObjectNameDisplayType displayType) where T: MemberInfo
        {
            var format = "";
            var hasObject = displayType.HasFlag(CommandConsole.ObjectNameDisplayType.GameObject);
            var hasClass = displayType.HasFlag(CommandConsole.ObjectNameDisplayType.Class);
            var hasMember = displayType.HasFlag(CommandConsole.ObjectNameDisplayType.Member);

            if (hasObject && member.key !=null)
            {
                format += $"{member.key.name}";
                format += hasClass || hasMember? ".":"";
            }
            if (hasClass)
            {
                format += $"{member.value.memberInfo.DeclaringType?.Name}";
                format += hasMember? ".":""; 
            }
            if (hasMember)
            {
                format += $"{member.value.memberInfo.Name}";
            }
            return format;
        }


        #region GUI Font Helpers
        
        public static int GetLogsHeight(List<string> logs,float rectWidth,Font font)
        {
            var height = 0;
            foreach (var log in logs)
            {
                var logWidth = GetLogWidth(log, font);
                height+= logWidth <= rectWidth ? 20 : 40;
            }
            return height;
        }
        public static int GetLogWidth(string log,Font font)
        {
            var textWidth = 0;
            log = Regex.Replace(log, DATE_PREFIX, string.Empty).TrimEnd(' ');
            foreach (var @char in log)
                if (font.GetCharacterInfo(@char,out var info,font.fontSize))
                    textWidth += info.advance;
            return textWidth;
        }
        
        #endregion

    }
}