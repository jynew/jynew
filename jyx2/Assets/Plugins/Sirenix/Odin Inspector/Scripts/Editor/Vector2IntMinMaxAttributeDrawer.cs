//-----------------------------------------------------------------------
// <copyright file="Vector2IntMinMaxAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

#if UNITY_EDITOR && UNITY_2017_2_OR_NEWER

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Sirenix.OdinInspector;
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Draws Vector2Int properties marked with <see cref="MinMaxSliderAttribute"/>.
    /// </summary>
    public class Vector2IntMinMaxAttributeDrawer : OdinAttributeDrawer<MinMaxSliderAttribute, Vector2Int>
    {
        private string errorMessage;

        private InspectorPropertyValueGetter<int> intMinGetter;
        private InspectorPropertyValueGetter<float> floatMinGetter;

        private InspectorPropertyValueGetter<int> intMaxGetter;
        private InspectorPropertyValueGetter<float> floatMaxGetter;

        private InspectorPropertyValueGetter<Vector2Int> vector2IntMinMaxGetter;

        /// <summary>
        /// Initializes the drawer by resolving any optional references to members for min/max value.
        /// </summary>
        protected override void Initialize()
        {
            MemberInfo member;

            // Min member reference.
            if (this.Attribute.MinMember != null)
            {
                if (MemberFinder.Start(this.Property.ParentType)
                    .IsNamed(this.Attribute.MinMember)
                    .HasNoParameters()
                    .TryGetMember(out member, out this.errorMessage))
                {
                    var type = member.GetReturnType();
                    if (type == typeof(int))
                    {
                        this.intMinGetter = new InspectorPropertyValueGetter<int>(this.Property, this.Attribute.MinMember);
                    }
                    else if (type == typeof(float))
                    {
                        this.floatMinGetter = new InspectorPropertyValueGetter<float>(this.Property, this.Attribute.MinMember);
                    }
                }
            }

            // Max member reference.
            if (this.Attribute.MaxMember != null)
            {
                if (MemberFinder.Start(this.Property.ParentType)
                    .IsNamed(this.Attribute.MaxMember)
                    .HasNoParameters()
                    .TryGetMember(out member, out this.errorMessage))
                {
                    var type = member.GetReturnType();
                    if (type == typeof(int))
                    {
                        this.intMaxGetter = new InspectorPropertyValueGetter<int>(this.Property, this.Attribute.MaxMember);
                    }
                    else if (type == typeof(float))
                    {
                        this.floatMaxGetter = new InspectorPropertyValueGetter<float>(this.Property, this.Attribute.MaxMember);
                    }
                }
            }

            // Min max member reference.
            if (this.Attribute.MinMaxMember != null)
            {
                this.vector2IntMinMaxGetter = new InspectorPropertyValueGetter<Vector2Int>(this.Property, this.Attribute.MinMaxMember);
                if (this.errorMessage != null)
                {
                    this.errorMessage = this.vector2IntMinMaxGetter.ErrorMessage;
                }
            }
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            // Get the range of the slider from the attribute or from member references.
            Vector2 range;
            if (this.vector2IntMinMaxGetter != null && this.errorMessage == null)
            {
                range = (Vector2)this.vector2IntMinMaxGetter.GetValue();
            }
            else
            {
                if (this.intMinGetter != null)
                {
                    range.x = this.intMinGetter.GetValue();
                }
                else if (this.floatMinGetter != null)
                {
                    range.x = this.floatMinGetter.GetValue();
                }
                else
                {
                    range.x = this.Attribute.MinValue;
                }

                if (this.intMaxGetter != null)
                {
                    range.y = this.intMaxGetter.GetValue();
                }
                else if (this.floatMaxGetter != null)
                {
                    range.y = this.floatMaxGetter.GetValue();
                }
                else
                {
                    range.y = this.Attribute.MaxValue;
                }
            }

            // Display evt. error message.
            if (this.errorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.errorMessage);
            }

            EditorGUI.BeginChangeCheck();
            Vector2 value = SirenixEditorFields.MinMaxSlider(label, (Vector2)this.ValueEntry.SmartValue, range, this.Attribute.ShowFields);
            if (EditorGUI.EndChangeCheck())
            {
                this.ValueEntry.SmartValue = new Vector2Int((int)value.x, (int)value.y);
            }
        }
    }
}
#endif // UNITY_EDITOR && UNITY_2017_2_OR_NEWER