using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace TALib
{
    public sealed class Levels<T> where T : struct
    {
        public Levels(double delta)
        {
            if (!double.IsFinite(delta) || delta <= 0.0) { throw new ArgumentOutOfRangeException(nameof(delta), "Delta must be a finite number greater than zero."); }
            _delta = delta;
        }
        public void Reset()
        {
            _data.Clear();
            _indices.Clear();
        }
        public double Set(double value, T data)
        {
            long index = ToIndex(value);
            _data[index] = data;
            _indices.Add(index);
            return ToMidPoint(index);
        }
        public bool Get(double value, out T data)
        {
            return _data.TryGetValue(ToIndex(value), out data);
        }
        public bool GetNextHigher(double value, out double midPoint, out T data) { return _tryGetNeighbor(ToIndex(value), higher: true, out midPoint, out data); }
        public bool GetNextLower(double value, out double midPoint, out T data) { return _tryGetNeighbor(ToIndex(value), higher: false, out midPoint, out data); }
        public double LowestMidPoint()
        {
            if (_indices.Count == 0) { throw new InvalidOperationException("No levels have been set."); }
            return ToMidPoint(_indices.Min);
        }
        public double HighestMidPoint()
        {
            if (_indices.Count == 0) { throw new InvalidOperationException("No levels have been set."); }
            return ToMidPoint(_indices.Max);
        }
        #region private
        private bool _tryGetNeighbor(long index, bool higher, out double midPoint, out T data)
        {
            midPoint = default;
            data = default;

            if (_indices.Count == 0) { return false; }

            if (higher)
            {
                if (index == long.MaxValue)
                    return false;

                var view = _indices.GetViewBetween(index + 1, long.MaxValue);
                using var e = view.GetEnumerator();
                if (!e.MoveNext())
                    return false;

                return _bind(e.Current, out midPoint, out data);
            }

            if (index == long.MinValue) {  return false; }

            var lower = _indices.GetViewBetween(long.MinValue, index - 1);
            // Reverse enumerator starts at Max — first MoveNext is O(log n).
            foreach (long prev in lower.Reverse()) { return _bind(prev, out midPoint, out data); }

            return false;
        }

        private bool _bind(long index, out double midPoint, out T data)
        {
            midPoint = ToMidPoint(index);
            data = _data[index];
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double ToMidPoint(long index) => (index + 0.5) * _delta;

        private long ToIndex(double value)
        {
            if (!double.IsFinite(value))
                throw new ArgumentOutOfRangeException(nameof(value), "Value must be finite.");
            if (value < 0.0)
                throw new ArgumentOutOfRangeException(nameof(value), "Levels start at 0 and go up.");

            double scaled = value / _delta;
            if (scaled >= _maxExactInteger)
                throw new OverflowException("value/delta exceeds the exact-integer range of double (2^53).");

            return _floorSnapToNextInteger(scaled);
        }

        /// <summary>
        /// Floor(scaled), except when scaled is a few ULPs below the next integer.
        /// Fixes 0.3/0.1 == 2.999… mapping into the wrong bin, without moving
        /// interior points.
        /// </summary>
        private static long _floorSnapToNextInteger(double scaled)
        {
            long floor = (long)Math.Floor(scaled);
            double next = floor + 1.0;
            if (scaled < next && _withinUlps(scaled, next, _boundaryUlps))
                return floor + 1;
            return floor;
        }

        private static bool _withinUlps(double a, double b, int maxUlps)
        {
            if (a == b)
                return true;
            if (!double.IsFinite(a) || !double.IsFinite(b))
                return false;

            long ia = BitConverter.DoubleToInt64Bits(a);
            long ib = BitConverter.DoubleToInt64Bits(b);

            // Map two's-complement so lexicographic order matches numeric order.
            if (ia < 0) ia = long.MinValue - ia;
            if (ib < 0) ib = long.MinValue - ib;

            long diff = ia - ib;
            if (diff < 0) diff = -diff;
            return diff <= maxUlps;
        }
        private const double _maxExactInteger = 1L << 53; // 2^53
        private const int _boundaryUlps = 16;
        private readonly double _delta;
        private readonly Dictionary<long, T> _data = new();
        private readonly SortedSet<long> _indices = new();
        #endregion
    }
}
