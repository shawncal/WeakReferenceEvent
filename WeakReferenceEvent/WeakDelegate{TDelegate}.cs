using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace WeakReferenceEvent;

public class WeakDelegate<TDelegate>
    where TDelegate : Delegate
{
    // A weak reference to the target object instance (non-static delegtes only)
    private readonly WeakReference<object>? _targetWeakRef;

    // A weak reference to the method
    private readonly WeakReference<MethodInfo>? _methodWeakRef;

    // A strong reference to the delegate (for static delegates only)
    private readonly TDelegate? _strongDelegate;

    public object? Target => _targetWeakRef?.TryGetTarget(out var target) == true ? target : null;

    public MethodInfo? Method => this._strongDelegate?.Method ??
        (_methodWeakRef?.TryGetTarget(out var method) == true ? method : null);

    public WeakDelegate(TDelegate d)
    {
        if (d is null)
        {
            throw new ArgumentNullException(nameof(d));
        }

        if (d.Target != null)
        {
            // If the target is not null, create a weak reference to it
            // Note: if the target is null, the delegate is static, and we don't need a target instance.
            this._targetWeakRef = new WeakReference<object>(d.Target);
            this._methodWeakRef = new WeakReference<MethodInfo>(d.Method);
        }
        else
        {
            this._strongDelegate = d;
        }
    }

    public bool TryGetDelegate(out TDelegate? strongDelegate)
    {
        if (this._strongDelegate is not null)
        {
            // Static callback -- just return the delegate
            strongDelegate = this._strongDelegate;
            return true;
        }

        var targetObject = this.Target;
        var method = this.Method;
        if(targetObject != null && method != null)
        {
            strongDelegate = (TDelegate)Delegate.CreateDelegate(typeof(TDelegate), targetObject, method);
            return true;
        }
        
        strongDelegate = null;
        return false;
    }

    public override bool Equals(object? obj)
    {
        // If the other object is not a WeakDelegate<T>, return false
        if (obj is not WeakDelegate<TDelegate> other) { return false; }

        // Compare the target and method references
        return ReferenceEquals(this._targetWeakRef, other?._targetWeakRef)
            && this._methodWeakRef?.Equals(other?._methodWeakRef) == true
            && this._strongDelegate?.Equals(other?._methodWeakRef) == true;
    }

    public override int GetHashCode()
    {
        return this._strongDelegate?.GetHashCode() ?? 0;
            //this._targetWeakRef is null ? this.Method?.GetHashCode()
            //: this._targetWeakRef.GetHashCode() ^ this.Method?.GetHashCode();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator ==(WeakDelegate<TDelegate>? d1, WeakDelegate<TDelegate>? d2)
    {
        if (d2 is null)
        {
            return d1 is null;
        }

        return ReferenceEquals(d2.Target, d1?.Target) || d2.Equals((object?)d1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool operator !=(WeakDelegate<TDelegate>? d1, WeakDelegate<TDelegate>? d2)
    {
        if (d2 is null)
        {
            return d1 is not null;
        }

        return !ReferenceEquals(d2, d1) && !d2.Equals(d1);
    }
}