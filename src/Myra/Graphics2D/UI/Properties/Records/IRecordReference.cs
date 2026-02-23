using System;
using Myra.Utility.Types;

namespace Myra.Graphics2D.UI.Properties
{
    public interface IRecordReference
    {
        Record Record { get; }
        Type Type { get; }
        bool IsReadOnly { get; }
        object GetValue(object field);
        void SetValue(object field, object value);
    }

    public interface IRecordReference<T> : IRecordReference
    {
        new T GetValue(object field);
        void SetValue(object field, T value);
    }
    
    public interface IStructRecordReference<T> : IRecordReference<T> where T : struct
    {
        bool IsNullable { get; }
    }

    public interface INumberRecordReference<T> : IStructRecordReference<T> where T : struct
    {
        // todo include min/max T limiters
    }
}