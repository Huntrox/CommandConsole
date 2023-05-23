using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HuntroxGames.Utils
{
    
    public class ConsoleUIMenu : CommandConsole
    {
        [Header("UI Settings")]
        [SerializeField] private GameObject textPrefab;
        [SerializeField] private RectTransform content;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform uiParentPanel;
        [SerializeField] private TMP_InputField consoleInputField;
        [SerializeField] private TextMeshProUGUI autoCompleteText;
        [Header("suggestion Settings")]
        [SerializeField] private GameObject suggestionTextPrefab;
        [SerializeField] private RectTransform suggestionContent;
        [SerializeField] private RectTransform suggestionParent;
        [Header("Inputs")]
        [SerializeField] private KeyCode completionKey = KeyCode.Tab;

        

        private string commandInput = "";

        private void Start()
        {
            consoleInputField.onValueChanged.AddListener(OnConsoleInputValueChanged);
            consoleInputField.onSubmit.AddListener(OnConsoleInputSubmit);
        }

        
        private void OnConsoleInputSubmit(string arg0)
        {
            HandleCommandInput(arg0, InsertLog);
            consoleInputField.text = "";
            consoleInputField.Select();
            commandSuggestion.SetInput(commandInput);
            UpdateSuggestions();
        }

        private void OnConsoleInputValueChanged(string value)
        {
            commandInput = value;
            commandSuggestion.SetInput(commandInput);
            autoCompleteText.text = commandInput.IsNullOrEmpty()? "" : commandSuggestion.AutoComplete(false);
            UpdateSuggestions();
        }

        private void UpdateSuggestions()
        {
            var suggestions = commandSuggestion.Suggestions;
            
            suggestionContent.DestroyAllChildren();
            foreach (var suggestion in suggestions)
            {
                var textGo = Instantiate(suggestionTextPrefab, suggestionContent).GetComponentInChildren<TextMeshProUGUI>();
                textGo.text = suggestion;
            }
            suggestionParent.gameObject.SetActive(!suggestions.IsNullOrEmpty() && !commandInput.IsNullOrEmpty());
        }


        private void UpdateLayout()
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.normalizedPosition = new Vector2(0, 0);
        }


        private void InsertLog(string text, bool dateFormat = true)
        {
            var dateText = FormatInput(dateFormat);
            var log = $"{dateText}{text}";
            logList.Add(log);
            var textGo = Instantiate(textPrefab, content).GetComponentInChildren<TextMeshProUGUI>();
            textGo.text = log;
            UpdateLayout();
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

        private void Update()
        {
            if(commandInput.IsNullOrEmpty()) return;
            if (Input.GetKeyDown(completionKey))
            {
                var autoComplete = commandSuggestion.AutoComplete(true);
                if (!autoComplete.IsNullOrEmpty())
                {
                    consoleInputField.text = autoComplete;
                }
            }
        }

        protected override void CommandExecuteInvoke(string cmd)
        {
            base.CommandExecuteInvoke(cmd);
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
        
        
        public override void ToggleConsole()
        {
            base.ToggleConsole();
            if (isActive)
            {
                UpdateLayout();
                consoleInputField.SetTextWithoutNotify("");
                consoleInputField.Select();
                consoleInputField.ActivateInputField();
            }
            uiParentPanel.gameObject.SetActive(isActive);
        }
    }
}