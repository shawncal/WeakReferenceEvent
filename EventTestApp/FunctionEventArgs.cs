namespace EventTestApp;

public class FunctionEventArgs : EventArgs
{
    public FunctionEventArgs(string value)
    {
        this.Value = value;
    }

    public string Value { get; set; }

    public bool Handled { get; set; } = false;
}
