namespace Koba.Contracts.Interfaces;

public interface IMessagingService<T>
{
    public void Subscribe(string topicName, Func<T, Task> callback);
    public Task PublishAsync(string topicName, T data, CancellationToken token);
}