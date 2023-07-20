# Unity Command Console
very simple Command Console to create cheat codes and easy developers commands
![gif](https://i.imgur.com/KRsCrnx.gif)
## Features
- Methods, Fields and Properties
- Static and non Static Objects
- Public and Private
- Method Return Types
- Method Prameters (float ,int, string, Vector2-3, Color, Enums) and more
- Auto Complete Suggestions
- Custom Command Options Callback
- Customizable UI
## How To Use

simply add ConsoleCommandAttribute to any field property or method
```c#
[ConsoleCommand(command: "SetPlayerGold", description: "Sets Player Gold Amount",objectExecutionType: MonoObjectExecutionType.Option)]
```
### Constructor parameters
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
        
        [ConsoleCommand(command: "SetPlayerGold", description: "",helpMenu: true,objectExecutionType: MonoObjectExecutionType.Option)]
        public void MethodWithArguments(string playerName, int gold)
        {
            Debug.Log($"player {playerName}'s gold: {gold}");
        }
```
![gif](https://i.imgur.com/T07V2Dx.gif)
- Fields and Properties
```c#
        [ReadOnly] public int health = 0;
        [ConsoleCommand("SetHealth","[int]")]
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

- Custom Command Options Callback

  allows you to add your list of executable options in one command 
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
        public CommandOptionsCallback TeleportTo()
            => new CommandOptionsCallback(
                new CommandOption("Wolfden", () => Debug.Log("teleported to Wolfden")),
                new CommandOption("Sudbury", () => Debug.Log("teleported to Sudbury")),
                new CommandOption("Hogsfeet", () => Debug.Log("teleported to Hogsfeet")));
   
```
 ![img](https://i.imgur.com/mCb7dZm.png)
