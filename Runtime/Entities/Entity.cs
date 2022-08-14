using System;
using System.Collections.Generic;
using GameKit.Entities.DragDrop;
using GameKit.Entities.Interactable;
using GameKit.Entities.Transparency;
using JetBrains.Annotations;
using UnityEngine;

namespace GameKit.Entities
{
    [PublicAPI]
    public class Entity: MonoBehaviour
    {
        private IEntityInteractable _interactable;
        
        public Transform Transform { get; private set; }
        public RectTransform RectTransform { get; private set; }
        public ITransparency Transparency { get; private set; }
        public SpriteRenderer Renderer { get; private set; }
        public EntityDragComponent Drag { get; private set; }
        public EntityDropComponent Drop { get; private set; }
        public bool Interactable { get => _interactable.Interactable; set => _interactable.Interactable = value; }

        private void Awake()
        {
            Transform = transform;
            var components = Transform.GetComponentsInChildren<EntityComponent>(true);
            foreach (var component in components)
            {
                if (component.Entity != null)
                    Debug.LogError("Entity is defined in component");
                component.Entity = this;
            }

            InitializeCaches();
        }

        private void InitializeCaches()
        {
            RectTransform = Transform as RectTransform;
            Transparency = new TransparencyGroup(Transform);
            _interactable = new EntityInteractableGroup(Transform);
            Renderer = GetComponent<SpriteRenderer>();
            
            Drag = GetComponent<EntityDragComponent>();
            Drop = GetComponent<EntityDropComponent>();
        }
    }
}