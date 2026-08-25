using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace TALib
{

    public struct Slicer<T> : IList<T>
    {
        public Slicer(IList<T> lbase, int start, int end)
        {
            _validateWindow(lbase, start, end);
            _lbase = lbase;
            _start = start;
            _end = end;
        }
        public int Count { get { return _end - _start + 1; } }
        public bool IsReadOnly { get { return false; } }
        public T this[int index]
        {
            get
            {
                var nindex = _start + index;
                if (nindex < _start || nindex > _end) { throw new Exception(string.Format("Index ({0}) out of range ", index)); }
                return _lbase[nindex];
            }
            set
            {
                var nindex = _start + index;
                if (nindex < _start || nindex > _end) { throw new Exception(string.Format("Index ({0}) out of range ", index)); }
                _lbase[nindex] = value;
            }
        }
        public int IndexOf(T item)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (item.Equals(this[i])) { return i; }
            }
            return -1;
        }
        public void Insert(int index, T item)
        {
            throw new Exception("Insert not allowed in a Slice");
        }
        public void RemoveAt(int index)
        {
            throw new Exception("Remove not allowed in a Slice");
        }
        public void Add(T item)
        {
            throw new Exception("Add not allowed in a Slice");
        }
        public void Clear()
        {
            throw new Exception("Clear not allowed in a Slice");
        }
        public bool Contains(T item)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (item.Equals(this[i])) { return true; }
            }
            return false;
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int i = 0; i < this.Count; i++)
            {
                array[arrayIndex] = this[i];
                arrayIndex++;
            }
        }
        public bool Remove(T item)
        {
            throw new Exception("Remove not allowed in a Slice");
        }
        public IEnumerator<T> GetEnumerator()
        {
            return new SlicerEnumerator<T>(this);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #region private
        private void _validateWindow(IList<T> lbase, int start, int end)
        {
            if (start >= lbase.Count) { throw new Exception(string.Format("Start ({0}) of slice is past end ({1}) of base list", start, lbase.Count)); }
            if (end < start) { throw new Exception(string.Format("Start ({0}) of slice is less than end ({1}) of slice", start, end)); }
            if (end >= lbase.Count) { throw new Exception(string.Format("End ({0}) of slice is past end end ({1}) of base list", end, lbase.Count)); }
        }

        private IList<T> _lbase;
        private int _start;
        private int _end;
        #endregion
    }
    public struct SlicerEnumerator<T> : IEnumerator<T>
    {
        public SlicerEnumerator(IList<T> collection)
        {
            _collection = collection;
            curIndex = -1;
            curT = default(T);
        }
        public bool MoveNext()
        {
            //Avoids going beyond the end of the collection.
            if (++curIndex >= _collection.Count)
            {
                return false;
            }
            else
            {
                // Set current box to next item in collection.
                curT = _collection[curIndex];
            }
            return true;
        }
        public void Reset() { curIndex = -1; }
        void IDisposable.Dispose() { }
        public T Current
        {
            get { return curT; }
        }
        object IEnumerator.Current
        {
            get { return Current; }
        }
        #region private
        private IList<T> _collection;
        private int curIndex;
        private T curT;
        #endregion
    }

    static public class ListPool<T>
    {
        static public ListPoolItem<T> Get()
        {
            ListPoolItem<T> ret;
            Interlocked.Add(ref _totalAllocated, 1);

            if (_pool.TryTake(out ret))
            {
                Interlocked.Add(ref _totalBuffered, 1);
                ret.allocationCount++;
                ret.released = false;
                return ret;
            }
 
            ret = new ListPoolItem<T>();
            ret.allocationCount = 1;
            ret.released = false;
            return ret;
        }
        static public AllocItem[] GetAllocationList()
        {
            var pool = _pool.ToArray();
            var ret = new AllocItem[pool.Length];

            for (int i = 0; i < pool.Length; i++)
            {
                ret[i] = new AllocItem(i, pool[i].allocationCount, pool[i].Capacity);
            }
            return ret;
        }
        static public int TotalAllocated { get { return _totalAllocated; } }
        static public int TotalBuffered { get { return _totalBuffered; } }
        static public int CurrentBuffer {  get { return _pool.Count; } }   
        static public int CurrentAllocated { get { return _totalAllocated - _totalBuffered - _pool.Count; } }  
        static internal void Return(ListPoolItem<T> list)
        {
            if (list.released) { return; }
            list.Clear();
            list.released = true;
            _pool.Add(list);
        }
        #region private
        static private ConcurrentBag<ListPoolItem<T>> _pool = new ConcurrentBag<ListPoolItem<T>>();
        static private int _totalAllocated = 0;
        static private int _totalBuffered = 0;
        #endregion
    }
    public class ListPoolItem<T> : List<T>, IDisposable
    {
        internal ListPoolItem() 
        {
            released = false;
            allocationCount = 0;
        }
        public void Dispose()
        {
            ListPool<T>.Return(this);
        }
        #region internal
        // Only updated/referenced by the ListPool<T>
        internal bool released;
        internal int allocationCount;
        #endregion
    }

    static public class ListExtention
    {
        static public IList<T> Slice<T>(this IList<T> lbase, int start, int len)
        {
            var ret = new Slicer<T>(lbase, start, start + len - 1);
            return (IList<T>)ret;
        }
        static public IList<T> Slice<T>(this IList<T> lbase, int start)
        {
            var ret = new Slicer<T>(lbase, start, lbase.Count - 1);
            return (IList<T>)ret;
        }
        static public IList<T> AdjustLength<T>(this IList<T> list, int length)
        {
            if (length < 0) { length = 0; }
            if (list.Count == length) { return list; }
            if (list.Count < length) { while (list.Count < length) { list.Add(default(T)); } return list; }
            if (list.Count > length) { while (list.Count < length) { list.RemoveAt(length); } return list; }
            return list;
        }
        static public void Dispose<T>(this IList<T> list)
        {
            var disp = list as IDisposable;
            if (disp != null) 
            { 
                disp.Dispose();
            }

        }
    }

    public class AllocItem
    {
        internal AllocItem(int id, int allocationCount, int size)
        {
            _id = id;
            _allocationCount = allocationCount;
            _size = size;
        }
        public int Id { get { return _id; } }
        public int AllocationCount { get { return _allocationCount; } }
        public int Size { get { return _size; } }
        #region private
        private int _id;
        private int _allocationCount;
        private int _size;
        #endregion
    }
}
