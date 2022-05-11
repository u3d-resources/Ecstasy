using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Collections;
using UnityEngine;

namespace AV.ECS.Examples 
{
    public interface ISmallBuffer
    {
        ushort Length { get; }
    }
    public interface ISmallBuffer<T> : ISmallBuffer
    {
        ref T GetElement(int index);
        void SetElement(int index, T value);
    }

    internal static unsafe class SmallBufferExtensions
    {
        [BurstDiscard]
        internal static void RequireNotFull(this ISmallBuffer buffer)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (buffer.Length == 10)
                throw new InvalidOperationException("Buffer overflow");
#endif
        }

        [BurstDiscard]
        internal static void RequireIndexInBounds(this ISmallBuffer buffer, int index)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (index < 0 || index >= buffer.Length)
                throw new InvalidOperationException("Index out of bounds: " + index);
#endif
        }

        [BurstDiscard]
        private static void RequirePositiveCount(int count)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", "Count must be positive: " + count);
#endif
        }

        internal static void Clear<T>(this ISmallBuffer<T> buffer)
        {
            for (int i = 0; i < buffer.Length; ++i)
                buffer.SetElement(i, default);
        }

        internal static void Insert<T>(this ISmallBuffer<T> buffer, int index, T value)
        {
            RequireNotFull(buffer);
            RequireIndexInBounds(buffer, index);

            for (int i = buffer.Length; i > index; --i)
                buffer.SetElement(i, buffer.GetElement(i - 1));

            buffer.SetElement(index, value);
        }

        internal static void RemoveAt<T>(this ISmallBuffer<T> buffer, int index)
        {
            RequireIndexInBounds(buffer, index);

            for (int i = index; i < buffer.Length - 1; ++i)
                buffer.SetElement(i, buffer.GetElement(i + 1));
        }

        internal static void RemoveRange<T>(this ISmallBuffer<T> buffer, int index, int count)
        {
            RequireIndexInBounds(buffer, index);

            RequirePositiveCount(count);

            RequireIndexInBounds(buffer, index + count - 1);
            int indexAfter = index + count;
            int indexEndCopy = indexAfter + count;

            if (indexEndCopy >= buffer.Length)
                indexEndCopy = buffer.Length;

            int numCopies = indexEndCopy - indexAfter;

            for (int i = 0; i < numCopies; ++i)
                buffer.SetElement(index + i, buffer.GetElement(index + count + i));

            for (int i = indexAfter; i < buffer.Length - 1; ++i)
                buffer.SetElement(i, buffer.GetElement(i + 1));
        }
    }


    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct SmallBuffer14<T> : ISmallBuffer<T> where T : unmanaged
    {
        public ushort Length { get; private set; }

        private readonly T element0;
        private readonly T element1;
        private readonly T element2;
        private readonly T element3;
        private readonly T element4;
        private readonly T element5;
        private readonly T element6;
        private readonly T element7;
        private readonly T element8;
        private readonly T element9;
        private readonly T element10;
        private readonly T element11;
        private readonly T element12;
        private readonly T element13;

        public ref T this[int index] => ref GetElement(index);

        public ref T GetElement(int index)
        {
            this.RequireIndexInBounds(index);
            fixed (T* elements = &element0)
                return ref elements[index];
        }

        public void SetElement(int index, T value)
        {
            fixed (T* elements = &element0)
                elements[index] = value;
        }

        public void Add(T item)
        {
            this.RequireNotFull();
            SetElement(Length, item);
            Length++;
        }

        public void Clear()
        {
            this.Clear<T>();
            Length = 0;
        }

        public void Insert(int index, T value)
        {
            this.Insert<T>(index, value);
            Length++;
        }

        public void RemoveAt(int index)
        {
            this.RemoveAt<T>(index);
            Length--;
        }

        public void RemoveRange(int index, ushort count)
        {
            this.RemoveRange<T>(index, count);
            Length -= count;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct SmallBuffer12<T> : ISmallBuffer<T> where T : unmanaged
    {
        public ushort Length { get; private set; }

        private readonly T element0;
        private readonly T element1;
        private readonly T element2;
        private readonly T element3;
        private readonly T element4;
        private readonly T element5;
        private readonly T element6;
        private readonly T element7;
        private readonly T element8;
        private readonly T element9;
        private readonly T element10;
        private readonly T element11;

        public ref T this[int index] => ref GetElement(index);

        public ref T GetElement(int index)
        {
            this.RequireIndexInBounds(index);
            fixed (T* elements = &element0)
                return ref elements[index];
        }

        public void SetElement(int index, T value)
        {
            fixed (T* elements = &element0)
                elements[index] = value;
        }

        public void Add(T item)
        {
            this.RequireNotFull();
            SetElement(Length, item);
            Length++;
        }

        public void Clear()
        {
            this.Clear<T>();
            Length = 0;
        }

        public void Insert(int index, T value)
        {
            this.Insert<T>(index, value);
            Length++;
        }

        public void RemoveAt(int index)
        {
            this.RemoveAt<T>(index);
            Length--;
        }

        public void RemoveRange(int index, ushort count)
        {
            this.RemoveRange<T>(index, count);
            Length -= count;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct SmallBuffer10<T> : ISmallBuffer<T> where T : unmanaged
    {
        public ushort Length { get; private set; }

        private readonly T element0;
        private readonly T element1;
        private readonly T element2;
        private readonly T element3;
        private readonly T element4;
        private readonly T element5;
        private readonly T element6;
        private readonly T element7;
        private readonly T element8;
        private readonly T element9;

        public ref T this[int index] => ref GetElement(index);

        public ref T GetElement(int index)
        {
            this.RequireIndexInBounds(index);
            fixed (T* elements = &element0)
                return ref elements[index];
        }

        public void SetElement(int index, T value)
        {
            fixed (T* elements = &element0)
                elements[index] = value;
        }

        public void Add(T item)
        {
            this.RequireNotFull();
            SetElement(Length, item);
            Length++;
        }

        public void Clear()
        {
            this.Clear<T>();
            Length = 0;
        }

        public void Insert(int index, T value)
        {
            this.Insert<T>(index, value);
            Length++;
        }

        public void RemoveAt(int index)
        {
            this.RemoveAt<T>(index);
            Length--;
        }

        public void RemoveRange(int index, ushort count)
        {
            this.RemoveRange<T>(index, count);
            Length -= count;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct SmallBuffer7<T> : ISmallBuffer<T> where T : unmanaged
    {
        public ushort Length { get; private set; }

        private readonly T element0;
        private readonly T element1;
        private readonly T element2;
        private readonly T element3;
        private readonly T element4;
        private readonly T element5;
        private readonly T element6;

        public ref T this[int index] => ref GetElement(index);

        public ref T GetElement(int index)
        {
            this.RequireIndexInBounds(index);
            fixed (T* elements = &element0)
                return ref elements[index];
        }

        public void SetElement(int index, T value)
        {
            fixed (T* elements = &element0)
                elements[index] = value;
        }

        public void Add(T item)
        {
            this.RequireNotFull();
            SetElement(Length, item);
            Length++;
        }

        public void Clear()
        {
            this.Clear<T>();
            Length = 0;
        }

        public void Insert(int index, T value)
        {
            this.Insert<T>(index, value);
            Length++;
        }

        public void RemoveAt(int index)
        {
            this.RemoveAt<T>(index);
            Length--;
        }

        public void RemoveRange(int index, ushort count)
        {
            this.RemoveRange<T>(index, count);
            Length -= count;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct SmallBuffer4<T> : ISmallBuffer<T> where T : unmanaged
    {
        public ushort Length { get; private set; }

        private readonly T element0;
        private readonly T element1;
        private readonly T element2;
        private readonly T element3;

        public ref T this[int index] => ref GetElement(index);

        public ref T GetElement(int index)
        {
            this.RequireIndexInBounds(index);
            fixed (T* elements = &element0)
                return ref elements[index];
        }

        public void SetElement(int index, T value)
        {
            fixed (T* elements = &element0)
                elements[index] = value;
        }

        public void Add(T item)
        {
            this.RequireNotFull();
            SetElement(Length, item);
            Length++;
        }

        public void Clear()
        {
            this.Clear<T>();
            Length = 0;
        }

        public void Insert(int index, T value)
        {
            this.Insert<T>(index, value);
            Length++;
        }

        public void RemoveAt(int index)
        {
            this.RemoveAt<T>(index);
            Length--;
        }

        public void RemoveRange(int index, ushort count)
        {
            this.RemoveRange<T>(index, count);
            Length -= count;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct SmallBuffer3<T> : ISmallBuffer<T> where T : unmanaged
    {
        public ushort Length { get; private set; }

        private readonly T element0;
        private readonly T element1;
        private readonly T element2;

        public ref T this[int index] => ref GetElement(index);

        public ref T GetElement(int index)
        {
            this.RequireIndexInBounds(index);
            fixed (T* elements = &element0)
                return ref elements[index];
        }

        public void SetElement(int index, T value)
        {
            fixed (T* elements = &element0)
                elements[index] = value;
        }

        public void Add(T item)
        {
            this.RequireNotFull();
            SetElement(Length, item);
            Length++;
        }

        public void Clear()
        {
            this.Clear<T>();
            Length = 0;
        }

        public void Insert(int index, T value)
        {
            this.Insert<T>(index, value);
            Length++;
        }

        public void RemoveAt(int index)
        {
            this.RemoveAt<T>(index);
            Length--;
        }

        public void RemoveRange(int index, ushort count)
        {
            this.RemoveRange<T>(index, count);
            Length -= count;
        }
    }
}
