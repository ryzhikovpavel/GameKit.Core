using UnityEngine;

namespace GameKit.Localizers
{
    public class LocalizerTextWithPfix: LocalizerText
    {
        [SerializeField] private string prefix;
        [SerializeField] private string postfix;
        
        public override void Bind(string text)
        {
            base.Bind(prefix + text + postfix);
        }
    }
}