using GameKit.Attributes;
using GameKit.Entities.Interactable;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

namespace GameKit.Entities.DragDrop
{
    [PublicAPI]
    [RequireComponent(typeof(EntityInteractable))]
    public class EntityDragComponent: EntityComponent, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private static class DefaultLayers
        {
            public static int IdleSortingLayer;
            public static int DragSortingLayer;

#if UNITY_EDITOR
            [InitializeOnLoadMethod]
            private static void Initialize()
            {
                IdleSortingLayer = SortingLayer.NameToID("Default");
                DragSortingLayer = SortingLayer.NameToID("Dragged");
            }
#endif
        }

        [SortingLayer, SerializeField] private int idleSortingLayer = DefaultLayers.IdleSortingLayer;
        [SortingLayer, SerializeField] private int dragSortingLayer = DefaultLayers.DragSortingLayer;
        [SerializeField] private Transform draggableObject;

        private EntityInteractable _interactable;
        private Vector3 _startPosition;
        private Vector2 _deltaPosition;
        private SortingGroup _sortingGroup;
        private SpriteRenderer _renderer;
        private Camera _dragCamera;

        public event EntityPointerHandler EventBeginDrag;
        public event EntityPointerHandler EventDrag;
        public event EntityPointerHandler EventEndDrag;
        public event EntityHandler EventStartDrag;
        public event EntityHandler EventStopDrag;
        public event EntityDragDropHandler EventDropped;
        public bool IsDragged { get; private set; }

        internal void Drop(Entity place, PointerEventData eventData)
        {
            EventDropped?.Invoke(place, Entity, eventData);
        }
        
        void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
        {
            _dragCamera = Camera.main;
            if (enabled == false || _dragCamera is null) return;

            SetDragState(true);
            _startPosition = draggableObject.position;
            _deltaPosition = (Vector2)_startPosition - (Vector2)(_dragCamera.ScreenToWorldPoint(eventData.position));
            EventStartDrag?.Invoke(Entity);
            EventBeginDrag?.Invoke(Entity, eventData);
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            if (IsDragged == false) return;
            draggableObject.position = (Vector2)(_dragCamera.ScreenToWorldPoint(eventData.position)) + _deltaPosition;
            EventDrag?.Invoke(Entity, eventData);
        }

        void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            if (IsDragged == false) return;
            StopDrag();
            EventEndDrag?.Invoke(Entity, eventData);
        }

        private void Awake()
        {
            _interactable = GetComponent<EntityInteractable>();
            if (draggableObject == null) draggableObject = transform;

            _sortingGroup = draggableObject.GetComponent<SortingGroup>();
            if (_sortingGroup is null)
                _renderer = draggableObject.GetComponent<SpriteRenderer>();
            
            if (_sortingGroup is null && _renderer is null)
                Debug.LogError($"{name} draggable object not found SortingGroup or SpriteRenderer");
        }

        private void OnDisable()
        {
            if (IsDragged) StopDrag();
        }

        private void SetDragState(bool dragged)
        {
            IsDragged = dragged;
            if (_sortingGroup != null)
                _sortingGroup.sortingLayerID = dragged ? dragSortingLayer : idleSortingLayer;

            if (_renderer != null)
                _renderer.sortingLayerID = dragged ? dragSortingLayer : idleSortingLayer;
            
            _interactable.SetLocalInteractable(!dragged);
        }

        private void StopDrag()
        {
            SetDragState(false);
            draggableObject.position = _startPosition;
            _dragCamera = null;
            EventStopDrag?.Invoke(Entity);
        }
        
        protected override void Reset()
        {
            base.Reset();
            draggableObject = transform;
        }
    }
}