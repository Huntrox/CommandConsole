using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace HuntroxGames.Utils
{
    public class SuggestionTextUI : MonoBehaviour ,IPointerClickHandler
    {
        
        public UnityEvent<RectTransform> onSelect = new UnityEvent<RectTransform> ();
        
        public void SetOnSelectAction(UnityAction<RectTransform>  action)
        {
            onSelect.AddListener(action);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            onSelect.Invoke(this.GetComponent<RectTransform>());
        }
    }
}