using System;
using System.Collections.Generic;

namespace WeakReferenceEvent;

public abstract class WeakReferenceEventBase<TDelegate> 
    where TDelegate : Delegate
{
    protected readonly List<WeakDelegate<TDelegate>> _handerWeakRefList = new();

    public static WeakReferenceEventBase<TDelegate> operator +(
        WeakReferenceEventBase<TDelegate> weakRefEvent,
        TDelegate handler)
    {
        weakRefEvent.Add(handler);

        return weakRefEvent;
    }

    public static WeakReferenceEventBase<TDelegate> operator -(
        WeakReferenceEventBase<TDelegate> weakRefEvent,
        TDelegate handler)
    {
        weakRefEvent.Remove(handler);

        return weakRefEvent;
    }

    protected void Add(TDelegate handler)
    {
        if (!TryGetRegisteredDelegate(handler, out var _))
        {
            // Only add the handler if it's not already registered.
            this._handerWeakRefList.Add(new WeakDelegate<TDelegate>(handler));
        }
    }

    protected void Remove(TDelegate handler)
    {
        if (TryGetRegisteredDelegate(handler, out var delegateRef) && delegateRef != null)
        {
            this._handerWeakRefList.Remove(delegateRef);
        }
    }
    
    protected bool TryGetRegisteredDelegate(TDelegate handler, out WeakDelegate<TDelegate>? weakDelegate)
    {
        foreach (var delegateRef in this._handerWeakRefList)
        {
            if (ReferenceEquals(delegateRef.Target, handler.Target)
                && delegateRef.Method == handler.Method)
            {
                weakDelegate = delegateRef;
                return true;
            }
        }

        weakDelegate = null;
        return false;
    }

    public void DynamicSafeInvoke(params object[] args)
    {
        foreach (var handler in this.GetInvocationList())
        {
            handler.DynamicInvoke(args);
        }
    }

    public void DynamicInvoke(params object[] args)
    {
        foreach (var handler in this.GetInvocationList())
        {
            try
            {
                handler.DynamicInvoke(args);
            }
            catch { continue; }
        }
    }

    public IEnumerable<TDelegate> GetInvocationList()
    {
        List<TDelegate> toInclude = new(this._handerWeakRefList.Count);
        List<WeakDelegate<TDelegate>>? toRemove = null;

        foreach (var delegateRef in this._handerWeakRefList)
        {
            if (delegateRef.TryGetDelegate(out var handler) && handler is not null)
            {
                yield return handler;
            }
            else
            {
                (toRemove ??= new()).Add(delegateRef);
            }
        }

        if (toRemove != null)
        {
            foreach (var delegateRef in toRemove)
            {
                this._handerWeakRefList.Remove(delegateRef);
            }
        }
    }
}
