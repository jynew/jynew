using System;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	public class UpperLeftWidgetHelper
	{
		public int DrawWidget( ParentNode owner, int selectedIndex, GUIContent[] displayedOptions )
		{
			if( owner.DropdownEditing )
			{
				int newValue = owner.EditorGUIPopup( owner.DropdownRect, selectedIndex, displayedOptions, UIUtils.PropertyPopUp );
				if( newValue != selectedIndex )
				{
					owner.DropdownEditing = false;
				}
				return newValue;
			}
			return selectedIndex;
		}

		public int DrawWidget( ParentNode owner, int selectedIndex, string[] displayedOptions )
		{
			if( owner.DropdownEditing )
			{
				int newValue = owner.EditorGUIPopup( owner.DropdownRect, selectedIndex, displayedOptions, UIUtils.PropertyPopUp );
				if( newValue != selectedIndex )
				{
					owner.DropdownEditing = false;
				}
				return newValue;
			}
			return selectedIndex;
		}

		public int DrawWidget( ParentNode owner, int selectedIndex, string[] displayedOptions, int[] optionValues )
		{
			if( owner.DropdownEditing )
			{
				int newValue = owner.EditorGUIIntPopup( owner.DropdownRect, selectedIndex, displayedOptions, optionValues, UIUtils.PropertyPopUp );
				if( newValue != selectedIndex )
				{
					owner.DropdownEditing = false;
				}
				return newValue;
			}
			return selectedIndex;
		}

		// GC free version
		public void DrawWidget<TEnum>( ref TEnum selectedIndex, ParentNode owner, Action<ParentNode> callback ) where TEnum : struct
		{
			if( owner.DropdownEditing )
			{
				Enum asEnumType = selectedIndex as Enum;
				if( asEnumType != null )
				{
					EditorGUI.BeginChangeCheck();
					selectedIndex = ( owner.EditorGUIEnumPopup( owner.DropdownRect, asEnumType, UIUtils.PropertyPopUp ) as TEnum? ).Value;
					if( EditorGUI.EndChangeCheck() )
					{
						owner.DropdownEditing = false;
						if( callback != null )
							callback( owner );
					}
				}
			}
		}

		/* 
		 * USE THIS OVERRIDE IN CASE THE NODE DOESN'T HAVE PREVIEW
		 */
		//public override void AfterCommonInit()
		//{
		//	base.AfterCommonInit();
		//	if( PaddingTitleLeft == 0 )
		//	{
		//		PaddingTitleLeft = Constants.PropertyPickerWidth + Constants.IconsLeftRightMargin;
		//		if( PaddingTitleRight == 0 )
		//			PaddingTitleRight = Constants.PropertyPickerWidth + Constants.IconsLeftRightMargin;
		//	}
		//}


		/* 
		 * USE THE SOURCE CODE BELOW INTO THE NODE YOU WANT THE WIDGET TO SHOW
		 */
		//private UpperLeftWidgetHelper m_upperLeftWidget = new UpperLeftWidgetHelper();
	}
}
