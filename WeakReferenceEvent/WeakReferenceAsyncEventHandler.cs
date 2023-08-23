using System;
using System.Threading;
using System.Threading.Tasks;

namespace WeakReferenceEvent;

public class WeakReferenceAsyncEventHandler<TEventArgs> : WeakReferenceEventBase<AsyncEventHandler<TEventArgs>>
    where TEventArgs : EventArgs
{
    public static WeakReferenceAsyncEventHandler<TEventArgs> operator +(
        WeakReferenceAsyncEventHandler<TEventArgs> weakRefEvent,
        AsyncEventHandler<TEventArgs> handler)
    {
        weakRefEvent.Add(handler);

        return weakRefEvent;
    }

    public static WeakReferenceAsyncEventHandler<TEventArgs> operator -(
        WeakReferenceAsyncEventHandler<TEventArgs> weakRefEvent,
        AsyncEventHandler<TEventArgs> handler)
    {
        weakRefEvent.Remove(handler);

        return weakRefEvent;
    }

    public async Task InvokeAsync(object? sender, TEventArgs args, CancellationToken cancellationToken = default)
    {
        foreach (var handLer in this.GetInvocationList())
        {
            try
            {
                await handLer.Invoke(sender, args, cancellationToken);
            }
            catch { continue; }
        }
    }
}
