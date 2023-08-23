using System;
using System.Threading;
using System.Threading.Tasks;

namespace WeakReferenceEvent;

public delegate Task AsyncEventHandler<TEventArgs>(
    object? sender,
    TEventArgs args,
    CancellationToken cancellationToken = default) where TEventArgs : EventArgs;