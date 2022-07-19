using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif
namespace HuntroxGames.Utils
{
    public class ConsoleMenuTrigger : MonoBehaviour
    {
        
#if ENABLE_INPUT_SYSTEM
        [SerializeField] private InputAction consoleAction =
            new InputAction("<Keyboard>/backquote", InputActionType.Button, "<Keyboard>/backquote");
#endif
        
#if ENABLE_LEGACY_INPUT_MANAGER
        [SerializeField] private KeyCode consoleKey = KeyCode.BackQuote;
#endif
        public void Update()
        {
#if ENABLE_INPUT_SYSTEM
            
            if (consoleAction.triggered)
            {
                CommandConsole.Instance.Console();
                return;
            }

#elif ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(consoleKey))
                CommandConsole.Instance.Console();
#endif
        }
    }
}
