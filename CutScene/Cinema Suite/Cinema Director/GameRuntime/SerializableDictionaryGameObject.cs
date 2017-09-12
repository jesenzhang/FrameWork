using System;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

[Serializable]
public class ReplaceResData
{
    public string ResName;
    public GameObject Obj;
}

[Serializable]
public class SerializableDictionaryGameObject : IDictionary<string, ReplaceResData>, IDictionary
{
    // can't serialize internal generic struct.
    // extract to 4 arrays.

    //struct Entry
    //{
    //    public int hashCode;
    //    public int next;
    //    public string key;
    //    public string value;
    //}

    // serializable state.
    [SerializeField, HideInInspector]
    int[] buckets; // link index of first entry. empty is -1.
    [SerializeField, HideInInspector]
    int count;

    // Entry[] entries;
    [SerializeField, HideInInspector]
    int[] entriesHashCode;
    [SerializeField, HideInInspector]
    int[] entriesNext;
    [SerializeField, HideInInspector]
    string[] entriesKey;
    [SerializeField, HideInInspector]
    ReplaceResData[] entriesValue;

    // when remove, mark slot to free
    [SerializeField, HideInInspector]
    int freeCount;
    [SerializeField, HideInInspector]
    int freeList;

    int version; // version does not serialize

    KeyCollection keys;
    ValueCollection values;
    object _syncRoot;

    // equality comparer is not serializable, use specified comparer.
    public virtual IEqualityComparer<string> Comparer
    {
        get
        {
            return EqualityComparer<string>.Default;
        }
    }

    public SerializableDictionaryGameObject()
    {
        Initialize(0);
    }

    protected SerializableDictionaryGameObject(int initialCapacity)
    {
        Initialize(initialCapacity);
    }

    SerializableDictionaryGameObject(int staticCapacity, bool forceSize)
    {
        Initialize(staticCapacity, forceSize);
    }

    public int Count
    {
        get { return count - freeCount; }
    }

    public KeyCollection Keys
    {
        get
        {
            if (keys == null) keys = new KeyCollection(this);
            return keys;
        }
    }

    ICollection<string> IDictionary<string, ReplaceResData>.Keys
    {
        get
        {
            if (keys == null) keys = new KeyCollection(this);
            return keys;
        }
    }

    public ValueCollection Values
    {
        get
        {
            if (values == null) values = new ValueCollection(this);
            return values;
        }
    }

    ICollection<ReplaceResData> IDictionary<string, ReplaceResData>.Values
    {
        get
        {
            if (values == null) values = new ValueCollection(this);
            return values;
        }
    }

    public ReplaceResData this[string key]
    {
        get
        {
            int i = FindEntry(key);
            if (i >= 0) return entriesValue[i];
            throw new KeyNotFoundException();
        }
        set
        {
            Insert(key, value, false);
        }
    }

    public void Add(string key, ReplaceResData value)
    {
        Insert(key, value, true);
    }

    void ICollection<KeyValuePair<string, ReplaceResData>>.Add(KeyValuePair<string, ReplaceResData> keyValuePair)
    {
        Add(keyValuePair.Key, keyValuePair.Value);
    }

    bool ICollection<KeyValuePair<string, ReplaceResData>>.Contains(KeyValuePair<string, ReplaceResData> keyValuePair)
    {
        int i = FindEntry(keyValuePair.Key);
        if (i >= 0 && EqualityComparer<ReplaceResData>.Default.Equals(entriesValue[i], keyValuePair.Value))
        {
            return true;
        }
        return false;
    }

    bool ICollection<KeyValuePair<string, ReplaceResData>>.Remove(KeyValuePair<string, ReplaceResData> keyValuePair)
    {
        int i = FindEntry(keyValuePair.Key);
        if (i >= 0 && EqualityComparer<ReplaceResData>.Default.Equals(entriesValue[i], keyValuePair.Value))
        {
            Remove(keyValuePair.Key);
            return true;
        }
        return false;
    }

