using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NonsensicalKit.Custom
{

    class AutoSortList<T> where T : IComparable
    {
        private List<T> values = new List<T>();

        public T this[int index] { get { return values[index]; } set { values[index] = value; } }

        public int Count => values.Count;


        public void Add(T item)
        {

            for (int i = 0; i < values.Count; i++)
            {
                if (values[i].CompareTo(item) > 0)
                {
                    values.Insert(i, item);
                    return;
                }
            }
            values.Add(item);
        }

        public int CheckPos(T item)
        {
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i].CompareTo(item) > 0)
                {
                    return i;
                }
            }
            return values.Count;
        }

        public void Clear()
        {
            values.Clear();
        }

        public bool Contains(T item)
        {
            return values.Contains(item);
        }

        public bool Remove(T item)
        {
            return values.Remove(item);
        }

        public void RemoveAt(int index)
        {
            values.RemoveAt(index);
        }

        public T[] ToArray()
        {
            return values.ToArray();
        }
    }
}
