#define COMMANDS_CONSOLE
using System;
using UnityEngine;

namespace HuntroxGames.Utils
{
    public class ConsoleGUIMenu : CommandConsole
    {

        [Header("GUI Style")] [SerializeField] 
        private Font font;

        
        [Header("Input")]
        [SerializeField] private KeyCode autoCompletionKey = KeyCode.Tab;
        [SerializeField] private KeyCode submitKey = KeyCode.Return;
        
        
        

        private Vector2 scroll;

        private string commandInput = "";
        private GUISkin consoleStyle;

        private Rect logBoxRect = new Rect(5, -250, Screen.width - 10, 200);
        private Rect viewRect;
        private float animationDuration = 0;
       

        private bool updateScrollView = false;
        private Rect textBoxRect;
        private bool markFocus;



        
        protected override void Awake()
        {
            consoleStyle = Resources.Load<GUISkin>("ConsoleStyle");
            base.Awake();
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


        protected override void OnUnityLogMessageReceived(string condition, string stacktrace, LogType type)
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
                var parameters = " ";

                if (!command.parametersNames.IsNullOrEmpty())
                    foreach (var parameter in command.parametersNames)
                        parameters += parameter + " ";

                var parametersColor = ColorUtility.ToHtmlStringRGBA(parameterColor);
                var description = command.description.IsNullOrEmpty() ? "" : $": {command.description}";
                var text = $"{command.command}<color=#{parametersColor}>{parameters}</color>{description}";
                InsertLog(text);
            }
        }

        public override void ToggleConsole()
        {
            base.ToggleConsole();
            if (isActive)
            {
                markFocus = true;
            }
            else
                GUI.UnfocusWindow();
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

            if (!current.isKey) return;
            switch (current.keyCode)
            {
                case KeyCode.UpArrow:
                    HandleConsoleNavigation(ConsoleNavigation.Up);
                    break;
                case KeyCode.DownArrow:
                    HandleConsoleNavigation(ConsoleNavigation.Down);
                    break;
                default:
                {
                        
                    if (current.keyCode == submitKey)
                    {
                        if (!commandInput.IsNullOrEmpty())
                            HandleCommandInput(commandInput, InsertLog);
                    }
                    else if (current.keyCode == autoCompletionKey)
                    {
                        if (!commandInput.IsNullOrEmpty() && !commandSuggestion.AutoCompleteSuggestion.IsNullOrEmpty())
                        {
                            commandInput += commandSuggestion.AutoCompleteSuggestion.Trim(' ');
                            TextFieldLineEnd();
                        }
                    }
                    break;
                }
            }
        }

        private static void TextFieldLineEnd()
        {
            GUI.FocusControl("commandInputField");
            var textEditor =
                (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
            textEditor?.MoveLineEnd();
        }


        private void HandleConsoleNavigation(ConsoleNavigation navigation)
        {
            switch (navigation)
            {
                case ConsoleNavigation.Up:
                    //if (commandInput.IsNullOrEmpty() || commandInput != commandSuggestion.AutoCompleteSuggestion)
                        commandInput = consoleHistory.Previous(commandInput);
                    // else
                    //     commandSuggestion.Previous();
                    TextFieldLineEnd();
                    break;
                case ConsoleNavigation.Down:
                    //if (commandInput.IsNullOrEmpty() || commandInput != commandSuggestion.AutoCompleteSuggestion)
                        commandInput = consoleHistory.Next(commandInput);
                    // else
                    //     commandSuggestion.Next();
                    TextFieldLineEnd();
                    break;
                case ConsoleNavigation.Left:
                    break;
                case ConsoleNavigation.Right:
                    break;
            }
        }

        protected override void CommandExecuteInvoke(string cmd)
        {
            base.CommandExecuteInvoke(cmd);
            commandInput = "";
            updateScrollView = true;
        }

        protected override void CommandExecuteWithParametersInvoke(string cmd, string[] @params)
        {
            base.CommandExecuteWithParametersInvoke(cmd, @params);
        }
    }
}