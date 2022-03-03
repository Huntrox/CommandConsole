using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
namespace HuntroxGames.Utils
{
    public class ConsoleMenuTrigger : MonoBehaviour
    {

        void Update()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current.backquoteKey.wasPressedThisFrame)
            {
                CommandConsole.Instance.Console();
                return;
            }
#endif
#if ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(KeyCode.BackQuote))
                CommandConsole.Instance.Console();
#endif
        }
    }
}
