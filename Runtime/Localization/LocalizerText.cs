#pragma warning disable 649
namespace GameKit
{
    public abstract class LocalizerText: Localizer
    {
        public bool upperFirstChar;
        
        protected override void OnLocalize()
        {
            Bind(Service<Localization>.Instance.Translate(Key), Service<Localization>.Instance.Translation.rtl);
        }
        
        public virtual void Bind(string text, bool rlt)
        {
            if (upperFirstChar) text = text.ToFirstUpper();
            UpdateText(text, rlt);
        }



        protected abstract void UpdateText(string text, bool rtl);
    }
}