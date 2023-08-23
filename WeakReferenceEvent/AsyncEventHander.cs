using System;
using System.Threading;
using System.Threading.Tasks;

namespace WeakReferenceEvent;

public delegate Task AsyncEventHandler(
    object? sender,
    EventArgs args,
    CancellationToken cancellationToken = default);