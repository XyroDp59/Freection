using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A serializable <see cref="System.Collections.Generic.Dictionary{K,V}"/> class that supports basic events.
/// </summary>
/// <typeparam name="K"></typeparam>
/// <typeparam name="V"></typeparam>
[Serializable]
public class TDictionary<K,V> : IEnumerable
{
    public int Count => content.Count;

    [Serializable]
    public struct Data
    {
        public K key;
        public V value;
    }
    
    [SerializeField] List<Data> content;

    /// <summary>
    /// Event triggered right after adding an element to the TDictionary. K is the added key, V is the value assigned to the key.
    /// </summary>
    public Action<K,V> OnAdd;
    /// <summary>
    /// Event triggered right after removing an element of the TDictionary. K is the removed key, V is the value assigned to the key.
    /// </summary>
    public Action<K,V> OnRemove;
    /// <summary>
    /// Event triggered right after modifying an element of the TDictionary. V1 is the old value, K is the key, V2 is the new value.
    /// </summary>
    public Action<V,K,V> OnModify;
    /// <summary>
    /// Event triggered right after clearing the TDictionary. int is the number of cleared elements.
    /// </summary>
    public Action<int> OnClear;

    public V this[K key]
    {
        get => content.Find((data) => data.key.Equals(key)).value;
        set => setValue(key,value);
    }
    private void setValue(K key, V value)
    {
        if (content.Exists((data) => data.key.Equals(key)))
        {
            Data data = content.Find((data) => data.key.Equals(key));
            V oldValue = data.value; 
            data.value = value;
            OnModify.Invoke(oldValue, key, value);
        }   
        else 
        {
            throw new KeyNotFoundException();
        }
    }

    public void Add(K key, V value)
    {
        if (!content.Exists((data) => data.key.Equals(key)))
        {
            content.Add(new Data{key = key, value = value});
            OnAdd.Invoke(key, value);
        }
    }

    public void Remove(K key)
    {
        if (content.Exists((data) => data.key.Equals(key)))
        {
            V removedValue = content.Find((data) => data.key.Equals(key)).value;
            content.RemoveAll((data) => data.key.Equals(key));
            OnRemove.Invoke(key, removedValue);
        }
    }

    public void Clear()
    {
        int oldCount = content.Count;
        content.Clear();
        OnClear.Invoke(oldCount);
    }

    public bool ContainsKey(K key)
    {
        return content.Exists((data) => data.key.Equals(key));
    }
    public bool ContainsValue(V value)
    {
        return content.Exists((data) => data.value.Equals(value));
    }

    public K KeyAt(int index)
    {
        return content[index].key;
    }
    public V ValueAt(int index)
    {
        return content[index].value;
    }

    public bool Exists(System.Predicate<(K,V)> predicate)
    {
        for (int i = 0; i < content.Count; ++i)
        {
            if (predicate.Invoke((content[i].key, content[i].value)))
            {
                return true;
            }
        }
        return false;
    }
    public (K,V) Find(System.Predicate<(K,V)> predicate)
    {
        for (int i = 0; i < content.Count; ++i)
        {
            if (predicate.Invoke((content[i].key, content[i].value)))
            {
                return (content[i].key, content[i].value);
            }
        }
        return default;
    }

    private class TDictEnumerator : IEnumerator
    {
        public List<Data> content;
        private int position = -1;

        public TDictEnumerator(List<Data> c)
        {
            content = c;
        }

        public object Current
        {
            get
                {
                    try
                    {
                        return content[position];
                    }
                    catch (System.IndexOutOfRangeException)
                    {
                        throw new System.InvalidOperationException();
                    }
                }
        }

        public bool MoveNext()
        {
            position++;
            return position < content.Count;
        }

        public void Reset()
        {
            position = -1;
        }
    }

    public IEnumerator GetEnumerator()
    {
        return new TDictEnumerator(content);
    }
}