    public void Clear()
    {
        if (count > 0)
        {
            for (int i = 0; i < buckets.Length; i++) buckets[i] = -1;
            Array.Clear(entriesHashCode, 0, count);
            Array.Clear(entriesKey, 0, count);
            Array.Clear(entriesNext, 0, count);
            Array.Clear(entriesValue, 0, count);

            freeList = -1;
            count = 0;
            freeCount = 0;
            version++;
        }
    }

    public bool ContainsKey(string key)
    {
        return FindEntry(key) >= 0;
    }

    public bool ContainsValue(ReplaceResData value)
    {
        if (value == null)
        {
            for (int i = 0; i < count; i++)
            {
                if (entriesHashCode[i] >= 0 && entriesValue[i] == null) return true;
            }
        }
        else
        {
            EqualityComparer<ReplaceResData> c = EqualityComparer<ReplaceResData>.Default;
            for (int i = 0; i < count; i++)
            {
                if (entriesHashCode[i] >= 0 && c.Equals(entriesValue[i], value)) return true;
            }
        }
        return false;
    }

    private void CopyTo(KeyValuePair<string, ReplaceResData>[] array, int index)
    {
        if (array == null)
        {
            throw new ArgumentNullException("array");
        }

        if (index < 0 || index > array.Length)
        {
            throw new ArgumentOutOfRangeException("index", index, SR.ArgumentOutOfRange_Index);
        }

        if (array.Length - index < Count)
        {
            throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);
        }

