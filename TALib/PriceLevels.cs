using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace TALib
{
    public class PriceLevels
    {
        public PriceLevels(double delta, int turningPointCount)
        {
            _levels = new Levels<LevelData>(delta);
            if (turningPointCount < 1) { throw new ArgumentOutOfRangeException(nameof(turningPointCount), "Turning point count must be at least 1."); }
            _lastHighs = new double[turningPointCount * 2 + 1];
            _lastLows = new double[turningPointCount * 2 + 1];
        }
        public void LogBar(DateTime dt, double open, double high, double low, double close, double volume)
        {
            _update(high, _lastHighs);
            _update(low, _lastLows);
            if (_lastHighs[0] != 0.0)
            {
                if (_isMaximum(_lastHighs))
                {
                    var max = _lastHighs[_lastHighs.Length / 2];
                    if (!_levels.Get(max, out var levelData))
                    {
                        levelData.Count = 1;
                    }
                    else
                    {
                        levelData.Count++;
                    }
                    levelData.DateLastBounce = dt;
                    _levels.Set(max, levelData);
                }
                if (_isMinimum(_lastLows))
                {
                    var min = _lastLows[_lastLows.Length / 2];
                    if (!_levels.Get(min, out var levelData))
                    {
                        levelData.Count = 1;
                    }
                    else
                    {
                        levelData.Count++;
                    }
                    levelData.DateLastBounce = dt;
                    _levels.Set(min, levelData);
                }
            }
        }
        public bool GetSupport(DateTime dateTime, double value, int daysLastBounce, out double strength, out double support)
        {
            support = 0.0;
            strength = 0.0;
            while (_levels.GetNextLower(value, out support, out var supportData))
            {
                if (dateTime.Subtract(supportData.DateLastBounce).Days <= daysLastBounce)
                {
                    strength = supportData.Count;
                    return true;
                }
                value = support;
                support = 0.0;
                strength = 0.0;
            }
            return false;
        }
        public bool GetResistance(DateTime dateTime, double value, int daysLastBounce, out double strength, out double resistance)
        {
            resistance = 0.0;
            strength = 0.0;
            while (_levels.GetNextHigher(value, out resistance, out var resistanceData))
            {
                if (dateTime.Subtract(resistanceData.DateLastBounce).Days <= daysLastBounce)
                {
                    strength = resistanceData.Count;
                    return true;
                }
                value = resistance;
                resistance = 0.0;
                strength = 0.0;
            }
            return false;
        }
        #region private
        private Levels<LevelData> _levels;
        private double[] _lastHighs;
        private double[] _lastLows;
        private void _update(double value, double[] points)
        {
            for (int i = 0; i < points.Length - 1; i++)
            {
                points[i] = points[i + 1];
            }
            points[points.Length - 1] = value;
        }
        private bool _isMaximum(double[] points)
        {
            int mid = (points.Length - 1) / 2;
            for (int i = 0; i < mid; i++) 
            { 
                if (points[i] >= points[i + 1]) { return false; } 
                if (points[mid + i] <= points[mid + i + 1]) { return false; }
            }
            return true;
        }
        private bool _isMinimum(double[] points)
        {
            int mid = (points.Length - 1) / 2;
            for (int i = 0; i < mid; i++)
            {
                if (points[i] <= points[i + 1]) { return false; }
                if (points[mid + i] >= points[mid + i + 1]) { return false; }
            }
            return true;
        }
        #endregion
    }
    public struct LevelData
    {
        public LevelData()
        {
            Count = 0;
            DateLastBounce = DateTime.MinValue;
        }
        public int Count;
        public DateTime DateLastBounce;
    }
}