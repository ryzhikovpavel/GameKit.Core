#pragma warning disable 649
using UnityEngine;

namespace GameKit.Localizers
{
    public class LocalizerTextWithFormat: LocalizerText
    {
        [SerializeField] private string[] values;
        
        public override void Bind(string text)
        {
            base.Bind(string.Format(text, values));
        }
    }
}