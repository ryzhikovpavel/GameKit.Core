using UnityEngine;

namespace GameKit.Entities.Transparency
{
    public class TransparencyGroup: ITransparency
    {
        private ITransparency[] _transparencies;
        
        public float alpha { get; set; }

        public TransparencyGroup(Transform owner)
        {
            _transparencies = owner.GetComponentsInChildren<ITransparency>(true);
        }
    }
}