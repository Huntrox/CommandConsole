#define COMMANDS_CONSOLE
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace HuntroxGames.Utils
{
    public class CommandConsole : Singleton<CommandConsole>
    {
        [Flags]
        public enum FetchMode
        {
            OnSceneLoad = 1 << 0,
            OnConsoleTrigger = 1 << 1,
            BeforeExecutingAnyCommand = 1 << 2,
        }
        
        [SerializeField, Tooltip("Receive Unity Log Messages")]
        private bool unityLogMessages;

        [SerializeField] private FetchMode fetchMode = FetchMode.OnSceneLoad;


#if COMMANDS_CONSOLE

        private Vector2 scroll;
        private readonly List<string> logList = new List<string>();
        private string commandInput = "";
        private GUISkin consoleStyle;

        private Rect logBoxRect = new Rect(5, -250, Screen.width - 10, 200);
        private Rect viewRect;
        private float animationDuration = 0;
        private bool isActive;

        private bool updateScrollView = false;
        private Rect textBoxRect;
        private bool markFocus;
        private readonly ConsoleHistory consoleHistory = new ConsoleHistory();
        private readonly CommandSuggestion commandSuggestion = new CommandSuggestion();

        public delegate void OnConsole(bool value);

        /// <summary>
        /// this event will rise everytime when the console opens or close
        /// Usage Example: add listener and use it to pause/unpause the game
        /// </summary>
        public event OnConsole onConsole;

        protected override void Awake()
        {
            base.Awake();
            consoleStyle = Resources.Load<GUISkin>("ConsoleStyle");
            SceneManager.sceneLoaded += OnSceneLoaded;
            CommandsHandler.FetchCommandAttributes();
        }

        private void OnEnable()
        {
            if (unityLogMessages)
                Application.logMessageReceivedThreaded += OnUnityLogMessageReceived;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnUnityLogMessageReceived(string condition, string stacktrace, LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                    InsertLog($"<color=red>{condition}</color>");
                    break;
                case LogType.Assert:
                    InsertLog(condition);
                    break;
                case LogType.Warning:
                    InsertLog($"<color=yellow>{condition}</color>");
                    break;
                case LogType.Log:
                    InsertLog(condition);
                    break;
                case LogType.Exception:
                    InsertLog($"<color=red>{condition}</color>");
                    break;
            }
        }

        private void OnDisable()
        {
            Application.logMessageReceivedThreaded -= OnUnityLogMessageReceived;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (fetchMode.HasFlag(FetchMode.OnSceneLoad))
                CommandsHandler.FetchCommandAttributes();
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
            {
                markFocus = true;
                if (fetchMode.HasFlag(FetchMode.OnConsoleTrigger))
                    CommandsHandler.FetchCommandAttributes();
            }else
                GUI.UnfocusWindow();

            onConsole?.Invoke(isActive);
        }


        private void InsertLog(string text, bool dateFormat)
            => InsertLog(text, dateFormat, false);
        private void InsertLog(string text,bool dateFormat = true, bool clearAllBefore = false)
        {
            if (clearAllBefore)
                ClearConsole();
            var date = DateTime.Now;
            var dateText = dateFormat ? $"[<color=yellow>{date.Hour:00}:{date.Minute:00}:{date.Second:00}</color>]" : "          ";
            var log = $"{dateText} {text}";
            logList.Add(log);
        }

        [ConsoleCommand]
        private void ClearConsole()
        {
            logList.Clear();
            consoleHistory.Clear();
            updateScrollView = true;
        }

        [ConsoleCommand("Help", "", false, MonoObjectExecutionType.FirstInHierarchy)]
        private void HelpCommand()
        {
            var com = CommandsHandler.GETConsoleCommandDescription();
            foreach (var command in com)
            {
                string parameters = " ";

                if (!command.parametersNames.IsNullOrEmpty())
                    foreach (var parameter in command.parametersNames)
                        parameters += parameter + " ";

                var parametersColor = ColorUtility.ToHtmlStringRGBA(new Color(1, 1, 1, 0.25f));
                var text = $"{command.command}<color=#{parametersColor}>{parameters}</color>: {command.description}";
                InsertLog(text);
            }
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
            viewRect.height = 20 * logList.Count;
            viewRect.width -= 20;
            viewRect.y -= 5;
            //viewRect.x += 5;
            scrollRect.y += 25;
            scrollRect.height -= 35;
            scroll = GUI.BeginScrollView(scrollRect, scroll, viewRect);

            for (int i = 0; i < logList.Count; i++)
            {
                Rect labelRect = new Rect(scrollRect.x + 5, logBoxRect.y + 20 * i, viewRect.width - 30, 20);
                GUI.Label(labelRect, logList[i]);
            }

            if (updateScrollView)
            {
                GUI.ScrollTo(new Rect(scrollRect.x, viewRect.height, viewRect.width, viewRect.height));
                updateScrollView = false;
            }

            GUI.EndScrollView();


            GUI.Box(new Rect(logBoxRect.x, logBoxRect.y + logBoxRect.height + 2, logBoxRect.width * 0.35f, 22), "");
            GUI.backgroundColor = new Color(0, 0, 0, 0);


            var color = GUI.color;
            textBoxRect = new Rect(logBoxRect.x + 5, logBoxRect.y + logBoxRect.height + 2,
                (logBoxRect.width * 0.35f) - 5, 20f);
            GUI.color = consoleStyle.customStyles[3].normal.textColor;
            if (!commandInput.IsNullOrEmpty())
                GUI.Label(textBoxRect, commandSuggestion.AutoCompleteSuggestion);

            GUI.color = consoleStyle.textField.normal.textColor;
            GUI.SetNextControlName("commandInputField");
            commandInput = GUI.TextField(textBoxRect, commandInput);
            commandSuggestion.SetInput(commandInput);
            GUI.color = color;

            if (markFocus)
            {
                GUI.FocusControl("commandInputField");
                markFocus = false;
            }

            var current = Event.current;

            if (current.type == EventType.Layout || !isActive)
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
                        HandleConsoleNavigation(ConsoleNavigation.Up);
                        break;
                    case KeyCode.DownArrow:
                        HandleConsoleNavigation(ConsoleNavigation.Down);
                        break;
                    case KeyCode.Tab:
                        if (!commandInput.IsNullOrEmpty() && !commandSuggestion.AutoCompleteSuggestion.IsNullOrEmpty())
                        {
                            commandInput += commandSuggestion.AutoCompleteSuggestion.Trim(' ');
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
            textEditor?.MoveLineEnd();
        }


        private void HandleConsoleNavigation(ConsoleNavigation navigation)
        {
            switch (navigation)
            {
                case ConsoleNavigation.Up:
                    if (commandInput.IsNullOrEmpty())
                        commandInput = consoleHistory.Previous();
                    else
                        commandSuggestion.Previous();
                    TextFieldLineEnd();
                    break;
                case ConsoleNavigation.Down:
                    if (commandInput.IsNullOrEmpty())
                        commandInput = consoleHistory.Next();
                    else
                        commandSuggestion.Next();
                    TextFieldLineEnd();
                    break;
                case ConsoleNavigation.Left:
                    break;
                case ConsoleNavigation.Right:
                    break;
            }
        }

        private void HandleCommandInput()
        {
            InsertLog(commandInput);
            consoleHistory.Add(commandInput);
            if (fetchMode.HasFlag(FetchMode.BeforeExecutingAnyCommand))
                CommandsHandler.FetchCommandAttributes();
            var command = ConsoleCommandHelper.SplitCommand(commandInput);
            CommandsHandler.ExecuteCommand(command.cmd, command.param, InsertLog);
            commandInput = "";
            updateScrollView = true;
        }


#endif
    }

    public enum ConsoleNavigation
    {
        Up,
        Down,
        Left,
        Right
    }
}