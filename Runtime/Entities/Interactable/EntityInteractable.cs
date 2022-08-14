using UnityEngine;
// ReSharper disable Unity.NoNullPropagation

namespace GameKit.Entities.Interactable
{
    [RequireComponent(typeof(Collider2D))]
    public class EntityInteractable: EntityComponent, IEntityInteractable
    {
        private Collider2D _collider;
        private bool _globalInteractable;
        private bool _localInteractable;

        public bool Interactable
        {
            get => _collider?.enabled ?? false;
            set => SetInteractable(value);
        }

        public void SetLocalInteractable(bool value)
        {
            _localInteractable = value;
            _collider.enabled = _localInteractable && _globalInteractable;
        }

        private void SetInteractable(bool value)
        {
            if (_collider is null)
            {
                Debug.LogWarning("Entity not contain colliders");
                return;
            }

            _globalInteractable = value;
            _collider.enabled = _localInteractable && _globalInteractable;
        }
        
        private void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _localInteractable = _collider.enabled;
            _globalInteractable = _localInteractable;
        }
    }
}