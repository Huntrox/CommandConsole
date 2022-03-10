using UnityEditor;
using UnityEngine;

namespace HuntroxGames.Utils
{
    public static class CCToolbar
    {
        [MenuItem("HuntroxUtils/CommandConsole/Add Command Console")]
        public static void AddConsoleManager()
        {
            if (GameObject.FindObjectOfType<CommandConsole>() == null)
            {
                var console = new GameObject("Console", typeof(CommandConsole));
                Selection.SetActiveObjectWithContext(console, console);
            }
        }

        [MenuItem("HuntroxUtils/CommandConsole/Add Console Menu Trigger")]
        public static void AddConsoleManagerTrigger()
        {
            var console = GameObject.FindObjectOfType<CommandConsole>();
            if (console == null)
            {
                AddConsoleManager();
                console = GameObject.FindObjectOfType<CommandConsole>();
            }

            console.gameObject.AddComponent<ConsoleMenuTrigger>();
        }
    }
}