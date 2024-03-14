using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HuntroxGames.Utils
{
    
    public class ConsoleUIMenu : CommandConsole
    {
        [Header("UI Font Style")]
        [SerializeField] private float inputFontSize = 22;
        [SerializeField] private float outputFontSize = 22;
        [SerializeField] private float parameterFontSize = 22;
        [SerializeField] private float autoCompleteFontSize = 22;
        [Header("Command Options Font Style")]
        [SerializeField] private Color optionTextColor = Color.white;
        [Header("UI Settings")]
        [SerializeField] private GameObject textPrefab;
        [SerializeField] private RectTransform content;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform uiParentPanel;
        [SerializeField] private TMP_InputField consoleInputField;
        [SerializeField] private TextMeshProUGUI autoCompleteText;
        [SerializeField] private bool checker = false;
        [Header("Suggestion Settings")]
        [SerializeField] private GameObject suggestionTextPrefab;
        [SerializeField] private ScrollRect suggestionView;
        [SerializeField] private RectTransform suggestionParent;
        [SerializeField] private Color suggestionTextColor = Color.white;
        [SerializeField] private Color selectedSuggestionTextColor = new Color32(65, 183, 25, 255);
        
        private string commandInput = "";

        private RectTransform suggestionContent;
        private void Start()
        {
            CommandsHandler.SetOptionsFormatter(OptionsCallbackFormatter);
            consoleInputField.onValueChanged.AddListener(OnConsoleInputValueChanged);
            consoleInputField.onSubmit.AddListener(OnConsoleInputSubmit);
            suggestionContent = suggestionView.content;
            foreach (var tmp in consoleInputField.GetComponentsInChildren<TextMeshProUGUI>())
                tmp.fontSize = inputFontSize;
            
        }

        
        private void OnConsoleInputSubmit(string value)
        {
            if (suggestionParent.gameObject.activeSelf)
            {
                AutoComplete();
                return;
            }
            
            HandleCommandInput(value, InsertLog);
            consoleInputField.text = "";
            consoleInputField.Select();
            commandSuggestion.SetInput(commandInput);
            consoleInputField.ActivateInputField();
            LoadSuggestions();
        }

        private void OnConsoleInputValueChanged(string value)
        {
            commandInput = value;
            commandSuggestion.SetInput(commandInput);
            autoCompleteText.text = commandInput.IsNullOrEmpty() ? "" : commandSuggestion.AutoComplete(false);
            autoCompleteText.fontSize = inputFontSize;
            LoadSuggestions();
        }

        private void LoadSuggestions()
        {
            var suggestions = commandSuggestion.Suggestions;
            
            suggestionContent.DestroyAllChildren();
            var index = 0;
            foreach (var suggestion in suggestions)
            {
                var textGo = Instantiate(suggestionTextPrefab, suggestionContent);
                var textTmp = textGo.GetComponentInChildren<TextMeshProUGUI>();
                textTmp.text = suggestion;
                textTmp.fontSize = autoCompleteFontSize;
                textTmp.color = index == commandSuggestion.CurrentIndex ? selectedSuggestionTextColor : suggestionTextColor;
                var eventTrigger = textGo.GetComponent<SuggestionTextUI>();
                eventTrigger.SetOnSelectAction(rectTransform =>
                {
                    consoleInputField.text = rectTransform.GetComponentInChildren<TextMeshProUGUI>().text;
                    commandSuggestion.SetInput(consoleInputField.text);
                    EventSystem.current.SetSelectedGameObject(consoleInputField.gameObject);
                    consoleInputField.MoveTextEnd(false);
                    consoleInputField.MoveToEndOfLine(false, false);
                    consoleInputField.DeactivateInputField();

                    //consoleInputField.MoveToEndOfLine( false,true);
                    LoadSuggestions();
                });
                index++;
            }
            suggestionParent.gameObject.SetActive(suggestions.Count > 1 && !commandInput.IsNullOrEmpty());
        }


        private void ScrollToSuggestion(RectTransform target) 
            => suggestionView.GetComponent<ScrollRectEnsureVisible>().CenterOnItem(target);

        private void UpdateSuggestions()
        {
            for (int i = 0; i < suggestionContent.childCount; i++)
            {
                var index = i;
                var isSelect = index == commandSuggestion.CurrentIndex;
                var text = suggestionContent.GetChild(i).GetComponentInChildren<TextMeshProUGUI>();
                text.color = isSelect ? selectedSuggestionTextColor : suggestionTextColor;
                suggestionContent.GetChild(i).GetComponent<Image>().color = isSelect ? new Color(1, 1, 1, 0.3f) : Color.clear;

                if (!isSelect) 
                    continue;
                
                UpdateLayout();
                ScrollToSuggestion(text.rectTransform);
           
    
            }
            autoCompleteText.text = commandInput.IsNullOrEmpty() ? "" : commandSuggestion.AutoComplete(false);
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
            var isChecker = checker && content.childCount % 2 == 0;
            var textGo = Instantiate(textPrefab, content).GetComponentInChildren<TextMeshProUGUI>();
            textGo.text = log;
            textGo.fontSize = outputFontSize;
            if (isChecker)
                textGo.GetComponentInParent<Image>().color = new Color(1, 1, 1, 0.03f);

            UpdateLayout();
        }
        
        private string OptionsCallbackFormatter(CommandOptionsCallback optnsCallback)
        {
            var optionsLog = "";
            var index = 0;
            var color = ColorUtility.ToHtmlStringRGBA(optionTextColor);
            var whiteSpace = "           ";
            foreach (var option in optnsCallback.options)
            {
                optionsLog += $"<color=#{color}>Option[{index}]: {option.Value.optionName}</color>";
                index++;
                if (index < optnsCallback.options.Count)
                    optionsLog += $"\n\n{whiteSpace}";
            }
            return optionsLog;
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
            
            if (Input.GetKeyDown(autoCompletionKey))
            {
                AutoComplete();
            }
            
            //if(!isActive) return;
            if(commandSuggestion.Suggestions.IsNullOrEmpty()) return;
            
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                commandSuggestion.Previous();
                UpdateSuggestions();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                commandSuggestion.Next();
                UpdateSuggestions();
            }
        }

        private void AutoComplete()
        {
            var autoComplete = commandSuggestion.AutoComplete(false);
            if (!autoComplete.IsNullOrEmpty())
            {
                consoleInputField.Select();
                consoleInputField.ActivateInputField();
                consoleInputField.MoveToEndOfLine(false, true);
                consoleInputField.text = autoComplete;
                consoleInputField.MoveTextEnd(false);
            }
        }

        [ConsoleCommand]
        private void ClearConsole()
        {
            logList.Clear();
            consoleHistory.Clear();
            content.DestroyAllChildren();
            LoadSuggestions();
            UpdateLayout();
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
            uiParentPanel.gameObject.SetActive(isActive);
            if (!isActive) return;
            UpdateLayout();
            consoleInputField.SetTextWithoutNotify("");
            consoleInputField.Select();
            consoleInputField.ActivateInputField();
        }
    }
}