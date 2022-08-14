using UnityEngine;

namespace GameKit.Entities.Interactable
{
    public class EntityInteractableGroup : IEntityInteractable
    {
        private IEntityInteractable[] _interactables;
        private bool _interactable;
        
        public bool Interactable { get => _interactable; set => SetInteractable(value); }

        private void SetInteractable(bool value)
        {
            _interactable = value;
            foreach (var e in _interactables)
            {
                e.Interactable = value;
            }
        }

        public EntityInteractableGroup(Transform owner)
        {
            _interactables = owner.GetComponentsInChildren<IEntityInteractable>(true);
            if (_interactables.Length == 0) return;
            _interactable = _interactables[0].Interactable;

            foreach (var e in _interactables)
            {
                if (e.Interactable != _interactable)
                {
                    Debug.LogWarning($"Entity interactable value is different");
                }
            }

        }
    }
}