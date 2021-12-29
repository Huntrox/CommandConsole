# Unity Command Console

### very simple Command Console to create cheat codes and easy developers commands
![gif](https://i.imgur.com/KRsCrnx.gif)
## Features
- Static and non static objects
- Public and non-public
- Method return types
- Fields and properties access
- Method arguments
- Auto Complete Suggestions

## How To Use

simply add ConsoleCommandAttribute to any field property or method
```c#
[ConsoleCommand(command:"SetPlayerGold",description:"[string] [int]",helpMenu:true)]
```
### Constructor parameters
1. `command:` the command you need to type in the console to execute
    - default value: **(field, property, method) name**
2. `description:` command description in the help commands list
    - default value: **string.empty**
3. `helpMenu:` the option to include this command in the help commands listj
    - default value: **True**

## Examples
- Methods
```c#
        [ConsoleCommand]
        public void KillAllEnemies()
        {
            Debug.Log("DIE DIE DIE!");
        }
        
        [ConsoleCommand(command: "SetPlayerGold", description: "[string] [int]", helpMenu: true)]
        public void MethodWithArguments(string playerName, int gold)
        {
            Debug.Log($"player {playerName}'s gold: {gold}");
        }
```
![gif](https://i.imgur.com/T07V2Dx.gif)
- Properties
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