        int count = this.count;
        for (int i = 0; i < count; i++)
        {
            if (entriesHashCode[i] >= 0)
            {
                array[index++] = new KeyValuePair<string, ReplaceResData>(entriesKey[i], entriesValue[i]);
            }
        }
    }

    public Enumerator GetEnumerator()
    {
        return new Enumerator(this, Enumerator.KeyValuePair);
    }

    IEnumerator<KeyValuePair<string, ReplaceResData>> IEnumerable<KeyValuePair<string, ReplaceResData>>.GetEnumerator()
    {
        return new Enumerator(this, Enumerator.KeyValuePair);
    }

    int FindEntry(string key)
    {
        if (key == null) throw new ArgumentNullException("key");

        if (buckets != null)
        {
            int hashCode = Comparer.GetHashCode(key) & 0x7FFFFFFF;
            for (int i = buckets[hashCode % buckets.Length]; i >= 0; i = entriesNext[i])
            {
                if (entriesHashCode[i] == hashCode && Comparer.Equals(entriesKey[i], key)) return i;
            }
        }

        return -1;
    }

    void Initialize(int capacity)
    {
        Initialize(capacity, false);
    }

    void Initialize(int capacity, bool forceSize)
    {
        int size = forceSize ? capacity : HashHelpers.GetPrime(capacity);
        buckets = new int[size];
        for (int i = 0; i < buckets.Length; i++) buckets[i] = -1;
        entriesHashCode = new int[size];
        entriesKey = new string[size];
        entriesNext = new int[size];
        entriesValue = new ReplaceResData[size];

        freeList = -1;
    }

    void Insert(string key, ReplaceResData value, bool add)
    {
        if (key == null) throw new ArgumentNullException("key");
        if (buckets == null || buckets.Length == 0) Initialize(0);

        int hashCode = Comparer.GetHashCode(key) & 0x7FFFFFFF;
        int targetBucket = hashCode % buckets.Length;

        for (int i = buckets[targetBucket]; i >= 0; i = entriesNext[i])
        {
            if (entriesHashCode[i] == hashCode && Comparer.Equals(entriesKey[i], key))
            {
                if (add)
                {
                    throw new ArgumentException(SR.Format(SR.Argument_AddingDuplicate, key));
                }
                entriesValue[i] = value;
                version++;
                return;
            }
        }

        int index;

        if (freeCount > 0)
        {
            index = freeList;
            freeList = entriesNext[index];
            freeCount--;
        }
        else
        {
            if (count == entriesHashCode.Length)
            {
                Resize();
                targetBucket = hashCode % buckets.Length;
            }
            index = count;
            count++;
        }

        entriesHashCode[index] = hashCode;
        entriesNext[index] = buckets[targetBucket];
        entriesKey[index] = key;
        entriesValue[index] = value;
        buckets[targetBucket] = index;
        version++;
    }

    void Resize()
    {
        Resize(HashHelpers.ExpandPrime(count), false);
    }

    void Resize(int newSize, bool forceNewHashCodes)
    {
        int[] newBuckets = new int[newSize];
        for (int i = 0; i < newBuckets.Length; i++) newBuckets[i] = -1;

        var newEntriesKey = new string[newSize];
        var newEntriesValue = new ReplaceResData[newSize];
        var newEntriesHashCode = new int[newSize];
        var newEntriesNext = new int[newSize];
        Array.Copy(entriesKey, 0, newEntriesKey, 0, count);
        Array.Copy(entriesValue, 0, newEntriesValue, 0, count);
        Array.Copy(entriesHashCode, 0, newEntriesHashCode, 0, count);
        Array.Copy(entriesNext, 0, newEntriesNext, 0, count);

        if (forceNewHashCodes)
        {
            for (int i = 0; i < count; i++)
            {
                if (newEntriesHashCode[i] != -1)
                {
                    newEntriesHashCode[i] = (Comparer.GetHashCode(newEntriesKey[i]) & 0x7FFFFFFF);
                }
            }
        }

        for (int i = 0; i < count; i++)
        {
            if (newEntriesHashCode[i] >= 0)
            {
                int bucket = newEntriesHashCode[i] % newSize;
                newEntriesNext[i] = newBuckets[bucket];
                newBuckets[bucket] = i;
            }
        }

        buckets = newBuckets;

        entriesKey = newEntriesKey;
        entriesValue = newEntriesValue;
        entriesHashCode = newEntriesHashCode;
        entriesNext = newEntriesNext;
    }


    public bool Remove(string key)
    {
        if (key == null) throw new ArgumentNullException("key");

        if (buckets != null)
        {
            int hashCode = Comparer.GetHashCode(key) & 0x7FFFFFFF;
            int bucket = hashCode % buckets.Length;
            int last = -1;
            for (int i = buckets[bucket]; i >= 0; last = i, i = entriesNext[i])
            {
                if (entriesHashCode[i] == hashCode && Comparer.Equals(entriesKey[i], key))
                {
                    if (last < 0)
                    {
                        buckets[bucket] = entriesNext[i];
                    }
                    else
                    {
                        entriesNext[last] = entriesNext[i];
                    }
                    entriesHashCode[i] = -1;
                    entriesNext[i] = freeList;
                    entriesKey[i] = default(string);
                    entriesValue[i] = default(ReplaceResData);
                    freeList = i;
                    freeCount++;
                    version++;
                    return true;
                }
            }
        }
        return false;
    }

    public bool TryGetValue(string key, out ReplaceResData value)
    {
        int i = FindEntry(key);
        if (i >= 0)
        {
            value = entriesValue[i];
            return true;
        }
        value = default(ReplaceResData);
        return false;
    }

    bool ICollection<KeyValuePair<string, ReplaceResData>>.IsReadOnly
    {
        get { return false; }
    }

    void ICollection<KeyValuePair<string, ReplaceResData>>.CopyTo(KeyValuePair<string, ReplaceResData>[] array, int index)
    {
        CopyTo(array, index);
    }

    void ICollection.CopyTo(Array array, int index)
    {
        if (array == null)
        {
            throw new ArgumentNullException("array");
        }

        if (array.Rank != 1)
        {
            throw new ArgumentException(SR.Arg_RankMultiDimNotSupported, "array");
        }

        if (array.GetLowerBound(0) != 0)
        {
            throw new ArgumentException(SR.Arg_NonZeroLowerBound, "array");
        }

        if (index < 0 || index > array.Length)
        {
            throw new ArgumentOutOfRangeException("index", index, SR.ArgumentOutOfRange_Index);
        }

        if (array.Length - index < Count)
        {
            throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);
        }

        KeyValuePair<string, ReplaceResData>[] pairs = array as KeyValuePair<string, ReplaceResData>[];
        if (pairs != null)
        {
            CopyTo(pairs, index);
        }
        else if (array is DictionaryEntry[])
        {
            DictionaryEntry[] dictEntryArray = array as DictionaryEntry[];
            for (int i = 0; i < count; i++)
            {
                if (entriesHashCode[i] >= 0)
                {
                    dictEntryArray[index++] = new DictionaryEntry(entriesKey[i], entriesValue[i]);
                }
            }
        }
        else
        {
            object[] objects = array as object[];
            if (objects == null)
            {
                throw new ArgumentException(SR.Argument_InvalidArrayType, "array");
            }

            try
            {
                int count = this.count;
                for (int i = 0; i < count; i++)
                {
                    if (entriesHashCode[i] >= 0)
                    {
                        objects[index++] = new KeyValuePair<string, ReplaceResData>(entriesKey[i], entriesValue[i]);
                    }
                }
            }
            catch (ArrayTypeMismatchException)
            {
                throw new ArgumentException(SR.Argument_InvalidArrayType, "array");
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return new Enumerator(this, Enumerator.KeyValuePair);
    }

    bool ICollection.IsSynchronized
    {
        get { return false; }
    }

    object ICollection.SyncRoot
    {
        get
        {
            if (_syncRoot == null)
            {
                System.Threading.Interlocked.CompareExchange<object>(ref _syncRoot, new object(), null);
            }
            return _syncRoot;
        }
    }

    bool IDictionary.IsFixedSize
    {
        get { return false; }
    }

    bool IDictionary.IsReadOnly
    {
        get { return false; }
    }

    ICollection IDictionary.Keys
    {
        get { return (ICollection)Keys; }
    }

    ICollection IDictionary.Values
    {
        get { return (ICollection)Values; }
    }

    object IDictionary.this[object key]
    {
        get
        {
            if (IsCompatibleKey(key))
            {
                int i = FindEntry((string)key);
                if (i >= 0)
                {
                    return entriesValue[i];
                }
            }
            return null;
        }
        set
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            try
            {
                string tempKey = (string)key;
                try
                {
                    this[tempKey] = (ReplaceResData)value;
                }
                catch (InvalidCastException)
                {
                    throw new ArgumentException(SR.Format(SR.Arg_WrongType, value, typeof(string)), "value");
                }
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException(SR.Format(SR.Arg_WrongType, key, typeof(string)), "key");
            }
        }
    }

    private static bool IsCompatibleKey(object key)
    {
        if (key == null)
        {
            throw new ArgumentNullException("key");
        }
        return (key is string);
    }

    void IDictionary.Add(object key, object value)
    {
        if (key == null)
        {
            throw new ArgumentNullException("key");
        }

        if (value == null)
            throw new ArgumentNullException("value");

        try
        {
            string tempKey = (string)key;

            try
            {
                Add(tempKey, (ReplaceResData)value);
            }
            catch (InvalidCastException)
            {
                throw new ArgumentException(SR.Format(SR.Arg_WrongType, value, typeof(string)), "value");
            }
        }
        catch (InvalidCastException)
        {
            throw new ArgumentException(SR.Format(SR.Arg_WrongType, key, typeof(string)), "key");
        }
    }

    bool IDictionary.Contains(object key)
    {
        if (IsCompatibleKey(key))
        {
            return ContainsKey((string)key);
        }

        return false;
    }

    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
        return new Enumerator(this, Enumerator.DictEntry);
    }

    void IDictionary.Remove(object key)
    {
        if (IsCompatibleKey(key))
        {
            Remove((string)key);
        }
    }

    public void TrimExcess()
    {
        var newDict = new SerializableDictionaryGameObject(Count, true);

        // fast copy
        for (int i = 0; i < count; i++)
        {
            if (entriesHashCode[i] >= 0)
            {
                newDict.Add(entriesKey[i], entriesValue[i]);
            }
        }

        // copy internal field to this
        this.buckets = newDict.buckets;
        this.count = newDict.count;
        this.entriesHashCode = newDict.entriesHashCode;
        this.entriesKey = newDict.entriesKey;
        this.entriesNext = newDict.entriesNext;
        this.entriesValue = newDict.entriesValue;
        this.freeCount = newDict.freeCount;
        this.freeList = newDict.freeList;
    }

    public struct Enumerator : IEnumerator<KeyValuePair<string, ReplaceResData>>,
        IDictionaryEnumerator
    {
        private SerializableDictionaryGameObject dictionary;
        private int version;
        private int index;
        private KeyValuePair<string, ReplaceResData> current;
        private int getEnumeratorRetType;  // What should Enumerator.Current return?

        internal const int DictEntry = 1;
        internal const int KeyValuePair = 2;

        internal Enumerator(SerializableDictionaryGameObject dictionary, int getEnumeratorRetType)
        {
            this.dictionary = dictionary;
            version = dictionary.version;
            index = 0;
            this.getEnumeratorRetType = getEnumeratorRetType;
            current = new KeyValuePair<string, ReplaceResData>();
        }

        public bool MoveNext()
        {
            if (version != dictionary.version)
            {
                throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
            }

            // Use unsigned comparison since we set index to dictionary.count+1 when the enumeration ends.
            // dictionary.count+1 could be negative if dictionary.count is Int32.MaxValue
            while ((uint)index < (uint)dictionary.count)
            {
                if (dictionary.entriesHashCode[index] >= 0)
                {
                    current = new KeyValuePair<string, ReplaceResData>(dictionary.entriesKey[index], dictionary.entriesValue[index]);
                    index++;
                    return true;
                }
                index++;
            }

            index = dictionary.count + 1;
            current = new KeyValuePair<string, ReplaceResData>();
            return false;
        }

        public KeyValuePair<string, ReplaceResData> Current
        {
            get { return current; }
        }

        public void Dispose()
        {
        }

        object IEnumerator.Current
        {
            get
            {
                if (index == 0 || (index == dictionary.count + 1))
                {
                    throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                }

                if (getEnumeratorRetType == DictEntry)
                {
                    return new System.Collections.DictionaryEntry(current.Key, current.Value);
                }
                else
                {
                    return new KeyValuePair<string, ReplaceResData>(current.Key, current.Value);
                }
            }
        }

        void IEnumerator.Reset()
        {
            if (version != dictionary.version)
            {
                throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
            }

            index = 0;
            current = new KeyValuePair<string, ReplaceResData>();
        }

        DictionaryEntry IDictionaryEnumerator.Entry
        {
            get
            {
                if (index == 0 || (index == dictionary.count + 1))
                {
                    throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                }

                return new DictionaryEntry(current.Key, current.Value);
            }
        }

        object IDictionaryEnumerator.Key
        {
            get
            {
                if (index == 0 || (index == dictionary.count + 1))
                {
                    throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                }

                return current.Key;
            }
        }

        object IDictionaryEnumerator.Value
        {
            get
            {
                if (index == 0 || (index == dictionary.count + 1))
                {
                    throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                }

                return current.Value;
            }
        }
    }

    //[DebuggerTypeProxy(typeof(DictionaryKeyCollectionDebugView<,>))]
    [DebuggerDisplay("Count = {Count}")]
    public sealed class KeyCollection : ICollection<string>, ICollection
    {
        private SerializableDictionaryGameObject dictionary;

        public KeyCollection(SerializableDictionaryGameObject dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }
            this.dictionary = dictionary;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(dictionary);
        }

        public void CopyTo(string[] array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (index < 0 || index > array.Length)
            {
                throw new ArgumentOutOfRangeException("index", index, SR.ArgumentOutOfRange_Index);
            }

            if (array.Length - index < dictionary.Count)
            {
                throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);
            }

            int count = dictionary.count;

            for (int i = 0; i < count; i++)
            {
                if (dictionary.entriesHashCode[i] >= 0) array[index++] = dictionary.entriesKey[i];
            }
        }

        public int Count
        {
            get { return dictionary.Count; }
        }

        bool ICollection<string>.IsReadOnly
        {
            get { return true; }
        }

        void ICollection<string>.Add(string item)
        {
            throw new NotSupportedException(SR.NotSupported_KeyCollectionSet);
        }

        void ICollection<string>.Clear()
        {
            throw new NotSupportedException(SR.NotSupported_KeyCollectionSet);
        }

        bool ICollection<string>.Contains(string item)
        {
            return dictionary.ContainsKey(item);
        }

        bool ICollection<string>.Remove(string item)
        {
            throw new NotSupportedException(SR.NotSupported_KeyCollectionSet);
        }

        IEnumerator<string> IEnumerable<string>.GetEnumerator()
        {
            return new Enumerator(dictionary);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(dictionary);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (array.Rank != 1)
            {
                throw new ArgumentException(SR.Arg_RankMultiDimNotSupported, "array");
            }

            if (array.GetLowerBound(0) != 0)
            {
                throw new ArgumentException(SR.Arg_NonZeroLowerBound, "array");
            }

            if (index < 0 || index > array.Length)
            {
                throw new ArgumentOutOfRangeException("index", index, SR.ArgumentOutOfRange_Index);
            }

            if (array.Length - index < dictionary.Count)
            {
                throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);
            }

            string[] keys = array as string[];
            if (keys != null)
            {
                CopyTo(keys, index);
            }
            else
            {
                object[] objects = array as object[];
                if (objects == null)
                {
                    throw new ArgumentException(SR.Argument_InvalidArrayType, "array");
                }

                int count = dictionary.count;
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (dictionary.entriesHashCode[i] >= 0) objects[index++] = (object)dictionary.entriesKey[i];
                    }
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException(SR.Argument_InvalidArrayType, "array");
                }
            }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { return ((ICollection)dictionary).SyncRoot; }
        }

        public struct Enumerator : IEnumerator<string>, System.Collections.IEnumerator
        {
            private SerializableDictionaryGameObject dictionary;
            private int index;
            private int version;
            private string currentKey;

            internal Enumerator(SerializableDictionaryGameObject dictionary)
            {
                this.dictionary = dictionary;
                version = dictionary.version;
                index = 0;
                currentKey = default(string);
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (version != dictionary.version)
                {
                    throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                }

                while ((uint)index < (uint)dictionary.count)
                {
                    if (dictionary.entriesHashCode[index] >= 0)
                    {
                        currentKey = dictionary.entriesKey[index];
                        index++;
                        return true;
                    }
                    index++;
                }

                index = dictionary.count + 1;
                currentKey = default(string);
                return false;
            }

            public string Current
            {
                get
                {
                    return currentKey;
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    if (index == 0 || (index == dictionary.count + 1))
                    {
                        throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                    }

                    return currentKey;
                }
            }

            void System.Collections.IEnumerator.Reset()
            {
                if (version != dictionary.version)
                {
                    throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                }

                index = 0;
                currentKey = default(string);
            }
        }
    }

    //[DebuggerTypeProxy(typeof(DictionaryValueCollectionDebugView<,>))]
    [DebuggerDisplay("Count = {Count}")]
    public sealed class ValueCollection : ICollection<ReplaceResData>, ICollection
    {
        private SerializableDictionaryGameObject dictionary;

        public ValueCollection(SerializableDictionaryGameObject dictionary)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("dictionary");
            }
            this.dictionary = dictionary;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(dictionary);
        }

        public void CopyTo(ReplaceResData[] array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (index < 0 || index > array.Length)
            {
                throw new ArgumentOutOfRangeException("index", index, SR.ArgumentOutOfRange_Index);
            }

            if (array.Length - index < dictionary.Count)
            {
                throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);
            }

            int count = dictionary.count;

            for (int i = 0; i < count; i++)
            {
                if (dictionary.entriesHashCode[i] >= 0) array[index++] = dictionary.entriesValue[i];
            }
        }

        public int Count
        {
            get { return dictionary.Count; }
        }

        bool ICollection<ReplaceResData>.IsReadOnly
        {
            get { return true; }
        }

        void ICollection<ReplaceResData>.Add(ReplaceResData item)
        {
            throw new NotSupportedException(SR.NotSupported_ValueCollectionSet);
        }

        bool ICollection<ReplaceResData>.Remove(ReplaceResData item)
        {
            throw new NotSupportedException(SR.NotSupported_ValueCollectionSet);
        }

        void ICollection<ReplaceResData>.Clear()
        {
            throw new NotSupportedException(SR.NotSupported_ValueCollectionSet);
        }

        bool ICollection<ReplaceResData>.Contains(ReplaceResData item)
        {
            return dictionary.ContainsValue(item);
        }

        IEnumerator<ReplaceResData> IEnumerable<ReplaceResData>.GetEnumerator()
        {
            return new Enumerator(dictionary);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(dictionary);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (array.Rank != 1)
            {
                throw new ArgumentException(SR.Arg_RankMultiDimNotSupported, "array");
            }

            if (array.GetLowerBound(0) != 0)
            {
                throw new ArgumentException(SR.Arg_NonZeroLowerBound, "array");
            }

            if (index < 0 || index > array.Length)
            {
                throw new ArgumentOutOfRangeException("index", index, SR.ArgumentOutOfRange_Index);
            }

            if (array.Length - index < dictionary.Count)
                throw new ArgumentException(SR.Arg_ArrayPlusOffTooSmall);

            ReplaceResData[] values = array as ReplaceResData[];
            if (values != null)
            {
                CopyTo(values, index);
            }
            else
            {
                object[] objects = array as object[];
                if (objects == null)
                {
                    throw new ArgumentException(SR.Argument_InvalidArrayType, "array");
                }

                int count = dictionary.count;
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (dictionary.entriesHashCode[i] >= 0) objects[index++] = dictionary.entriesValue[i];
                    }
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException(SR.Argument_InvalidArrayType, "array");
                }
            }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { return ((ICollection)dictionary).SyncRoot; }
        }

        public struct Enumerator : IEnumerator<ReplaceResData>, System.Collections.IEnumerator
        {
            private SerializableDictionaryGameObject dictionary;
            private int index;
            private int version;
            private ReplaceResData currentValue;

            internal Enumerator(SerializableDictionaryGameObject dictionary)
            {
                this.dictionary = dictionary;
                version = dictionary.version;
                index = 0;
                currentValue = default(ReplaceResData);
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (version != dictionary.version)
                {
                    throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                }

                while ((uint)index < (uint)dictionary.count)
                {
                    if (dictionary.entriesHashCode[index] >= 0)
                    {
                        currentValue = dictionary.entriesValue[index];
                        index++;
                        return true;
                    }
                    index++;
                }
                index = dictionary.count + 1;
                currentValue = default(ReplaceResData);
                return false;
            }

            public ReplaceResData Current
            {
                get
                {
                    return currentValue;
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    if (index == 0 || (index == dictionary.count + 1))
                    {
                        throw new InvalidOperationException(SR.InvalidOperation_EnumOpCantHappen);
                    }

                    return currentValue;
                }
            }

            void System.Collections.IEnumerator.Reset()
            {
                if (version != dictionary.version)
                {
                    throw new InvalidOperationException(SR.InvalidOperation_EnumFailedVersion);
                }
                index = 0;
                currentValue = default(ReplaceResData);
            }
        }
    }

    static class SR
    {
        public const string InvalidOperation_EnumFailedVersion = "InvalidOperation_EnumFailedVersion";
        public const string InvalidOperation_EnumOpCantHappen = "InvalidOperation_EnumOpCantHappen";
        public const string ArgumentOutOfRange_Index = "ArgumentOutOfRange_Index";
        public const string Argument_InvalidArrayType = "Argument_InvalidArrayType";
        public const string NotSupported_ValueCollectionSet = "NotSupported_ValueCollectionSet";
        public const string Arg_RankMultiDimNotSupported = "Arg_RankMultiDimNotSupported";
        public const string Arg_ArrayPlusOffTooSmall = "Arg_ArrayPlusOffTooSmall";
        public const string Arg_NonZeroLowerBound = "Arg_NonZeroLowerBound";
        public const string NotSupported_KeyCollectionSet = "NotSupported_KeyCollectionSet";
        public const string Arg_WrongType = "Arg_WrongType";
        public const string ArgumentOutOfRange_NeedNonNegNum = "ArgumentOutOfRange_NeedNonNegNum";
        public const string Arg_HTCapacityOverflow = "Arg_HTCapacityOverflow";
        public const string Argument_AddingDuplicate = "Argument_AddingDuplicate";

        public static string Format(string f, params object[] args)
        {
            return string.Format(f, args);
        }
    }

    static class HashHelpers
    {
        // Table of prime numbers to use as hash table sizes. 
        // A typical resize algorithm would pick the smallest prime number in this array
        // that is larger than twice the previous capacity. 
        // Suppose our Hashtable currently has capacity x and enough elements are added 
        // such that a resize needs to occur. Resizing first computes 2x then finds the 
        // first prime in the table greater than 2x, i.e. if primes are ordered 
        // p_1, p_2, ..., p_i, ..., it finds p_n such that p_n-1 < 2x < p_n. 
        // Doubling is important for preserving the asymptotic complexity of the 
        // hashtable operations such as add.  Having a prime guarantees that double 
        // hashing does not lead to infinite loops.  IE, your hash function will be 
        // h1(key) + i*h2(key), 0 <= i < size.  h2 and the size must be relatively prime.
        public static readonly int[] primes = {
            3, 7, 11, 17, 23, 29, 37, 47, 59, 71, 89, 107, 131, 163, 197, 239, 293, 353, 431, 521, 631, 761, 919,
            1103, 1327, 1597, 1931, 2333, 2801, 3371, 4049, 4861, 5839, 7013, 8419, 10103, 12143, 14591,
            17519, 21023, 25229, 30293, 36353, 43627, 52361, 62851, 75431, 90523, 108631, 130363, 156437,
            187751, 225307, 270371, 324449, 389357, 467237, 560689, 672827, 807403, 968897, 1162687, 1395263,
            1674319, 2009191, 2411033, 2893249, 3471899, 4166287, 4999559, 5999471, 7199369, 8639249, 10367101,
            12440537, 14928671, 17914409, 21497293, 25796759, 30956117, 37147349, 44576837, 53492207, 64190669,
            77028803, 92434613, 110921543, 133105859, 159727031, 191672443, 230006941, 276008387, 331210079,
            397452101, 476942527, 572331049, 686797261, 824156741, 988988137, 1186785773, 1424142949, 1708971541,
            2050765853, MaxPrimeArrayLength };

        public static int GetPrime(int min)
        {
            if (min < 0)
                throw new ArgumentException(SR.Arg_HTCapacityOverflow);

            for (int i = 0; i < primes.Length; i++)
            {
                int prime = primes[i];
                if (prime >= min) return prime;
            }

            return min;
        }

        public static int GetMinPrime()
        {
            return primes[0];
        }

        // Returns size of hashtable to grow to.
        public static int ExpandPrime(int oldSize)
        {
            int newSize = 2 * oldSize;

            // Allow the hashtables to grow to maximum possible size (~2G elements) before encoutering capacity overflow.
            // Note that this check works even when _items.Length overflowed thanks to the (uint) cast
            if ((uint)newSize > MaxPrimeArrayLength && MaxPrimeArrayLength > oldSize)
            {
                return MaxPrimeArrayLength;
            }

            return GetPrime(newSize);
        }


        // This is the maximum prime smaller than Array.MaxArrayLength
        public const int MaxPrimeArrayLength = 0x7FEFFFFD;
    }
}