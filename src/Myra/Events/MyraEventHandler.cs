namespace Myra.Events
{
    public delegate void MyraEventHandler(object? sender, MyraEventArgs e);

    public delegate void MyraEventHandler<T>(object? sender, T e) where T : MyraEventArgs;
}
