using UnityEngine;
using UnityEngine.EventSystems;

namespace GameKit.Behaviours
{
    public class SettingButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private AudioChannel channel;
        [SerializeField] private GameObject[] enabledObjects;
        [SerializeField] private GameObject[] disabledObjects;

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            Service<GameAudio>.Instance.SetMute(channel,!Service<GameAudio>.Instance.IsMuted(channel));
        }

        private void OnEnable()
        {
            Service<GameAudio>.Instance.EventChannelMutedChanged += UpdateView;
            UpdateView();
        }

        private void OnDisable()
        {
            Service<GameAudio>.Instance.EventChannelMutedChanged -= UpdateView;
        }
        
        private void UpdateView()
        {
            SetView(Service<GameAudio>.Instance.IsMuted(channel) == false);
        }

        private void SetView(bool enable)
        {
            foreach (var o in enabledObjects)
            {
                o.SetActive(enable);
            }

            foreach (var o in disabledObjects)
            {
                o.SetActive(enable == false);
            }
        }
    }
}