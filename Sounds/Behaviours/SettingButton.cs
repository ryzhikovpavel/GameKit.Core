using System;
using GameKit;
using Sources.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Sources.UI.Behaviours
{
    public class SettingButton : MonoBehaviour, IPointerClickHandler
    {
        private enum Type
        {
            Sound,
            Music
        }

        [SerializeField] private Type type;
        
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            switch (type)
            {
                case Type.Sound:
                    Service<Sound>.Instance.SoundMuted = !Service<Sound>.Instance.SoundMuted;
                    break;
                case Type.Music:
                    Service<Sound>.Instance.MusicMuted = !Service<Sound>.Instance.MusicMuted;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            UpdateView();
        }

        private void UpdateView()
        {
            
        }
    }
}