using UnityEngine;

namespace HuntroxGames.Utils
{
    public class ConsoleMenuTrigger : MonoBehaviour
    {
        [SerializeField] private KeyCode consoleKey = KeyCode.BackQuote;
        public void Update()
        {
#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(consoleKey))
                CommandConsole.Instance.ToggleConsole();
#endif
        }
    }
}
