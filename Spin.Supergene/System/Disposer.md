# Disposer

The disposer class helps to create objects that encapsulate unmanaged resources.

# Features
1. Ensure that Dispose logic is only called once.
2. Provide utility methods to assert that the object is not disposed when being used
3. Provide a common interface for disposing managed vs. unmanaged resources

# How to Use 

Inherit this class when:

1. Your class has fields that implement `IDisposable`
2. When your class has fields that represent unmanaged resources

## Managed Resources

```csharp
public class Foo : Disposable
{
    private HttpClient _client;

    //Throw an exception if the object has been disposed
    public HttpClient => Disposer.Assert(_client);
    public string Fetch() => Disposer.Assert(() => _client.Get("www.contoso.com"));

    protected override DisposeManaged => _client?.Dispose();
}
```


## Unmanaged Resources

```csharp
public class Foo : Disposable
{
    private IntPtr _handle;

    //Backstop to clear unmanaged resources, in case developer neglects to call Dispose
    ~Foo() => Dispose(false);

    public void Foo()
    {
        //Ensure we haven't been disposed
        Disposer.Assert();
        Bar(_handle);
    }

    protected override DisposeUnmanaged => Free(_handle);
}
```
