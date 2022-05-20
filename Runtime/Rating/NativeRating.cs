using System;
using GameKit.Rating.Implementation;

// ReSharper disable once CheckNamespace
namespace GameKit
{
    public abstract class NativeRating
    {
        public static event Action EventRated;
        
        /// <summary>
        /// Готово ли к показу нативное окно
        /// </summary>
        public bool IsReady { get; protected set; }

        /// <summary>
        /// Создает инстанс для соответствующей платформы.
        /// Подготовка сессии происходит некоторое время, но она не вечная, поэтому создавать нужно перед показом
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static NativeRating Create()
        {
#if UNITY_EDITOR
            return new EditorPlatformReview();
#elif UNITY_ANDROID
            return new AndroidPlatformReview();
#elif UNITY_IOS
            return new IosPlatformReview();
#endif
            throw new NotSupportedException();
        }
        
        protected NativeRating(){}
        protected void RatedDispatch() => EventRated?.Invoke();
        
        /// <summary>
        /// Инициирует отображение нативного окна, если IsReady = true
        /// В противном случае откроен ссылку в браузере
        /// </summary>
        /// <param name="onComplete"></param>
        public abstract void Open(Action onComplete);
        
        /// <summary>
        /// Очищает созданные для показа объекты
        /// </summary>
        public abstract void Dispose();
    }
}