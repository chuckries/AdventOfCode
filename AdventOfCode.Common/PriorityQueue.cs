using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.Common
{
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
            _heap = new T[1];
            _size = 0;
        }

        public void Enqueue(T item)
        {
            Add(item);
            UpHeap(_size - 1);
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
            T temp = _heap[a];
            _heap[a] = _heap[b];
            _heap[b] = temp;
        }

        private IComparer<T> _comparer;
        private T[] _heap;
        private int _size;
    }
}
