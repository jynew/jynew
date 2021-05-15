// Animancer // https://kybernetik.com.au/animancer // Copyright 2021 Kybernetik //

#if UNITY_EDITOR

using UnityEditor;

namespace Animancer.Editor
{
    /// <summary>[Editor-Only] A wrapper around a <see cref="SerializedProperty"/> representing an array field.</summary>
    public sealed class SerializedArrayProperty
    {
        /************************************************************************************************************************/

        private SerializedProperty _Property;

        /// <summary>The target property.</summary>
        public SerializedProperty Property
        {
            get => _Property;
            set
            {
                _Property = value;
                Refresh();
            }
        }

        /************************************************************************************************************************/

        private int _Count;

        /// <summary>The cached <see cref="SerializedProperty.arraySize"/> of the <see cref="Property"/>.</summary>
        public int Count
        {
            get => _Count;
            set => Property.arraySize = _Count = value;
        }

        /************************************************************************************************************************/

        private bool _HasMultipleDifferentValues;
        private bool _GotHasMultipleDifferentValues;

        /// <summary>The cached <see cref="SerializedProperty.hasMultipleDifferentValues"/> of the <see cref="Property"/>.</summary>
        public bool HasMultipleDifferentValues
        {
            get
            {
                if (!_GotHasMultipleDifferentValues)
                {
                    _GotHasMultipleDifferentValues = true;
                    _HasMultipleDifferentValues = Property.hasMultipleDifferentValues;
                }

                return _HasMultipleDifferentValues;
            }
        }

        /************************************************************************************************************************/

        /// <summary>Updates the cached <see cref="Count"/> and <see cref="HasMultipleDifferentValues"/>.</summary>
        public void Refresh()
        {
            _Count = _Property != null ? _Property.arraySize : 0;
            _GotHasMultipleDifferentValues = false;
        }

        /************************************************************************************************************************/

        /// <summary>Calls <see cref="SerializedProperty.GetArrayElementAtIndex"/> on the <see cref="Property"/>.</summary>
        /// <remarks>
        /// Returns null if the element is not actually a child of the <see cref="Property"/>, which can happen
        /// if multiple objects are selected with different array sizes.
        /// </remarks>
        public SerializedProperty GetElement(int index)
        {
            var element = Property.GetArrayElementAtIndex(index);
            if (!HasMultipleDifferentValues || element.propertyPath.StartsWith(Property.propertyPath))
                return element;
            else
                return null;
        }

        /************************************************************************************************************************/
    }
}

#endif

