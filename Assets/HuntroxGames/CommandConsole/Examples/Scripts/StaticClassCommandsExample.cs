using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HuntroxGames.Utils
{
    public static class StaticClassCommandsExample
    {
        
        [ConsoleCommand]
        public static void StaticMethod() => Debug.Log("Public Static Method");
        [ConsoleCommand]
        private static void PrivateStaticMethod() => Debug.Log("Private Static Method");
        
        //Property setter and getter
        private static bool staticBoolField = false;
        
        [ConsoleCommand("SetStaticBool","[true/false]")]
        [ConsoleCommand("GetStaticBool")]
        public static bool StaticBoolField
        {
            get => staticBoolField;
            set
            {
                Debug.Log("StaticBoolField Property Value: " + value);
                staticBoolField = value;
            }
        }
        
        [ConsoleCommand] public static int staticIntField = 0;
        [ConsoleCommand(nameof(GetStaticIntField))]
        public static void GetStaticIntField() => Debug.Log(staticIntField);
    }
}