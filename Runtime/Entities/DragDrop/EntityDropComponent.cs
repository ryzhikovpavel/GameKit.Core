using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameKit.Entities.DragDrop
{
    [PublicAPI]
    [RequireComponent(typeof(Collider2D))]
    public class EntityDropComponent : EntityComponent, IDropHandler
    {
        public event EntityDragDropHandler EventDropped;
        
        void IDropHandler.OnDrop(PointerEventData eventData)
        {
            if (enabled == false) return;
            
            EntityDragComponent dragged = eventData.pointerDrag.GetComponent<EntityDragComponent>();
            if (dragged == null || dragged.gameObject == gameObject) return;
            dragged.Drop(Entity, eventData);
            EventDropped?.Invoke(Entity, dragged.Entity, eventData);
        }
    }
}