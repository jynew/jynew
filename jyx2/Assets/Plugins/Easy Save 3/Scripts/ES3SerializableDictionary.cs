using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ES3Internal
{
	[System.Serializable]
	public abstract class ES3SerializableDictionary<TKey,TVal> : Dictionary<TKey,TVal>, ISerializationCallbackReceiver
	{
		[SerializeField]
		private List<TKey> _Keys;
		[SerializeField]
		private List<TVal> _Values;

		protected abstract bool KeysAreEqual(TKey a, TKey b);
		protected abstract bool ValuesAreEqual(TVal a, TVal b);

		public void OnBeforeSerialize()
		{
			_Keys = new List<TKey>();
			_Values = new List<TVal>();
			foreach(KeyValuePair<TKey, TVal> pair in this)
			{
                try
                {
                    _Keys.Add(pair.Key);
                    _Values.Add(pair.Value);
                }
                catch { }
			}
		}

		// load dictionary from lists
		public void OnAfterDeserialize()
		{
            // There are some situations where Unity will not get the serialized data correctly, returning null.
            // In this case we don't want to change anything, otherwise we'll lose the data entirely.
            if (_Keys == null || _Values == null)
                return;

			if(_Keys.Count != _Values.Count)
				throw new System.Exception(string.Format("Key count is different to value count after deserialising dictionary."));

            this.Clear();

            for (int i = 0; i < _Keys.Count; i++)
			{
                if (_Keys[i] != null)
                {
                    try
                    {
                        this.Add(_Keys[i], _Values[i]);
                    }
                    catch { }
                }
			}

			_Keys = null;
			_Values = null;
		}
			
		public int RemoveNullValues()
		{
			var nullKeys = this.Where(pair => pair.Value == null)
				.Select(pair => pair.Key)
				.ToList();
			foreach (var nullKey in nullKeys)
				Remove(nullKey);
			return nullKeys.Count;
		}

		// Changes the key of a value without changing it's position in the underlying Lists.
		// Mainly used in the Editor where position might otherwise change while the user is editing it.
		// Returns true if a change was made.
		public bool ChangeKey(TKey oldKey, TKey newKey)
		{
			if(KeysAreEqual(oldKey, newKey))
				return false;

			var val = this [oldKey];
			Remove(oldKey);
			this [newKey] = val;
			return true;
		}
	}
}