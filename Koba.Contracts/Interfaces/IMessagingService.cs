namespace Koba.Contracts.Interfaces;

public interface IMessagingService<T>
{
    public void Subscribe(string topicName, Func<T, Task> callback);
    public void Publish(string topicName, T data);
}