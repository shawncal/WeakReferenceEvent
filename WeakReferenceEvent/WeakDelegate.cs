using System;

namespace WeakReferenceEvent;

/// <summary>
/// A class that represents a weak delegate, i.e. a delegate that does not prevent the target 
/// object from being garbage collected
/// </summary>
public class WeakDelegate : WeakDelegate<Delegate>
{
    // The constructor that takes a delegate as an argument and extracts the target and the method
    public WeakDelegate(Delegate d)
        : base(d)
    {
    }

    // A method that invokes the delegate if the target object is still alive, or returns null otherwise
    public object? DynamicInvoke(object[] args)
    {
        // Try to get the target object
        object? targetObject = this.Target;

        // If the target object is alive, invoke the method on it
        if (targetObject != null)
        {
            return this.Method?.Invoke(targetObject, args);
        }

        // Otherwise, return null
        return null;
    }
}
