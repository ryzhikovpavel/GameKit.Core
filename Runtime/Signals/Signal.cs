using System;

namespace GameKit
{
    public class Signal : DispatchedSignal<Action>, ISignal
    {
        public void Dispatch()
        {
            var callbacks = BeginInvocation();
            foreach (var callback in callbacks)
            {
                if (callback is null) continue;
                try { callback(); }
                catch (Exception e) { Logger.Error(e); }
            }
            EndInvocation(callbacks);
            OnDispatched();
        }
    }
    
    public class Signal<T1>: DispatchedSignal<Action<T1>>, ISignal
    {
        public void Dispatch(T1 arg1)
        {
            var callbacks = BeginInvocation();
            foreach (var callback in callbacks)
            {
                if (callback is null) continue;
                try { callback(arg1); }
                catch (Exception e) { Logger.Error(e); }
            }
            EndInvocation(callbacks);
            OnDispatched();
        }
    }

    public class Signal<T1, T2>: DispatchedSignal<Action<T1, T2>>, ISignal
    {
        
        public void Dispatch(T1 arg1, T2 arg2)
        {
            var callbacks = BeginInvocation();
            foreach (var callback in callbacks)
            {
                if (callback is null) continue;
                try { callback(arg1, arg2); }
                catch (Exception e) { Logger.Error(e); }
            }
            EndInvocation(callbacks);
            OnDispatched();
        }
    }
    
    public class Signal<T1, T2, T3>: DispatchedSignal<Action<T1, T2, T3>>,  ISignal
    {
        public void Dispatch(T1 arg1, T2 arg2, T3 arg3)
        {
            var callbacks = BeginInvocation();
            foreach (var callback in callbacks)
            {
                if (callback is null) continue;
                try { callback(arg1, arg2, arg3); }
                catch (Exception e) { Logger.Error(e); }
            }
            EndInvocation(callbacks);
            OnDispatched();
        }
    }
    
    public class Signal<T1, T2, T3, T4>: DispatchedSignal<Action<T1, T2, T3, T4>>, ISignal
    {
        public void Dispatch(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            var callbacks = BeginInvocation();
            foreach (var callback in callbacks)
            {
                if (callback is null) continue;
                try { callback(arg1, arg2, arg3, arg4); }
                catch (Exception e) { Logger.Error(e); }
            }
            EndInvocation(callbacks);
            OnDispatched();
        }
    }
}