using UnityEngine;
using UnityEngine.SceneManagement;

namespace HuntroxGames.Utils
{
    public class MethodCommandExample : MonoBehaviour
    {        
        private Vector3 targetPosition;

         // if (Input.GetKeyDown(KeyCode.BackQuote))
         //        Console();
        private void Update() =>
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 2);


        [ConsoleCommand]
        public void KillAllEnemies() => Debug.Log("DIE DIE DIE!");
        [ConsoleCommand("SetPlayerGold","[string] [int]")]
        public void MethodWithArguments(string playerName,int gold) => Debug.Log($"player {playerName}'s gold has been set to {gold}");

        [ConsoleCommand("ReloadScene")]
        private void Reload() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        [ConsoleCommand("AddNumbers")]
        // private int AddNumbers(int[] numbers) => numbers.Sum();
        private int AddNumbers(int a, int b) => a + b;
        
        #region Vector2-3 and Vector2Int-3Int

        [ConsoleCommand("SetPosition","[x,y,z]")]
        public void SetPosition(Vector3 position)
        {
            Debug.Log($"Vector3 {position}");
            targetPosition = position;
        }
        [ConsoleCommand("SetPositionV2")]
        public void SetPosition(Vector2 position)
        {
            Debug.Log($"Vector2 {position}");
            targetPosition = position;
        }
        [ConsoleCommand("SetPositionV3INT","[x,y,z]:int")]
        public void SetPosition(Vector3Int position)
        {
            Debug.Log($"Vector3Int {position}");
            targetPosition = position;
        }
        [ConsoleCommand("SetPositionV2Int")]
        public void SetPosition(Vector2Int position)
        {
            Debug.Log($"Vector2Int {position}");
            targetPosition = (Vector2)position;
        }
        #endregion
    }
}