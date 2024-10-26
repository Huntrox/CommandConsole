# Unity Command Console
very simple Command Console to create cheat codes and easy developers commands
![gif](https://i.imgur.com/KRsCrnx.gif)
## Features
- Methods, Fields and Properties
- Static and non-Static Objects
- Public and Private
- Method Return Types
- Method Arguments (float ,int, string, Vector2-3, Color, Enums) and more
- Auto Complete Suggestions
- Custom Command Options Callback
- Customizable UI
## How To Use

1. add CommandConsoleGUI prefab to the scene or CommandConsoleUI to the canvas whichever you prefer
2. tweak the settings in the inspector to your liking
3. add ConsoleCommandAttribute to any field property or method
4. open the console and type the command than press enter to execute the command
```c#
[ConsoleCommand]
//or with Constructor parameters
[ConsoleCommand(command: "SetPlayerGold", description: "Sets Player Gold Amount",objectExecutionType: MonoObjectExecutionType.Option)]
```
### Parameters
1. `command:` the command you need to type in the console to execute
    - default value: **(field, property, method) name**
2. `description:` command description in the help commands list
    - default value: **string.empty**
3. `helpMenu:` the option to include this command in the help commands list
    - default value: **True**
4. `objectExecutionType:` determines how the command would be executed
    - MonoObjectExecutionType.FirstInHierarchy: **will execute on the first object in hierarchy assigned to the command**
    - MonoObjectExecutionType.All: **will execute on all objects in hierarchy assigned with the command**
    - MonoObjectExecutionType.Option: **will allow the user to choose and execute the command on a specific object from the hierarchy.**
       This option provides flexibility, as it enables the user to select a single object from the hierarchy and apply the command only to that particular object.
    - default value: **MonoObjectExecutionType.FirstInHierarchy**


## Examples
- Methods
```c#
        [ConsoleCommand]
        public void KillAllEnemies()
        {
            Debug.Log("DIE DIE DIE!");
        }
        //private
        [ConsoleCommand(command: "SetPlayerGold", description: "",helpMenu: true,objectExecutionType: MonoObjectExecutionType.Option)]
        private void MethodWithArguments(string playerName, int gold)
        {
            Debug.Log($"player {playerName}'s gold: {gold}");
        }
        //static
        [ConsoleCommand]
        private static void LoadLevel(string levelName)
        {
            SceneManager.LoadScene(levelName);
        }
```
![gif](https://i.imgur.com/T07V2Dx.gif)
- Fields and Properties
```c#
        [ReadOnly] public int health = 0;
        //Multi Commands
        [ConsoleCommand("SetHealth","[Integer Input]")]
        [ConsoleCommand("GetHealth")]
        public int Health
        {
            get => health;
            set
            {
                Debug.Log("Health Property Value: " + value);
                health = value;
            }
        }
```
![gif](https://i.imgur.com/TDVH345.gif)

### Custom Command Options Callback

- This allows you to add your list of executable options in one command. 
- submitting the command without arguments will display the list of options.
- providing arguments will execute the selected option , e.g. `SetGraphics High` or `SetGraphics 2`, argument "`2`" is the index of the selected option "High"



```c#
        [ConsoleCommand]
        public CommandOptionsCallback SetGraphics()
        {
            var options = new CommandOptionsCallback();
            options.AddOption("Low", () => QualitySettings.SetQualityLevel(0));
            options.AddOption("Medium", () => QualitySettings.SetQualityLevel(1));
            options.AddOption("High", () => QualitySettings.SetQualityLevel(2));
            return options;
        }

        [ConsoleCommand]
        public CommandOptionsCallback SetLanguage()
        {
            return new CommandOptionsCallback(
                new CommandOption("English", () => Debug.Log("Language set to English")),
                new CommandOption("French", () => Debug.Log("Language set to French")),
                new CommandOption("Spanish", () => Debug.Log("Language set to Spanish")));
        }
   
```
![img](https://i.imgur.com/TKss9ss.png)

# Api
| Class / Method              | Description                                                                                                                                                                        |
|-----------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **CommandsHandler**             |                                                                                                                                                                                    |
| `FetchCommandAttributes()`  | Fetches all the commands from the static classes and the MonoBehaviours in the scene. This can be called manually if you want to update the commands in the console.           |
| `ExecuteCommand(command, commandArguments, executeLogCallback)` | Executes the command with the given arguments if the command is found.                                                                                             |
| `SetOptionsFormatter(formatter)`  | Sets the function to format command options. This function will be called when options are available for a command.                                                         |
| `GetConsoleCommandDescription()` | Gets a list of ConsoleCommandAttributes that describe all available commands in the console.                                                                                  |
| **CommandConsole**              |                                                                                                                                                                                    |
| Properties:                 |                                                                                                                                                                                    |
| `unityLogMessages`          | Indicates whether the console should receive Unity log messages.                                                                                                                  |
| `fetchMode`                 | Specifies when the console should fetch commands (OnSceneLoad, OnConsoleTrigger, BeforeExecutingAnyCommand).                                                                    |
| `objectNameDisplay`         | Specifies how the object name should be displayed (GameObject Name, Class Name, Class Member Name).                                                                             |
| `commandInputFieldColor`    | The color of the command input field.                                                                                                                                             |
| `textColor`                 | The color of the console text.                                                                                                                                                     |
| `parameterColor`            | The color of the command parameters.                                                                                                                                              |
| `autoCompleteColor`         | The color of the auto-completion text.                                                                                                                                            |
| `inputPrefixStyle`          | The style of the input prefix (Date, Dash, None, Custom).                                                                                                                         |
| `inputPrefixColor`          | The color of the input prefix.                                                                                                                                                    |
| `customInputPrefix`         | The custom input prefix to use if the inputPrefixStyle is set to Custom.                                                                                                          |
| `dateFormat`                | The date format to use if the inputPrefixStyle is set to Date. Default format `HH:MM:SS`                                                                         |
| `autoCompletionKey`         | The key used to trigger auto-completion.                                                                                                                                          |
| `submitKey`                 | The key used to submit a command.                                                                                                                                                 |
| Methods:                    |                                                                                                                                                                                    |
| `ToggleConsole()`           | Opens or closes the console. Invokes the OnConsole event with the current state (Active/Deactivate).                                                                              |
| `HandleCommandInput(commandInput, executionLogCallback)` | Handles the command input and executes the command if found.                                                                                                         |
| `FormatInput(prefix)`       | Formats the input with an optional prefix.                                                                                                                                        |
| Events:                     |                                                                                                                                                                                    |
| `OnConsole`                 | This event is raised every time the console is opened or closed.                                                                                                                 |
| `OnCommandExecuted`         | This event is raised every time a command is executed with its name.                                                                                                              |
| `OnCommandExecutedWithParameters` | This event is raised every time a command is executed with its name and parameters.                                                                                         |


