using WeakReferenceEvent;

namespace EventTestApp;

public class FunctionCompletedEventArgs : EventArgs
{
    public FunctionCompletedEventArgs(string? result)
    {
        this.Result = result;
    }

    public string? Result { get; }
    public bool Handled { get; set; } = false;
}

public class MockFunctionDefinition
{
    private WeakReferenceEventHandler<FunctionEventArgs> _invokingEventManager = new();
    public event EventHandler<FunctionEventArgs> Invoking
    {
        add => this._invokingEventManager += value;
        remove => this._invokingEventManager -= value;
    }

    private WeakReferenceAsyncEventHandler<FunctionCompletedEventArgs> _completedEventManager = new();
    public event AsyncEventHandler<FunctionCompletedEventArgs> Completed
    {
        add => this._completedEventManager += value;
        remove => this._completedEventManager -= value;
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        // Invoking the delegate will invoke ALL the (pre-) handlers
        this._invokingEventManager.Invoke(this, new FunctionEventArgs("Invoking"));

        // Invoking the delegate will invoke ALL the (post-) handlers
        await this._completedEventManager.InvokeAsync(this, new FunctionCompletedEventArgs("Completed"), cancellationToken);
    }

    public void RunStepwise()
    {
        // If we want to invoke the handlers one at a time, we can run it in a loop by calling the delegate's
        // GetInvocationList() method, which returns an array of delegates. We can then invoke each handler, and check
        // some value between calls.

        var preEventData = new FunctionEventArgs("Starting");
        foreach (var handler in this._invokingEventManager.GetInvocationList())
        {
            handler(this, preEventData);
            if (preEventData.Handled)
            {
                // If one of the handlers sets Handled to true, we stop invoking the rest of the handlers
                break;
            }
        }
    }
}
