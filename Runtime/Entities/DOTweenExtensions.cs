using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using GameKit.Entities.Transparency;

namespace GameKit.Entities
{
    public static class DoTweenExtensions
    {
        // ReSharper disable once InconsistentNaming
        public static TweenerCore<float, float, FloatOptions> DOFade(this ITransparency target, float endValue, float duration)
        {
            TweenerCore<float, float, FloatOptions> t = DOTween.To(() => target.alpha, x => target.alpha = x, endValue, duration);
            t.SetTarget(target);
            return t;
        }
    }
}