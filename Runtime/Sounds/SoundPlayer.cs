using UnityEngine;
using UnityEngine.EventSystems;

namespace GameKit
{
    public class SoundPlayer: MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Sound sound;
        
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            sound.Play();
        }
    }
}