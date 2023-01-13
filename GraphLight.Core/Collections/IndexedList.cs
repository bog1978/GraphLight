using System;
using System.Collections;
using System.Collections.Generic;

namespace GraphLight.Collections
{
    internal class IndexedList<T> : ICollection<T>, ICollection
        where T : IIndexedItem
    {
        #region Константы и поля

        private const int _defaultCapacity = 4;
        private const int _halfMaxLength = (int.MaxValue - 1024 * 1024) / 2;

        private static readonly T[] EmptyArray = new T[0];

        private int _size;
        private int _version;
        private T[] _items;

        #endregion

        #region Конструкторы

        public IndexedList()
        {
            _items = EmptyArray;
            SyncRoot = new object();
        }

        public IndexedList(int capacity) : this()
        {
            EnsureCapacity(capacity);
        }

        #endregion

        #region ICollection

        public void CopyTo(Array array, int arrayIndex) =>
            Array.Copy(_items, 0, array, arrayIndex, _size);

        public bool IsSynchronized => false;

        public object SyncRoot { get; }

        #endregion

        #region ICollection<T>

        public void Add(T item)
        {
            if (item.Index >= 0)
                throw new InvalidOperationException("Item already indexed.");
            if (_size == _items.Length)
                EnsureCapacity(_size + 1);
            item.Index = _size;
            _items[_size++] = item;
            _version++;
        }

        public void Clear()
        {
            if (_size > 0)
            {
                Array.Clear(_items, 0, _size);
                _size = 0;
            }
            _version++;
        }

        public bool Contains(T item)
        {
            if (item.Index < 0 || item.Index >= _size)
                return false;
            var existing = _items[item.Index];
            return Equals(existing, item);
        }

        public void CopyTo(T[] array, int arrayIndex) =>
            Array.Copy(_items, 0, array, arrayIndex, _size);

        public bool Remove(T item)
        {
            var index = item.Index;
            if (index >= 0 && index < _size)
            {
                var existing = _items[index];
                if (Equals(existing, item))
                {
                    var last = _items[_size - 1];
                    _items[index] = last;
                    last.Index = index;
                    _size--;
                    _version++;
                    item.Index = -1;
                    return true;
                }
            }
            return false;
        }

        public int Count => _size;

        public bool IsReadOnly => false;

        #endregion

        #region IEnumerable

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region IEnumerable<T>

        public IEnumerator<T> GetEnumerator() => new Enumerator(this);

        #endregion

        #region Другое

        private void EnsureCapacity(int min)
        {
            if (_items.Length >= min)
                return;

            var newCapacity = _items.Length == 0
                ? _defaultCapacity
                : _items.Length < _halfMaxLength
                    ? _items.Length * 2
                    : _halfMaxLength * 2;

            if (newCapacity < min)
                newCapacity = min;

            SetCapacity(newCapacity);
        }

        private void SetCapacity(int newCapacity)
        {
            if (newCapacity < _size)
                throw new ArgumentOutOfRangeException(nameof(newCapacity), newCapacity, "New capacity must be greater then current size.");

            if (newCapacity == _items.Length)
                return;

            if (newCapacity > 0)
            {
                var newItems = new T[newCapacity];
                if (_size > 0)
                    Array.Copy(_items, newItems, _size);
                _items = newItems;
            }
            else
                _items = EmptyArray;
        }

        #endregion

        private class Enumerator : IEnumerator<T>
        {
            #region Константы и поля

            private readonly IndexedList<T> _list;
            private readonly int _version;
            private int _index;
            private T _current;

            #endregion

            #region Конструкторы

            public Enumerator(IndexedList<T> list)
            {
                _list = list;
                _version = list._version;
                _current = default!;
            }

            #endregion

            #region IDisposable

            public void Dispose()
            {
            }

            #endregion

            #region IEnumerator

            public bool MoveNext()
            {
                if (_version == _list._version && _index < _list._size)
                {
                    _current = _list._items[_index];
                    _index++;
                    return true;
                }
                return MoveNextRare();
            }

            public void Reset()
            {
                if (_version != _list._version)
                    throw new InvalidOperationException("ExceptionResource.InvalidOperation_EnumFailedVersion");
                _index = 0;
                _current = default;
            }

            object IEnumerator.Current => Current;

            #endregion

            #region IEnumerator<T>

            public T Current
            {
                get
                {
                    if (_index == 0 || _index == _list._size + 1)
                        throw new InvalidOperationException("ExceptionResource.InvalidOperation_EnumOpCantHappen");
                    return _current;
                }
            }

            #endregion

            #region Другое

            private bool MoveNextRare()
            {
                if (_version != _list._version)
                    throw new InvalidOperationException("ExceptionResource.InvalidOperation_EnumFailedVersion");
                _index = _list._size + 1;
                _current = default;
                return false;
            }

            #endregion
        }
    }

    internal interface IIndexedItem
    {
        #region События, свойства, индексаторы

        int Index { get; set; }

        #endregion
    }
}