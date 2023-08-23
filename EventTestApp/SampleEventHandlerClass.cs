namespace EventTestApp;

public class SampleClass : IDisposable
{
    public string Name { get; set; }

    public SampleClass(string name = "SampleClass (instance)")
    {
        Name = name;
    }

    ~SampleClass()
    {
        Console.WriteLine($"Calling finalizer for {this.Name}");
    }

    // Instance method handler
    public void PreInvokeHandler_1(object? sender, FunctionEventArgs eventData)
    {
        if (this._disposed) { throw new InvalidOperationException($"{this.Name}: POST A  !!!Callback on a disposed object!!!"); }

        Console.WriteLine($"{this.Name}: PRE 1");
    }

    // Instance method handler that marks "eventData.Handled = true;"
    public void PreInvokeHandler_2(object? sender, FunctionEventArgs eventData)
    {
        if (this._disposed) { throw new InvalidOperationException($"{this.Name}: POST A  !!!Callback on a disposed object!!!"); }

        Console.WriteLine($"{this.Name}: PRE 2");
        eventData.Handled = true;
    }

    // Static handler
    public static void PreInvokeHandler_3(object? sender, FunctionEventArgs eventData)
    {
        Console.WriteLine("SampleClass (static): PRE 3");
    }

    // Instance mehod handler
    public async Task PostInvokeHandler_A(object? sender, FunctionCompletedEventArgs eventData, CancellationToken cancellationToken)
    {
        if (this._disposed) { throw new InvalidOperationException($"{this.Name}: POST A  !!!Callback on a disposed object!!!"); }

        Console.WriteLine($"{this.Name}: POST A");
        await Task.Delay(1000, cancellationToken);
    }

    // Instance method handler that marks "eventData.Handled = true;"
    public async Task PostInvokeHandler_B(object? sender, FunctionCompletedEventArgs eventData, CancellationToken cancellationToken)
    {
        if (this._disposed) { throw new InvalidOperationException($"{this.Name}: POST B  !!!Callback on a disposed object!!!"); }

        Console.WriteLine($"{this.Name}: POST B");
        eventData.Handled = true;
        await Task.Delay(1000, cancellationToken);
    }

    // Static handler
    public static async Task PostInvokeHandler_C(object? sender, FunctionCompletedEventArgs eventData, CancellationToken cancellationToken)
    {
        Console.WriteLine("SampleClass (static): POST C");
        await Task.Delay(1000, cancellationToken);
    }

    private bool _disposed = false;

    public void Dispose()
    {
        Console.WriteLine($"{this.Name}: Disposing");
        this._disposed = true;
    }
}
