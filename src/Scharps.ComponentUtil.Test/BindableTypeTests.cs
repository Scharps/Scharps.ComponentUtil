using Xunit;
using FluentAssertions;
using System.Threading.Tasks;
using System;

namespace Scharps.ComponentUtil.Test;

public class BindableTypeTests
{
    [Fact]
    public async Task Mutate_Changes_Value()
    {
        BindableType<int> v = new(10);
        await v.Mutate(20);
        int value = v;
        value.Should().Be(20);
    }

    [Fact]
    public async Task Callback_Gets_Invoked()
    {
        BindableType<int> v = new(10);
        bool handlerCalled = false;
        Func<int, Task> handler = (newValue) =>
        {
            handlerCalled = true;
            return Task.CompletedTask;
        };
        v += handler;
        await v.Mutate(20);

        handlerCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Callback_Gets_Removed()
    {
        BindableType<int> v = new(10);
        bool handlerCalled = false;
        Func<int, Task> handler = (newValue) =>
        {
            handlerCalled = true;
            return Task.CompletedTask;
        };
        v += handler;
        v -= handler;
        await v.Mutate(20);

        handlerCalled.Should().BeFalse();
    }

    [Fact]
    public void BindableType_Defaults_To_Types_Default()
    {
        BindableType<int> v = new();
        int typeValue = v;
        typeValue.Should().Be(default(int));
    }

    [Fact]
    public void Uninitialized_BindableType_Is_Default_Of_Value_Type()
    {
        var o = new ObjectContainingBindableType<int>();
        int typeValue = o.value;
        typeValue.Should().Be(default(int));
    }

    [Fact]
    public void Uninitialized_BindableType_Is_Default_Of_Reference_Type()
    {
        var o = new ObjectContainingBindableType<string>();
        string? typeValue = o.value;
        typeValue.Should().Be(default(string));
    }

    private class ObjectContainingBindableType<T>
    {
        public BindableType<T> value = new();
    }

    [Fact]
    public async Task Assigning_Same_Value_Does_Not_Invoke_Callbacks()
    {
        var handlerCalled = false;
        BindableType<ConsoleKey> consoleKey = new(ConsoleKey.A);
        Func<ConsoleKey, Task> handler = key =>
        {
            handlerCalled = true;
            return Task.CompletedTask;
        };
        consoleKey += handler;
        await consoleKey.Mutate(ConsoleKey.A);

        handlerCalled.Should().BeFalse();
    }
}