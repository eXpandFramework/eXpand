using System;

namespace TypeMock.Extensions
{
    public class TrackInstances<T>
    {
        public int Count { get; internal set; }
        public T LastInstance { get; internal set; }
        public Action<T> InitializeAction { get; set; }
    }
}