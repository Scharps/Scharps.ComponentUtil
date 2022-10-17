namespace Scharps.ComponentUtil;
public struct BindableType<T>
{
    private List<Func<T, Task>> _callbacks = new();
    private T? _value;

    public BindableType(T value)
    {
        _value = value;
    }

    public BindableType()
    {
        _value = default;
    }

    public void AddCallback(Func<T, Task> callback)
    {
        _callbacks.Add(callback);
    }

    public void RemoveCallback(Func<T, Task> callback)
    {
        _callbacks.Remove(callback);
    }

    public Task Mutate(T newValue)
    {
        if (newValue is null) throw new ArgumentException(nameof(newValue));

        if (newValue.Equals(_value)) return Task.CompletedTask;
        _value = newValue;
        return Task.WhenAll(_callbacks.Select(c => c.Invoke(newValue)));
    }

    public static BindableType<T> operator +(BindableType<T> vcc, Func<T, Task> callback)
    {
        vcc.AddCallback(callback);
        return vcc;
    }

    public static BindableType<T> operator -(BindableType<T> vcc, Func<T, Task> callback)
    {
        vcc.RemoveCallback(callback);
        return vcc;
    }

    public static implicit operator T?(BindableType<T> vcc)
    {
        return vcc._value;
    }
}