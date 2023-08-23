namespace EventTestApp;

internal class Program
{
    public static async Task Main(string[] args)
    {
        //await RunDemo1Async();

        //RunDemo2();

        //await RunDemo3Async();

        await RunDemo4Async();

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    /// <summary>
    /// Shows use of multicast delegates as event handlers (multiple handlers per event)
    /// </summary>
    /// <returns></returns>
    public static async Task RunDemo1Async()
    {
        Console.WriteLine("Starting Demo 1");

        SampleClass sampleInstance = new();

        var myFunction = new MockFunctionDefinition();
        myFunction.Invoking += sampleInstance.PreInvokeHandler_1;
        myFunction.Invoking += sampleInstance.PreInvokeHandler_2;
        myFunction.Invoking += SampleClass.PreInvokeHandler_3; // Static handler

        // 'Completed' event uses WeakReferences so that callers don't have to Dispose() or unregister handlers
        myFunction.Completed += sampleInstance.PostInvokeHandler_A;
        myFunction.Completed += sampleInstance.PostInvokeHandler_B;
        myFunction.Completed += SampleClass.PostInvokeHandler_C; // Static handler

        // Run the function - you should see all the pre-invoke handlers run, followed by all the post-invoke handlers
        await myFunction.RunAsync();

        Console.WriteLine();
    }

    /// <summary>
    /// Shows use of a stepwise handler implemetation, where any handler can mark a value (i.e. bool Handled = true)
    /// to stop further execution of delegates
    /// </summary>
    /// <returns></returns>
    public static void RunDemo2()
    {
        Console.WriteLine("Starting Demo 2");

        SampleClass sampleInstance = new();

        var myFunction = new MockFunctionDefinition();
        myFunction.Invoking += sampleInstance.PreInvokeHandler_1;
        myFunction.Invoking += sampleInstance.PreInvokeHandler_2;
        myFunction.Invoking += SampleClass.PreInvokeHandler_3; // Static handler

        // Run the function using the stepwise method - you should the handlers run until one of them sets Handled to
        // true, at which point the rest of the handlers for that event are skipped.
        myFunction.RunStepwise();

        Console.WriteLine();
    }

    /// <summary>
    /// Shows unregistration of handlers
    /// </summary>
    /// <returns></returns>
    public static async Task RunDemo3Async()
    {
        Console.WriteLine("Starting Demo 3");

        SampleClass sampleInstance = new();

        var myFunction = new MockFunctionDefinition();
        myFunction.Invoking += sampleInstance.PreInvokeHandler_1;
        myFunction.Invoking += sampleInstance.PreInvokeHandler_2;
        myFunction.Invoking += SampleClass.PreInvokeHandler_3; // Static handler

        // 'Completed' event uses WeakReferences so that callers don't have to Dispose() or unregister handlers
        myFunction.Completed += sampleInstance.PostInvokeHandler_A;
        myFunction.Completed += sampleInstance.PostInvokeHandler_B;
        myFunction.Completed += SampleClass.PostInvokeHandler_C; // Static handler

        // Run the function - you should see all the pre-invoke handlers run, followed by all the post-invoke handlers
        await myFunction.RunAsync();

        // Remove all but one handler of each type
        myFunction.Invoking -= sampleInstance.PreInvokeHandler_2;
        myFunction.Invoking -= SampleClass.PreInvokeHandler_3;

        myFunction.Completed -= sampleInstance.PostInvokeHandler_B;
        myFunction.Completed -= SampleClass.PostInvokeHandler_C;

        // Run the function again - you should see only the remaining pre-invoke handler (1) run, followed by
        // only the remaining post-invoke handler (A).
        await myFunction.RunAsync();

        Console.WriteLine();
    }


    /// <summary>
    /// Shows handlers going out of scope and being garbage collected
    /// </summary>
    /// <returns></returns>
    public static async Task RunDemo4Async()
    {
        Console.WriteLine("Starting Demo 4");

        var myFunction = new MockFunctionDefinition();

        // Create one class that handler for both events
        SampleClass sampleInstance1 = new("sampleInstance1");
        myFunction.Invoking += sampleInstance1.PreInvokeHandler_1;
        myFunction.Completed += sampleInstance1.PostInvokeHandler_A;

        {
            // Create a second class that handles both events
            using SampleClass sampleInstance2 = new("sampleInstance2");
            myFunction.Invoking += sampleInstance2.PreInvokeHandler_1;
            myFunction.Completed += sampleInstance2.PostInvokeHandler_A;

            // Run the function - you should see all the pre-invoke handlers run, followed by all the post-invoke handlers
            await myFunction.RunAsync();

            // Manually unregister the Invoking handler for sampleInstance2
            myFunction.Invoking -= sampleInstance2.PreInvokeHandler_1;

            // No need to unregister the Completed handler for sampleInstance2, since it's a WeakReference!
            // myFunction.Completed += sampleInstance2.PostInvokeHandler_A;
        }

        Console.WriteLine("...sampleInstance2 out of scope...");

        // Now that sampleInstance2 has gone out of scope, it should be garbage collected, and its handlers should no
        // longer be called.
        GC.Collect();
        GC.WaitForFullGCComplete();

        // Run the function again - you should see only sampleInstance1's handlers run.
        await myFunction.RunAsync();

        Console.WriteLine();
    }
}
