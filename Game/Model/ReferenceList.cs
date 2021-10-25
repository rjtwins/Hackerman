using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Game.Model
{
    [JsonObject(MemberSerialization.OptOut)]
    public class ReferenceList<T> : List<Guid>
    {
        [JsonIgnore]
        private Dictionary<Guid, T> refDict;

        [JsonIgnore]
        private Dictionary<Guid, T> RefDict
        {
            get
            {
                SetRefDictFromInfo();
                return refDict;
            }
            set
            {
                refDict = value;
                //Debug.WriteLine($"RefDict of type {RefDict.Values.ToList().GetType()} set value compare:");
                //string result = string.Empty;
                //foreach(Guid id in base.ToArray())
                //{
                //    if(!refDict.TryGetValue(id, out T item))
                //    {
                //        Debug.WriteLine("Base has keys that are not in the reference.");
                //        return;
                //    }
                //}
            }
        }

        [JsonProperty]
        public string TypeString { get; set; }
        [JsonProperty]
        public string PropertyString { get; set; }
        [JsonProperty]
        public Guid[] ArrayToSerialize { get; set; }

        [JsonConstructor]
        public ReferenceList()
        {
            
        }

        [OnSerializing]
        public void OnSerializing(StreamingContext streamingContext)
        {
            ArrayToSerialize = base.ToArray();
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext streamingContext)
        {
            base.Clear();
            base.AddRange(ArrayToSerialize);
            ArrayToSerialize = null;
            SetRefDictFromInfo();
        }

        private void SetRefDictFromInfo()
        {
            this.RefDict = (Dictionary<Guid, T>)Type.GetType(this.TypeString).GetProperty(this.PropertyString).GetValue(null);
        }

        public ReferenceList(Dictionary<Guid, T> refDict, string refDictPropertyName, Type refDictClass = null)
        {
            if(refDictClass == null)
            {
                refDictClass = typeof(Global);
            }
            TypeString = refDictClass.FullName;
            PropertyString = refDictPropertyName;
            this.RefDict = refDict;
        }

        public new ReferenceList<T> Add(T item)
        {
            Guid id = GetItemID(item);
            this.RefDict[id] = item;
            base.Add(id);
            return this;
        }

        //
        // Summary:
        //     Removes the first occurrence of a specific object from the System.Collections.Generic.List`1.
        //
        // Parameters:
        //   item:
        //     The object to remove from the System.Collections.Generic.List`1. The value can
        //     be null for reference types.
        //
        // Returns:
        //     true if item is successfully removed; otherwise, false. This method also returns
        //     false if item was not found in the System.Collections.Generic.List`1.
        public new bool Remove(T item)
        {
            Guid id = GetItemID(item);
            return base.Remove(id);
        }

        public void SetReferenceDict(Dictionary<Guid, T> refDict)
        {
            //this.RefDict = refDict;
        }

        public new T this[int index]
        {
            get
            {
                return RefDict[base[index]];
            }
            set
            {
                base[index] = GetItemID(value);
            }
        }

        public new T Get(int index)
        {
            try
            {
                return RefDict[base[index]];
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Get exception index: {index} count: {base.Count}\n Exception: {ex.Message} \n {ex.StackTrace}");
                throw;
            }
        }

        public new bool Contains(T item)
        {
            Guid id = GetItemID(item);
            return base.Contains(id);
        }

        public new List<T> ToList()
        {
            List<T> toReturn = new List<T>();
            Guid[] baseArray = new Guid[base.Count];
            base.CopyTo(baseArray);

            foreach(Guid id in baseArray)
            {
                toReturn.Add(RefDict[id]);
            }
            return toReturn;
        }

        public new T[] ToArray()
        {
            List<T> toReturn = new ();
            List<Guid> misMatches = new();
            //for (int i = 0; i < base.Count; i++)
            //{
            //    if(RefDict.TryGetValue(base[i], out T item))
            //    {
            //        toReturn.Add(item);
            //        continue;
            //    }
            //    misMatches.Add(base[i]);
            //}
            //misMatches.ForEach(x => base.Remove(x));
            base.ForEach(x => toReturn.Add(RefDict[x]));
            return toReturn.ToArray();
        }

        private Guid GetItemID(T item)
        {
            Guid id;
            try
            {
                Type type = item.GetType();
                PropertyInfo propertyInfo = type.GetProperty("Id");
                var value = propertyInfo.GetValue(item, null);
                id = (Guid)value;
            }
            catch (Exception e)
            {
                throw new Exception("LibReferenceList could not find the Id property in given item. Please make sure items supplied to this list have the Id property set");
            }

            if (id == Guid.Empty)
            {
                throw new Exception("LibReferenceList Id property contains an empty Guid.");
            }
            return id;
        }

        public new void AddRange(IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                base.Add(GetItemID(item));
            }
        }


        public new ReferenceList<T>.Enumerator GetEnumerator()
        {
            return new Enumerator(base.GetEnumerator(), this.RefDict);
        }

        public new struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
        {
            private List<Guid>.Enumerator BaseEnumerator;
            private Dictionary<Guid, T> refDict;

            public Enumerator(List<Guid>.Enumerator baseEnumerator, Dictionary<Guid, T> refDict)
            {
                this.BaseEnumerator = baseEnumerator;
                this.refDict = refDict;
            }

            public T Current
            {
                get 
                {
                    return refDict[BaseEnumerator.Current];
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return refDict[BaseEnumerator.Current];
                }
            }

            public void Dispose()
            {
                this.refDict = null;
                this.BaseEnumerator.Dispose();
            }

            public bool MoveNext()
            {
                return this.BaseEnumerator.MoveNext();
            }

            public void Reset()
            {
                ((IEnumerator)this.BaseEnumerator).Reset();
            }
        }

    }

    [JsonObject(MemberSerialization.OptOut)]
    public class ReferenceDictionary<T1,T2> : Dictionary<T1, Guid>
    {
        [JsonIgnore]
        private Dictionary<Guid, T2> refDict;

        [JsonIgnore]
        private Dictionary<Guid, T2> RefDict
        {
            get
            {
                SetRefDictFromInfo();
                return refDict;
            }
            set
            {
                refDict = value;
            }
        }

        [JsonProperty]
        public string TypeString { get; set; }
        [JsonProperty]
        public string PropertyString { get; set; }
        [JsonProperty]
        public Dictionary<T1, Guid> DictionaryToSerialize { get; set; }

        [JsonConstructor]
        public ReferenceDictionary()
        {

        }

        [OnSerializing]
        public void OnSerializing(StreamingContext streamingContext)
        {
            DictionaryToSerialize = new();
            base.Keys.ToList().ForEach(x => DictionaryToSerialize[x] = base[x]);
        }

        [OnDeserialized]
        public void OnDeserialized(StreamingContext streamingContext)
        {
            base.Clear();
            DictionaryToSerialize.Keys.ToList().ForEach(x => base[x] = DictionaryToSerialize[x]);
            DictionaryToSerialize = null;
            SetRefDictFromInfo();
        }

        private void SetRefDictFromInfo()
        {
            this.RefDict = (Dictionary<Guid, T2>)Type.GetType(this.TypeString).GetProperty(this.PropertyString).GetValue(null);
        }

        public ReferenceDictionary(Dictionary<Guid, T2> refDict, string refDictPropertyName, Type refDictClass = null)
        {
            if (refDictClass == null)
            {
                refDictClass = typeof(Global);
            }
            TypeString = refDictClass.FullName;
            PropertyString = refDictPropertyName;
            this.RefDict = refDict;
        }

        public new ReferenceDictionary<T1, T2> Add(T1 key, T2 value)
        {
            Guid id = GetItemID(value);
            this.RefDict[id] = value;
            base[key] = id;
            return this;
        }

        public void SetReferenceDict(Dictionary<Guid, T2> refDict)
        {
            this.RefDict = refDict;
        }

        public new T2 this[T1 key]
        {
            get
            {
                return RefDict[base[key]];
            }
            set
            {
                base[key] = GetItemID(value);
                RefDict[GetItemID(value)] = value;
            }
        }

        public new T2 Get(T1 key)
        {
            return RefDict[base[key]];
        }

        public new List<T2> Values
        {
            get
            {
                List<T2> toReturn = new List<T2>();
                foreach (Guid value in base.Values)
                {
                    toReturn.Add(RefDict[value]);
                }
                return toReturn;
            }
            private set
            {

            }
        }

        public new List<T1> Keys
        {
            get
            {
                return base.Keys.ToList();
            }
        }

        public new bool Contains(T1 key)
        {
            return base.ContainsKey(key);
        }

        public new bool TryGetValue(T1 key, [MaybeNullWhen(false)] out T2 value)
        {
            value = default(T2);
            if (!base.TryGetValue(key, out Guid key2))
            {
                return false;
            }
            if(!RefDict.TryGetValue(key2, out value))
            {
                return false;
            }
            return true;
        }

        private Guid GetItemID(T2 item)
        {
            Guid id;
            try
            {
                id = (Guid)item.GetType().GetProperty("Id").GetValue(item, null);
            }
            catch (Exception e)
            {
                throw new Exception("LibReferenceList could not find the Id property in given item. Please make sure items supplied to this list have the Id property set");
            }

            if (id == Guid.Empty)
            {
                throw new Exception("LibReferenceList Id property contains an empty Guid.");
            }
            return id;
        }
    }
}
