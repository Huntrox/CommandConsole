using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HuntroxGames.Utils
{
    [PublicAPI]
    public static class CCToolbar
    {
        
        [MenuItem("HuntroxUtils/CommandConsole/Add Command Console")]
        public static void AddConsoleManager()
        {
            
            if (CheckConsoleExists()) return;
            
            //check if canvas exists in scene if not create one and load CommandConsoleUI prefab from Resources and add it to canvas
            var canvas = Object.FindObjectOfType<Canvas>(); 
            if (canvas == null)
            {
                canvas = new GameObject("Canvas", typeof(Canvas)).GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.gameObject.AddComponent<CanvasScaler>();
                canvas.gameObject.AddComponent<GraphicRaycaster>();
                Selection.SetActiveObjectWithContext(canvas, canvas);
                //check if EventSystem exists in scene if not create one
                if (Object.FindObjectOfType<EventSystem>() == null)
                {
                    var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                }
            }
            
            var console = Resources.Load<ConsoleUIMenu>("UI/CommandConsoleUI");
            var consoleInstance = Object.Instantiate(console, canvas.transform);
            consoleInstance.name = "CommandConsoleUI";
            Selection.SetActiveObjectWithContext(consoleInstance, consoleInstance);
        }
        
        [MenuItem("HuntroxUtils/CommandConsole/Add Command Console (GUI)")]
        public static void AddConsoleManagerGUI()
        {
            if (CheckConsoleExists()) return;
            
            var console = new GameObject("Console", typeof(ConsoleGUIMenu));
            console.gameObject.AddComponent<ConsoleMenuTrigger>();
            Selection.SetActiveObjectWithContext(console, console);
        }

        [MenuItem("HuntroxUtils/CommandConsole/Add Console Menu Trigger")]
        public static void AddConsoleManagerTrigger()
        {
            var console = Object.FindObjectOfType<CommandConsole>();
            if (console == null)
            {
                AddConsoleManager();
                console = Object.FindObjectOfType<CommandConsole>();
            }
            console.gameObject.AddComponent<ConsoleMenuTrigger>();
        }
        
        //Validate if console exists
        [MenuItem("HuntroxUtils/CommandConsole/Add Console Menu Trigger", true)]
        public static bool ConsoleExists() 
            => Object.FindObjectOfType<CommandConsole>() != null;
        
        
        private static bool CheckConsoleExists()
        {
            var console = Object.FindObjectOfType<CommandConsole>();
            if (console == null)
            {
                return false;
            }
            
            Debug.Log($"{console.GetType()} already exists in scene");
            Selection.SetActiveObjectWithContext(console, console);
            return true;
        }
    }
    
    
}