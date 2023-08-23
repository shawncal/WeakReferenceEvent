using System;

namespace WeakReferenceEvent;

public class WeakReferenceEventHandler<TEventArgs> : WeakReferenceEventBase<EventHandler<TEventArgs>>
    where TEventArgs : EventArgs
{
    public static WeakReferenceEventHandler<TEventArgs> operator +(
        WeakReferenceEventHandler<TEventArgs> weakRefEvent,
        EventHandler<TEventArgs> handler)
    {
        weakRefEvent.Add(handler);

        return weakRefEvent;
    }

    public static WeakReferenceEventHandler<TEventArgs> operator -(
        WeakReferenceEventHandler<TEventArgs> weakRefEvent,
        EventHandler<TEventArgs> handler)
    {
        weakRefEvent.Remove(handler);

        return weakRefEvent;
    }

    public void Invoke(object? sender, TEventArgs args)
    {
        foreach (var handler in this.GetInvocationList())
        {
            try
            {
                handler.Invoke(sender, args);
            }
            catch { continue; }
        }
    }
}
