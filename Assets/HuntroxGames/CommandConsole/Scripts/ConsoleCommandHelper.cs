using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;

namespace HuntroxGames.Utils
{
    internal static class ConsoleCommandHelper
    {
        internal const string VECTOR_PATTERN =@"[\-]?\d*\.?\d+?";
        internal const string VECTOR_INT_PATTERN =@"([\-]?\d*\.?\d+?:int)$";
        internal const string DATE_PREFIX =@"\[<color=yellow>\d+\:\d+\:\d+</*color>\] ";
        
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
                // if(methodParameters[i])
                if (i >= valueParams.Length)
                    parameters[i] = null;
                else
                    parameters[i] = ParseArgumentValue((string) valueParams[i], methodParameters[i].ParameterType);
                
            }
            return parameters;
        }
       
        internal static object ParseArgumentValue(string value, Type type)
        {
            if (value.IsNullOrEmpty() || value.IsNullOrWhiteSpace())
                return null;
            
            if (type.IsEnum)
                return ToEnum(value, type);
            
            else
            {
                
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
            }

            if (type == typeof(Vector3) || type == typeof(Vector3Int)) 
                return ToVector(value);
            if(type == typeof(Vector2))
                return (Vector2)ToVector(value);
            if(type == typeof(Vector2Int))
                return (Vector2Int)ToVector(value);
            
            return null;
        }

        private static object ToEnum(string value, Type type)
        {
            try
            {
                return  Enum.Parse(type, value, true);
            }
            catch { }
            return null;
        }

        internal static object ToVector(string value)
        {
            var matches = Regex.Matches(value, ConsoleCommandHelper.VECTOR_PATTERN).Cast<Match>();
            if (Regex.IsMatch(value, VECTOR_INT_PATTERN))
                return ParseVectorInt(matches);
            else
                return ParseVector(matches);
        }
        private static object ParseVector(IEnumerable<Match> matches)
        {
            var coordinates = matches.Where( m =>float.TryParse(m.Value ,out var f))
                .Select(m=> float.Parse(m.Value)).ToArray();
            switch (coordinates.Length)
            {
                case 3:
                    return new Vector3(coordinates[0],coordinates[1],coordinates[2]);
                case 2:
                    return new Vector3(coordinates[0],coordinates[1],0);
            }
            return null;
        }
        private static object ParseVectorInt(IEnumerable<Match> matches)
        {
            var coordinates = matches.Where( m =>int.TryParse(m.Value ,out var i))
                .Select(m=> int.Parse(m.Value)).ToArray();
            switch (coordinates.Length)
            {
                case 3:
                    return new Vector3Int(coordinates[0],coordinates[1],coordinates[2]);
                case 2:
                    return new Vector3Int(coordinates[0],coordinates[1],0);
            }
            return null;
        }
        
        
        
    }
}