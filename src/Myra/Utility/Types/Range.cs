using System;

namespace Myra.Utility.Types
{
    /// <summary>
    /// Represents a generic value range with optional minimum and maximum clamp.
    /// </summary>
    public struct Range<TNum> where TNum : struct
    {
        static Range()
        {
            Type arg = typeof(TNum);
            TypeInfo info = TypeHelper<TNum>.Info;
            
            if(info.IsNullable)
                throw new ArgumentException($"Invalid Generic-Type Argument: '{arg}', Nullable types are unsupported");
            if(!info.IsNumber)
                throw new ArgumentException($"Invalid Generic-Type Argument: '{arg}', Only numeric types are supported");
        }
        
        public Range(TNum min, TNum max)
        {
            _min = min;
            _max = max;
        }
        public Range(TNum? min = null, TNum? max = null)
        {
            _min = min;
            _max = max;
        }
        
        private TNum? _min, _max;

        public TNum? Min
        {
            get => _min;
            set => _min = value;
        }
        public TNum? Max
        {
            get => _max;
            set => _max = value;
        }

        public bool InsideMinBound(TNum value)
        {
            if (_min.HasValue && MathHelper<TNum>.LessThan(value, _min.Value))
                return false;
            return true;
        }
        public bool InsideMaxBound(TNum value)
        {
            if (_max.HasValue && MathHelper<TNum>.GreaterThan(value, _max.Value))
                return false;
            return true;
        }
        
        public bool IsInRange(TNum value)
        {
            if (_min.HasValue && MathHelper<TNum>.LessThan(value, _min.Value))
                return false;
            if (_max.HasValue && MathHelper<TNum>.GreaterThan(value, _max.Value))
                return false;
            return true;
        }
        /// <inheritdoc cref="MathHelper{TNum}.Clamp(TNum, TNum?, TNum?)"/>
        public TNum Clamp(TNum value) => MathHelper<TNum>.Clamp(value, _min, _max);
    }
}