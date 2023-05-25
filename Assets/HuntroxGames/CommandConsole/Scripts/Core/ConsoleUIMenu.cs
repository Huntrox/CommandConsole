using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
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
        [Header("Suggestion Settings")]
        [SerializeField] private GameObject suggestionTextPrefab;
        [SerializeField] private ScrollRect suggestionView;
        [SerializeField] private RectTransform suggestionParent;
        [SerializeField] private Color suggestionTextColor = Color.white;
        [SerializeField] private Color selectedSuggestionTextColor = new Color32(65, 183, 25, 255);
        [Header("Inputs")]
        [SerializeField] private KeyCode completionKey = KeyCode.Tab;

        

        private string commandInput = "";

        private RectTransform suggestionContent;
        private void Start()
        {
            consoleInputField.onValueChanged.AddListener(OnConsoleInputValueChanged);
            consoleInputField.onSubmit.AddListener(OnConsoleInputSubmit);
            suggestionContent = suggestionView.content;
        }

        
        private void OnConsoleInputSubmit(string value)
        {
            HandleCommandInput(value, InsertLog);
            consoleInputField.text = "";
            consoleInputField.Select();
            commandSuggestion.SetInput(commandInput);
            LoadSuggestions();
        }

        private void OnConsoleInputValueChanged(string value)
        {
            commandInput = value;
            commandSuggestion.SetInput(commandInput);
            autoCompleteText.text = commandInput.IsNullOrEmpty() ? "" : commandSuggestion.AutoComplete(false);
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
                textTmp.color = index == commandSuggestion.CurrentIndex ? selectedSuggestionTextColor : suggestionTextColor;
                var eventTrigger = textGo.GetComponent<SuggestionTextUI>();
                eventTrigger.SetOnSelectAction(rectTransform =>
                {
                    consoleInputField.text = rectTransform.GetComponentInChildren<TextMeshProUGUI>().text;
                    consoleInputField.MoveToEndOfLine( false,false);
                    commandSuggestion.SetInput(consoleInputField.text);
                    LoadSuggestions();
                });
                index++;
            }
            suggestionParent.gameObject.SetActive(suggestions.Count > 1 && !commandInput.IsNullOrEmpty());
        }


        private void ScrollToSuggestion(RectTransform target)
        {
            //heck if the selected suggestion in content is visible in the view if not scroll to it
            // var objPosition = (Vector2)suggestionView.transform.InverseTransformPoint(target.position);
            // var scrollHeight = suggestionView.GetComponent<RectTransform>().rect.height;
            // var objHeight = target.rect.height;
            //
            // if (objPosition.y > scrollHeight / 2)
            // {
            //     suggestionContent.localPosition = new Vector2(0,
            //         suggestionContent.localPosition.y - objHeight - objHeight/2);
            // }
            //
            // if (objPosition.y < -scrollHeight / 2)
            // {
            //     suggestionContent.localPosition = new Vector2(0,
            //         suggestionContent.localPosition.y + objHeight  + objHeight/2);
            // }
            var anchoredPosition = 
                (Vector2)suggestionView.transform.InverseTransformPoint(suggestionContent.position)
                - (Vector2)suggestionView.transform.InverseTransformPoint(target.position);
            suggestionContent.anchoredPosition = new Vector2(0, anchoredPosition.y);
        }

        private void UpdateSuggestions()
        {
            for (int i = 0; i < suggestionContent.childCount; i++)
            {
                var index = i;
                var isSelect = index == commandSuggestion.CurrentIndex;
                var text = suggestionContent.GetChild(i).GetComponentInChildren<TextMeshProUGUI>();
                text.color = isSelect ? selectedSuggestionTextColor : suggestionTextColor;
                

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
            var textGo = Instantiate(textPrefab, content).GetComponentInChildren<TextMeshProUGUI>();
            textGo.text = log;
            UpdateLayout();
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
                var autoComplete = commandSuggestion.AutoComplete(false);
                if (!autoComplete.IsNullOrEmpty())
                {
                    consoleInputField.text = autoComplete;
                    consoleInputField.MoveTextEnd(false);
                }
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

        [ConsoleCommand]
        private void ClearConsole()
        {
            logList.Clear();
            consoleHistory.Clear();
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