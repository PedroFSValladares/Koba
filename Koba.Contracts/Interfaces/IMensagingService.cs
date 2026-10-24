namespace Koba.Contracts.Interfaces;

public interface IMensagingService<T>
{
    public void Subscribe(Func<T, Task> callback, string topicName);
    public void Publish(T data, string topicName);
}