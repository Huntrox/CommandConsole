using HuntroxGames.Utils;
using JetBrains.Annotations;

[PublicAPI]
public static class PluginTest
{
    [ConsoleCommand]
    private const string CONST_STRING = "This is a test const string in a static class plugin, you can't change it from the console";
    
    [ConsoleCommand]
    private static readonly string ReadOnlyString = "This is a test readonly string in a static class plugin, You can't change it from the console";
}
