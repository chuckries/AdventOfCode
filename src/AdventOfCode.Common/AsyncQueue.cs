namespace AdventOfCode.Common;

public class AsyncQueue<T>
{
    public AsyncQueue()
    {
        _items = new Queue<T>();
    }

    public AsyncQueue(IEnumerable<T> items)
    {
        _items = new Queue<T>(items);
    }

    public void Enqueue(T item)
    {
        TaskCompletionSource<T> outstandingRequest = null;
        lock (_lock)
        {
            if (_outstandingRequests.Count > 0)
            {
                outstandingRequest = _outstandingRequests.Dequeue();
            }
            else
            {
                _items.Enqueue(item);
            }
        }

        outstandingRequest?.SetResult(item);
    }

    public Task<T> Dequeue(CancellationToken cancellationToken = default)
    {
        lock (_lock)
        {
            if (_items.Count > 0)
                return Task.FromResult(_items.Dequeue());
            else
            {
                TaskCompletionSource<T> request = new TaskCompletionSource<T>();
                _outstandingRequests.Enqueue(request);
                return request.Task;
            }
        }
    }

    Queue<T> _items;
    Queue<TaskCompletionSource<T>> _outstandingRequests = new Queue<TaskCompletionSource<T>>();
    object _lock = new object();
}
