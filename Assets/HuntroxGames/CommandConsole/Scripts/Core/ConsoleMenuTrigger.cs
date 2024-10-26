using UnityEngine;
#if ENABLE_INPUT_SYSTEM && UNITY_2021_OR_NEWER
using UnityEngine.InputSystem;
#endif
namespace HuntroxGames.Utils
{
    public class ConsoleMenuTrigger : MonoBehaviour
    {
#if ENABLE_LEGACY_INPUT_MANAGER
        [SerializeField] private KeyCode consoleKey = KeyCode.BackQuote;
#endif 
        
        
#if ENABLE_INPUT_SYSTEM && UNITY_2021_OR_NEWER
        [SerializeField] private InputAction consoleAction =
            new InputAction("<Keyboard>/backquote", InputActionType.Button, "<Keyboard>/backquote");

        private void Start() => consoleAction.Enable();
        
#endif
        
        
        public void Update()
        {
#if ENABLE_INPUT_SYSTEM && UNITY_2021_OR_NEWER

            if (consoleAction.triggered)
            {
                CommandConsole.Instance.ToggleConsole();
                return;
            }

#elif ENABLE_LEGACY_INPUT_MANAGER
            if (Input.GetKeyDown(consoleKey))
                CommandConsole.Instance.ToggleConsole();
#endif
        }
    }
}
