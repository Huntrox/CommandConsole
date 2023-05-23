#define COMMANDS_CONSOLE
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace HuntroxGames.Utils
{
    [PublicAPI]
    public abstract class CommandConsole : Singleton<CommandConsole>
    {
        [Flags]
        private enum FetchMode
        {
            OnSceneLoad = 1 << 0,
            OnConsoleTrigger = 1 << 1,
            BeforeExecutingAnyCommand = 1 << 2,
        }

        public enum InputPrefixStyle
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
        [SerializeField] private ObjectNameDisplayType objectNameDisplay = ObjectNameDisplayType.GameObject | ObjectNameDisplayType.Member;
        
        [Header("Style")] 
        [SerializeField] protected Color commandInputFieldColor = new Color32(65, 183, 25, 255);
        [SerializeField] protected Color textColor = Color.white;
        [SerializeField] protected Color parameterColor = new Color(1, 1, 1, 0.25f);
        [SerializeField] protected Color autoCompleteColor = new Color(1, 1, 1, 0.25f);
        [Header("Input Style")] 
        [SerializeField] protected InputPrefixStyle inputPrefixStyle = InputPrefixStyle.Date;
        [SerializeField] protected Color inputPrefixColor = Color.yellow;
        [SerializeField] protected string customInputPrefix = "";
        
        public ObjectNameDisplayType ObjectNameDisplay => objectNameDisplay;


        /// <summary>
        /// this event will rise everytime when the console opens or close
        /// Usage Example: add listener and use it to pause/unpause the game
        /// </summary>
        public static event Action<bool> OnConsole;
        /// <summary>
        /// this event will rise everytime when the command is executed with command name
        /// </summary>
        public static event Action<string> OnCommandExecuted;
        /// <summary>
        /// this event will rise everytime when the command is executed with command name and parameters
        /// </summary>
        public static event Action<string,string[]> OnCommandExecutedWithParameters;
#if COMMANDS_CONSOLE
        
        protected bool isActive;
        protected readonly List<string> logList = new List<string>();
        protected readonly ConsoleHistory consoleHistory = new ConsoleHistory();
        protected readonly CommandSuggestion commandSuggestion = new CommandSuggestion();


        
        protected override void Awake()
        {
            base.Awake();
            SceneManager.sceneLoaded += OnSceneLoaded;
            CommandsHandler.FetchCommandAttributes();
        }

        private void OnEnable()
        {
            if (unityLogMessages)
                Application.logMessageReceivedThreaded += OnUnityLogMessageReceived;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        protected virtual void OnUnityLogMessageReceived(string condition, string stacktrace, LogType type)
        {
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
        
        
        /// <summary>
        /// can use this method to open the console.
        /// this will also invoke the <see cref="OnConsole"/> event with state value (Active/Deactive)
        /// </summary>
        public virtual void ToggleConsole()
        {
            isActive = !isActive;
            if (isActive)
            {
                if (fetchMode.HasFlag(FetchMode.OnConsoleTrigger))
                    CommandsHandler.FetchCommandAttributes();
            }
            OnConsole?.Invoke(isActive);
        }
        
        protected void HandleCommandInput(string commandInput,Action<string , bool> executionLogCallback)
        {
            executionLogCallback?.Invoke(commandInput, true);
            consoleHistory.Add(commandInput);
            if (fetchMode.HasFlag(FetchMode.BeforeExecutingAnyCommand))
                CommandsHandler.FetchCommandAttributes();
            var (cmd, @params) = ConsoleCommandHelper.SplitCommand(commandInput);
            CommandsHandler.ExecuteCommand(cmd, @params, executionLogCallback);
            CommandExecuteInvoke(cmd);
            CommandExecuteWithParametersInvoke(cmd, @params);
        }


        protected virtual void CommandExecuteInvoke(string cmd)
        {
            OnCommandExecuted?.Invoke(cmd);
        }
        protected virtual void CommandExecuteWithParametersInvoke(string cmd, string[] @params)
        {
            OnCommandExecutedWithParameters?.Invoke(cmd, @params);
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