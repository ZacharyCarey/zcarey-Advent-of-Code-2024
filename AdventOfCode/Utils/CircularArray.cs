using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    /// <summary>
    /// A standard array except elements are accesed in a circular fashion.
    /// </summary>
    public class CircularArray<T> : IEnumerable<T>
    {

        /// <summary>
        /// The total number of elements the array can hold.
        /// </summary>
        public int Capacity { get => array.Length; }

        /// <summary>
        /// The current index of the array. This value will always be in the range [0, Capacity).
        /// </summary>
        public int Index
        {
            get => index;
            set
            {
                index = ((value % array.Length) + array.Length) % array.Length;
            }
        }

        /// <summary>
        /// The current selected element in the array. 
        /// This is based on the Index and can be changed using 'Move(int offset)', 'Next(int count)', or 'Previous(int count)'.
        /// </summary>
        public T Current
        {
            get => array[index];
            set => array[index] = value;
        }

        //The internal array used to store the elements.
        private readonly T[] array;

        //The index in the array of the currently selected element.
        private int index = 0;

        /// <summary>
        /// Initialize the array with the given capacity.
        /// </summary>
        /// <param name="size"></param>
        public CircularArray(int size)
        {
            array = new T[size];
        }

        /// <summary>
        /// Initialize the array with the given capacity and fill the array with the given elements.
        /// If more elements are given than the capacity allows, the extra elements will be discarded and not saved in the array.
        /// If less elements are given than the capacity, the remaining elements are initialized with a default value.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="initialize"></param>
        public CircularArray(int size, IEnumerable<T> initialize) : this(size)
        {
            int index = 0;
            foreach (T element in initialize.Take(size))
            {
                array[index++] = element;
            }
        }

        /// <summary>
        /// Initialize the array with a given set of elements. The capacity of the array is determined by the number of elements given.
        /// </summary>
        /// <param name="initialize"></param>
        public CircularArray(IEnumerable<T> initialize) : this(initialize.Count(), initialize)
        {
        }

        /// <summary>
        /// Creaty a copy of another circular array. The new array defaults to the first element.
        /// </summary>
        /// <param name="copy"></param>
        public CircularArray(CircularArray<T> copy)
        {
            this.array = new T[copy.array.Length];
            Array.Copy(copy.array, this.array, copy.array.Length);
        }

        /// <summary>
        /// Access an element based on it's index. Negative and positive indexes are allowed and will simply wrap around the array as needed, creating a "circular" effect.
        /// For example, '-1' will access the last element in the array.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get
            {
                index = ((index % array.Length) + array.Length) % array.Length;
                return this.array[index];
            }
            set
            {
                index = ((index % array.Length) + array.Length) % array.Length;
                this.array[index] = value;
            }
        }

        /// <summary>
        /// Resets the index to the first element.
        /// </summary>
        public void Reset()
        {
            this.index = 0;
        }

        /// <summary>
        /// Move the current index by the given offset (default 1).
        /// Negative offsets are allowed and will simply move in the opposite direction as positive offsets.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public T Move(int offset = 1)
        {
            index += offset;
            index = ((index % array.Length) + array.Length) % array.Length;
            return array[index];
        }

        /// <summary>
        /// Move the current index by the given count (default 1) in the positive direction.
        /// Only moves in the positive direction, negative counts are not allowed.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public T Next(int count = 1)
        {
            if (count < 0) throw new ArgumentException("Count can't be negative. Consider using \'Move(int count)\' instead.", nameof(count));
            index += count;
            index %= array.Length;
            return array[index];
        }

        /// <summary>
        /// Move the current index by the given count (default 1) in the negative direction.
        /// Only moves in the negative direction, negative counts are not allowed.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public T Previous()
        {
            index -= 1;
            index = (index + array.Length) % array.Length;
            return array[index];
        }

        /// <summary>
        /// Move the current index by the given count (default 1) in the negative direction.
        /// Only moves in the negative direction, negative counts are not allowed.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public T Previous(int count = 1)
        {
            if (count < 0) throw new ArgumentException("Count can't be negative. Consider using \'Move(int count)\' instead.", nameof(count));
            index -= count;
            index = ((index % array.Length) + array.Length) % array.Length;
            return array[index];
        }

        /// <summary>
        /// Peeks at an element with the given offset (default 1).
        /// Negative offsets are allowed, and will simply offset in the opposite direction as a positive offset.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public T Peek(int offset = 1)
        {
            int peekIndex = this.index + offset;
            peekIndex = ((peekIndex % array.Length) + array.Length) % array.Length;
            return array[peekIndex];
        }

        /// <summary>
        /// Returns elements of the array moving in the positive direction.
        /// By default, will infinitely return items (as the array is "circular"). Set "infinite" to false to only iterate over each element once.
        /// The iteration begins at the current item.
        /// </summary>
        /// <param name="infinite">Set to false to only iterate over each element once.</param>
        /// <returns></returns>
        public IEnumerable<T> Forward(bool infinite = true)
        {
            if (infinite)
            {
                int index = this.index;
                while (true)
                {
                    yield return this.array[index];
                    index = (index + 1) % this.array.Length;
                }
            }
            else
            {
                int index = this.index;
                do
                {
                    yield return this.array[index];
                    index = (index + 1) % this.array.Length;
                } while (index != this.index);
            }
        }

        /// <summary>
        /// Returns elements of the array moving in the negative direction.
        /// By default, will infinitely return items (as the array is "circular"). Set "infinite" to false to aonly iterate over each element once.
        /// The iteration begins at the current item.
        /// </summary>
        /// <param name="infinite">Set to false to only iterate over each element once.</param>
        /// <returns></returns>
        public IEnumerable<T> Reverse(bool infinite = true)
        {
            if (infinite)
            {
                int index = this.index;
                while (true)
                {
                    yield return this.array[index];
                    index = (index - 1 + array.Length) % array.Length;
                }
            }
            else
            {
                int index = this.index;
                do
                {
                    yield return this.array[index];
                    index = (index - 1 + array.Length) % array.Length;
                } while (index != this.index);
            }
        }

        /// <summary>
        /// Fills the array with the given elements starting at the current index and moving in the positive direction.
        /// If the number of elements is greater than the capacity of the array, some of the given elements will be overriden in the array as the index wraps around.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="count"></param>
        public void Fill(IEnumerable<T> elements)
        {
            int index = this.index;
            foreach (T element in elements)
            {
                array[index] = element;
                index = (index + 1) % array.Length;
            }
        }

        /// <summary>
        /// Fills the array with the given elements starting at the current index and moving in the positive direction.
        /// If count is less than the given number of elements, only the given elements will be filled.
        /// If count is greater than the capacity of the array, some of the given elements will be overriden in the array as the index wraps around.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="count"></param>
        public void Fill(IEnumerable<T> elements, int count)
        {
            int index = this.index;
            foreach (T element in elements.Take(count))
            {
                array[index] = element;
                index = (index + 1) % array.Length;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Forward(false).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
