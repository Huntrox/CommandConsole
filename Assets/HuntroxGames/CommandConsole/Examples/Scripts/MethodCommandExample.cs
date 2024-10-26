using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HuntroxGames.Utils
{
    public class MethodCommandExample : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;

        [SerializeField]private Vector3 targetPosition;

        private void Update() =>
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 2);


        [ConsoleCommand("SetPlayerColor", "Change player color", MonoObjectExecutionType.Option)]
        private void SetColor(Color color)
        {
            if (spriteRenderer)
                spriteRenderer.color = color;
        }

        [ConsoleCommand]
        public void KillAllEnemies() => Debug.Log("DIE DIE DIE!");

        [ConsoleCommand(command: "SetPlayerGold")]
        public void MethodWithArguments(string playerName, int gold) =>
            Debug.Log($"player {playerName}'s gold: {gold}");

        [ConsoleCommand("ReloadScene")]
        private void Reload() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);


        [ConsoleCommand("AddNumbers", MonoObjectExecutionType.FirstInHierarchy)]
        private int AddNumbers(params int[] numbers) => numbers.Sum();

        //private int AddNumbers(int numberA, int numberB) => numberA + numberB;

        #region UnityLogMessages

        [ConsoleCommand]
        private void LogMessage() => Debug.Log("Unity Log Message!");

        [ConsoleCommand]
        private void WarningLogMessage() => Debug.LogWarning("Unity Warring Message");

        [ConsoleCommand]
        private void ErrorLogMessage() => Debug.LogError("Unity Error Message");

        #endregion

        #region Vector2-3 and Vector2Int-3Int

        [ConsoleCommand("SetPosition", MonoObjectExecutionType.Option)]
        public void SetPosition(Vector3 position)
        {
            Debug.Log($"Vector3 {position}");
            targetPosition = position;
        }

        [ConsoleCommand("SetPositionV2")]
        public void SetPosition(Vector2 vector2Position)
        {
            Debug.Log($"Vector2 {vector2Position}");
            targetPosition = vector2Position;
        }

        [ConsoleCommand("SetPositionV3INT")]
        public void SetPosition(Vector3Int vector3IntPosition)
        {
            Debug.Log($"Vector3Int {vector3IntPosition}");
            targetPosition = vector3IntPosition;
        }

        [ConsoleCommand("SetPositionV2Int")]
        public void SetPosition(Vector2Int vector2IntPosition)
        {
            Debug.Log($"Vector2Int {vector2IntPosition}");
            targetPosition = (Vector2) vector2IntPosition;
        }

        #endregion

        #region CommandOptionsCallback

        [ConsoleCommand]
        public CommandOptionsCallback SetLanguage()
        {
            return new CommandOptionsCallback(
                new CommandOption("English", () => Debug.Log("Language set to English")),
                new CommandOption("French", () => Debug.Log("Language set to French")),
                new CommandOption("Spanish", () => Debug.Log("Language set to Spanish")));
        }


        [ConsoleCommand]
        public CommandOptionsCallback SetGraphics()
        {
            var options = new CommandOptionsCallback();
            options.AddOption("Low", () => QualitySettings.SetQualityLevel(0));
            options.AddOption("Medium", () => QualitySettings.SetQualityLevel(1));
            options.AddOption("High", () => QualitySettings.SetQualityLevel(2));
            return options;
        }
        #endregion
    }
}