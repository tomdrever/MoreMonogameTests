using System;

namespace Pathfinding
{
    public class Heap<T> where T : IHeapItem<T>
    {
        private readonly T[] _items;
        private int _currentItemCount;

        public int Count => _currentItemCount;

        public Heap(int maxHeapSize)
        {
            _items = new T[maxHeapSize];
        }

        public void Add(T item)
        {
            item.HeapIndex = _currentItemCount;
            _items[_currentItemCount] = item;
            SortUp(item);
            _currentItemCount++;
        }

        public T RemoveFirst()
        {
            var firstItem = _items[0];
            _currentItemCount--;

            _items[0] = _items[_currentItemCount];
            _items[0].HeapIndex = 0;

            SortDown(_items[0]);

            return firstItem;
        }

        public bool Contains(T item)
        {
            return Equals(_items[item.HeapIndex], item);
        }

        public void Update(T item)
        {
            SortUp(item);
        }

        private void SortDown(T item)
        {
            while (true)
            {
                int childLeftIndex = item.HeapIndex * 2 + 1;
                int childRightIndex = item.HeapIndex * 2 + 2;

                if (childLeftIndex < _currentItemCount)
                {
                    int swapIndex = childLeftIndex;

                    if (childRightIndex < _currentItemCount)
                    {
                        if (_items[childLeftIndex].CompareTo(_items[childRightIndex]) < 0)
                        {
                            swapIndex = childRightIndex;
                        }
                    }

                    if (item.CompareTo(_items[swapIndex]) < 0)
                    {
                        Swap(item, _items[swapIndex]);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }

        private void SortUp(T item)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;

            while (true)
            {
                var parentItem = _items[parentIndex];

                if (item.CompareTo(parentItem) > 0)
                {
                    Swap(item, parentItem);
                }
                else
                {
                    break;
                }

                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }

        public void Swap(T itemA, T itemB)
        {
            _items[itemA.HeapIndex] = itemB;
            _items[itemB.HeapIndex] = itemA;

            int itemAIndex = itemA.HeapIndex;
            itemA.HeapIndex = itemB.HeapIndex;
            itemB.HeapIndex = itemAIndex;
        }
    }

    public interface IHeapItem<T> : IComparable<T>
    {
        int HeapIndex { get; set; }
    }
}