#define COMMANDS_CONSOLE
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace HuntroxGames.Utils
{
    public class CommandConsole : Singleton<CommandConsole>
    {
        #if COMMANDS_CONSOLE
        
        private Vector2 scroll;
        private List<string> commands = new List<string>();
        private string commandInput = "";
        private GUISkin consoleStyle;
        private int lastCommandIndex;
        private Rect logBoxRect = new Rect(5, -250, Screen.width - 10, 200);
        private Rect viewRect;
        private float animationDuration = 0;
        private bool isActive;
        
        private bool setScroll = false;
        private Rect textBoxRect;
        private bool markFocus;
        protected override void Awake()
        {
            base.Awake();
            consoleStyle = Resources.Load<GUISkin>("ConsoleStyle");
            SceneManager.sceneLoaded += OnSceneLoaded;
            CommandsHandler.FetchCommandAttributes();
        }

        private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
        private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;
        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            CommandsHandler.FetchCommandAttributes();
            Debug.Log("Scene loaded: "+arg0.name);
        }

        private void Start() 
            => InsertLog("<color=green>type <color=white><b>'Help'</b></color> for a list of commands</color>");

        private void Update()
        {
            if (isActive)
                animationDuration += 0.2f * Time.deltaTime;
            else
                animationDuration -= 0.2f * Time.deltaTime;
            animationDuration = Mathf.Clamp(animationDuration, 0, 1);
            logBoxRect.y = Mathf.Lerp(logBoxRect.y, isActive ? 5 : -(logBoxRect.height + 50), animationDuration);
  
        }



        public void Console()
        {
            isActive = !isActive;
            if (isActive)
                markFocus = true;
        }


        private void InsertLog(string text,bool clearAllBefore = false)
        {
            if (clearAllBefore)
                ClearConsole();
            var date = DateTime.Now;
            var log = $"[<color=yellow>{date.Hour:00}:{date.Minute:00}:{date.Second:00}</color>] {text}";
            commands.Add(log);
        }
        
        
        [ConsoleCommand]
        private void ClearConsole()
        {
            commands.Clear();
            setScroll = true;
        }

        [ConsoleCommand("Help","",false)]
        private void HelpCommand()
        {
            var com = CommandsHandler.GETConsoleCommandDescription();
            foreach (var command in com)
            {
                InsertLog(command.command+" "+command.commandDescription);
            }
            lastCommandIndex = commands.Count;
        }
        
        public void OnGUI()
        {
            if (consoleStyle == null)
                consoleStyle = Resources.Load<GUISkin>("ConsoleStyle");

            GUI.skin = consoleStyle;
            var style = new GUIStyle();
            logBoxRect.width = Screen.width - 10;
            logBoxRect.height = logBoxRect.height;
            
            
            GUI.Box(logBoxRect, "Console");
            viewRect = new Rect(logBoxRect);
            var scrollRect = new Rect(logBoxRect);
            viewRect.height = 20 * commands.Count;
            viewRect.width -= 20;
            viewRect.y -= 5;
            //viewRect.x += 5;
            scrollRect.y += 25;
            scrollRect.height -= 35;
            scroll = GUI.BeginScrollView(scrollRect, scroll, viewRect);

            for (int i = 0; i < commands.Count; i++)
            {
                Rect labelRect = new Rect(scrollRect.x + 5, logBoxRect.y+20 * i, viewRect.width - 30, 20);
                GUI.Label(labelRect, commands[i]);
            }
            if (setScroll)
            {
                GUI.ScrollTo(new Rect(scrollRect.x,viewRect.height,viewRect.width,viewRect.height));
                setScroll = false;
            }
            GUI.EndScrollView();

   
            
            GUI.Box(new Rect(logBoxRect.x, logBoxRect.y + logBoxRect.height+2, logBoxRect.width * 0.35f, 22), "");
            GUI.backgroundColor = new Color(0, 0, 0, 0);


            
            var color = GUI.color;
            textBoxRect = new Rect(logBoxRect.x + 5, logBoxRect.y + logBoxRect.height+2, (logBoxRect.width * 0.35f) - 5, 20f);
            GUI.color = consoleStyle.customStyles[3].normal.textColor; 
            if(!commandInput.IsNullOrEmpty() && !AutoCompleteSuggestions().IsNullOrEmpty())
                GUI.Label(textBoxRect,AutoComplete());

            GUI.color = consoleStyle.textField.normal.textColor;
            GUI.SetNextControlName("commandInputField");
            commandInput = GUI.TextField(textBoxRect, commandInput);
            GUI.color = color;
            
            if (markFocus)
            {
                GUI.FocusControl("commandInputField");
                markFocus = false;
            }
                
            
            var current = Event.current;

            if (current.type == EventType.Layout||!isActive)
                return;

            if (current.isKey)
            {
                switch (current.keyCode)
                {
                    case KeyCode.Return:
                        if (!commandInput.IsNullOrEmpty())
                            HandleCommandInput();
                        break;
                    case KeyCode.UpArrow:
                        if (!commands.IsNullOrEmpty())
                        {
                            commandInput = Regex.Replace(commands[Mathf.Min(0,lastCommandIndex - 1)],
                                ConsoleCommandHelper.DATE_PREFIX, string.Empty).TrimEnd(' ');
                            lastCommandIndex = (lastCommandIndex - 0) < 0 ? commands.Count:lastCommandIndex - 1;
                            TextFieldLineEnd();
                        }
                        break;
                    case KeyCode.DownArrow:
                        if (!commands.IsNullOrEmpty())
                        {
                            commandInput = Regex.Replace(commands[lastCommandIndex],
                                ConsoleCommandHelper.DATE_PREFIX, string.Empty).TrimEnd(' ');
                            lastCommandIndex = (lastCommandIndex +1) % commands.Count;
                            TextFieldLineEnd();
                        }
                        break;
                    case KeyCode.Tab:
                        if (!commandInput.IsNullOrEmpty() && !AutoComplete().IsNullOrEmpty())
                        {
                            commandInput += AutoComplete().Trim(' ');
                            TextFieldLineEnd();
                        }
                        break;
                }
            }
        }

        private void TextFieldLineEnd()
        {
            GUI.FocusControl("commandInputField");
            TextEditor textEditor =
                (TextEditor) GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
            if (textEditor != null)
                textEditor.MoveLineEnd();
        }


        private string AutoComplete()
        {
            string autoComplete = AutoCompleteSuggestions()[0];
            var removedChar = "";
            for (int i = 0; i < commandInput.Length; i++)
            {
                char atChar = autoComplete[i];
                char chr = commandInput[i];
                if (char.ToUpperInvariant(atChar) == char.ToUpperInvariant(chr))
                    removedChar += atChar;
                
            }
            autoComplete = autoComplete.Replace(removedChar,"");
            foreach (var chr in commandInput)
                autoComplete = autoComplete.Insert(0, " ");
            
            return autoComplete;
        }
        private List<string> AutoCompleteSuggestions()
        {
            var list = new List<string>();
            foreach (var command in CommandsHandler.GETConsoleCommandDescription())
            {
                if (command.command.StartsWith(commandInput,StringComparison.CurrentCultureIgnoreCase))
                    list.Add(command.command);
            }
            return list;
        }

        private void HandleCommandInput()
        {
            InsertLog(commandInput);
            lastCommandIndex = commands.Count;
            var command = ConsoleCommandHelper.SplitCommand(commandInput);
            CommandsHandler.ExecuteCommand(command.cmd,command.param,InsertLog);
            commandInput = "";
            setScroll = true;
        }


#endif
    }
}
