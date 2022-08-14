using UnityEngine;
using UnityEngine.EventSystems;

namespace GameKit.Entities
{
    public delegate void EntityHandler(Entity entity);
    public delegate void EntityPointerHandler(Entity entity, PointerEventData eventData);
    public delegate void EntityDragDropHandler(Entity place, Entity dropped, PointerEventData eventData);
    
    public abstract class EntityComponent: MonoBehaviour
    {
        public Entity Entity { get; internal set; }
        
        protected virtual void Start()
        {
            if (Entity is null) Debug.LogError("Entity owner not found");
        }

        protected virtual void Reset()
        {
            if (transform.GetComponentInParent<Entity>() == false)
            {
                gameObject.AddComponent<Entity>();
            }
        }
    }
}