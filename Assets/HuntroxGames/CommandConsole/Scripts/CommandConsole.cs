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
        private enum FetchMode
        {
            OnSceneLoad = 1 << 0,
            OnConsoleTrigger = 1 << 1,
            BeforeExecutingAnyCommand = 1 << 2,
        }

        private enum InputPrefixStyle
        {
            [InspectorName("[HH:MM:SS] Text")] Date,
            [InspectorName("- Text")] Dash,
            [InspectorName("Text")] None,
            Custom
        }

        [Flags]
        public enum ObjectNameDisplayType
        {
            [InspectorName("GameObject Name")]
            GameObject = 1 << 0,
            [InspectorName("Class Name")]
            Class = 1 << 1,
            [InspectorName("Class Member Name")]
            Member = 1 << 2,
        }

        [SerializeField, Tooltip("Receive Unity Log Messages")]
        private bool unityLogMessages;

        [SerializeField] private FetchMode fetchMode = FetchMode.OnSceneLoad;

        [Header("Style")] [SerializeField] private Font font;
        [SerializeField] private Color commandInputFieldColor = new Color32(65, 183, 25, 255);
        [SerializeField] private Color textColor = Color.white;
        [SerializeField] private Color parameterColor = new Color(1, 1, 1, 0.25f);
        [SerializeField] private Color autoCompleteColor = new Color(1, 1, 1, 0.25f);
        [SerializeField] private ObjectNameDisplayType objectNameDisplay = ObjectNameDisplayType.GameObject | ObjectNameDisplayType.Member;

        [Header("Input Style")] [SerializeField]
        private InputPrefixStyle inputPrefixStyle = InputPrefixStyle.Date;

        [SerializeField] private Color inputPrefixColor = Color.yellow;
        [SerializeField] private string customInputPrefix = "";

        public ObjectNameDisplayType ObjectNameDisplay => objectNameDisplay;

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


        /// <summary>
        /// this event will rise everytime when the console opens or close
        /// Usage Example: add listener and use it to pause/unpause the game
        /// </summary>
        public static event Action<bool> OnConsole;
        public static event Action<string> OnCommandExecuted;
        public static event Action<string,string[]> OnCommandExecutedWithParameters;
        
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
            }
            else
                GUI.UnfocusWindow();

            OnConsole?.Invoke(isActive);
        }


        private void InsertLog(string text, bool dateFormat)
            => InsertLog(text, dateFormat, false);

        private void InsertLog(string text, bool dateFormat = true, bool clearAllBefore = false)
        {
            if (clearAllBefore)
                ClearConsole();
            var dateText = FormatInput(dateFormat);
            var log = $"{dateText}{text}";
            logList.Add(log);
        }

        private string FormatInput(bool dateFormat)
        {
            var color = ColorUtility.ToHtmlStringRGBA(inputPrefixColor);
            switch (inputPrefixStyle)
            {
                case InputPrefixStyle.Date:
                    var date = DateTime.Now;
                    return dateFormat
                        ? $"[<color=#{color}>{date.Hour:00}:{date.Minute:00}:{date.Second:00}</color>] "
                        : "           ";
                case InputPrefixStyle.Dash:
                    return dateFormat ? $"<color=#{color}>-</color> " : "  ";
                case InputPrefixStyle.None:
                    return "";
                case InputPrefixStyle.Custom:
                    var whiteSpace = " ";
                    for (int i = 0; i < customInputPrefix.Length; i++)
                        whiteSpace += " ";
                    return dateFormat ? $"<color=#{color}>{customInputPrefix}</color> " : whiteSpace;
            }

            return "";
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
            var com = CommandsHandler.GetConsoleCommandDescription();
            foreach (var command in com)
            {
                string parameters = " ";

                if (!command.parametersNames.IsNullOrEmpty())
                    foreach (var parameter in command.parametersNames)
                        parameters += parameter + " ";

                var parametersColor = ColorUtility.ToHtmlStringRGBA(parameterColor);
                var description = command.description.IsNullOrEmpty() ? "" : $": {command.description}";
                var text = $"{command.command}<color=#{parametersColor}>{parameters}</color>{description}";
                InsertLog(text);
            }
        }

        public void OnGUI()
        {
            if (consoleStyle == null)
                consoleStyle = Resources.Load<GUISkin>("ConsoleStyle");
            
            GUI.skin = consoleStyle;
            
            GUI.skin.label.normal.textColor = textColor;
            GUI.skin.textField.contentOffset = new Vector2(-3, 0);
            
            logBoxRect.width = Screen.width - 10;
            logBoxRect.height = logBoxRect.height;


            GUI.Box(logBoxRect, "Console");
            viewRect = new Rect(logBoxRect);
            var scrollRect = new Rect(logBoxRect);
            viewRect.width -= 20;
            var rectWidth = viewRect.width - 30;
            viewRect.height = ConsoleCommandHelper.GetLogsHeight(logList, rectWidth, consoleStyle.font);
            viewRect.y -= 5;
            //viewRect.x += 5;
            scrollRect.y += 25;
            scrollRect.height -= 35;
            scroll = GUI.BeginScrollView(scrollRect, scroll, viewRect);
            var labelY = logBoxRect.y;

            for (int i = 0; i < logList.Count; i++)
            {
                var textWidth = ConsoleCommandHelper.GetLogWidth(logList[i], consoleStyle.font);
                var rectHeight = (textWidth) <= rectWidth ? 20 : 40;

                Rect labelRect = new Rect(scrollRect.x + 5, labelY, rectWidth, rectHeight);
                GUI.Label(labelRect, logList[i]);
                labelY += rectHeight;
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
            GUI.color = autoCompleteColor;
            if (!commandInput.IsNullOrEmpty())
                GUI.Label(textBoxRect, commandSuggestion.AutoCompleteSuggestion);

            GUI.color = commandInputFieldColor;
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
                (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
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
            var (cmd, @params) = ConsoleCommandHelper.SplitCommand(commandInput);
            CommandsHandler.ExecuteCommand(cmd, @params, InsertLog);
            OnCommandExecuted?.Invoke(cmd);
            OnCommandExecutedWithParameters?.Invoke(cmd,  @params);
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