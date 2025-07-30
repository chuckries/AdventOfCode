namespace AdventOfCode.Common;

[Obsolete(".NET has a better PriorityQueue built in now. Keeping this for posterity.")]
public class PriorityQueue<T>
{
    public int Count => _size;

    public PriorityQueue()
        : this(Comparer<T>.Default)
    {
    }

    public PriorityQueue(IComparer<T> comparer)
    {
        _comparer = comparer;
        _heap = new T[4];
        _size = 0;
    }

    public PriorityQueue(IEnumerable<T> collection)
        : this(collection, Comparer<T>.Default)
    {
    }

    public PriorityQueue(IEnumerable<T> collection, IComparer<T> comparer)
        : this(comparer)
    {
        foreach (var i in collection)
        {
            Enqueue(i);
        }
    }

    public void Enqueue(T item)
    {
        Add(item);
        UpHeap(_size - 1);
    }

    public void EnqueueRange(IEnumerable<T> collection)
    {
        foreach (var item in collection)
            Enqueue(item);
    }

    public T Dequeue()
    {
        if (_size == 0)
            throw new InvalidOperationException();

        T toReturn = _heap[0];
        _heap[0] = _heap[_size - 1];
        _size--;
        DownHeap(0);
        return toReturn;
    }

    private void Add(T item)
    {
        if (_size == _heap.Length)
        {
            T[] newHeap = new T[_size * 2];
            Array.Copy(_heap, 0, newHeap, 0, _size);
            _heap = newHeap;
        }
        _heap[_size] = item;
        _size++;
    }

    private void UpHeap(int index)
    {
        if (index > 0)
        {
            int parentIndex = (index - 1) / 2;
            if (_comparer.Compare(_heap[index], _heap[parentIndex]) < 0)
            {
                Swap(index, parentIndex);
                UpHeap(parentIndex);
            }
        }
    }

    private void DownHeap(int index)
    {
        int leftIndex = index * 2 + 1;
        int rightIndex = index * 2 + 2;

        int minIndex = index;

        if (leftIndex < _size && _comparer.Compare(_heap[leftIndex], _heap[minIndex]) < 0)
            minIndex = leftIndex;

        if (rightIndex < _size && _comparer.Compare(_heap[rightIndex], _heap[minIndex]) < 0)
            minIndex = rightIndex;

        if (minIndex != index)
        {
            Swap(minIndex, index);
            DownHeap(minIndex);
        }
    }

    private void Swap(int a, int b)
    {
        (_heap[a], _heap[b]) = (_heap[b], _heap[a]);
    }

    private IComparer<T> _comparer;
    private T[] _heap;
    private int _size;
}
