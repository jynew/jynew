// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
//
// Donated by BinaryCats
// https://forum.unity.com/threads/best-tool-asset-store-award-amplify-shader-editor-node-based-shader-creation-tool.430959/page-60#post-3414465
//////////////////////
// README / HOW TO USE
//////////////////////
// Examples:
// 
// Floats:
// 
// x Equals value
// EditableIf( _float1, Equalto, 1)
// This will allow the value to be edited, if the property _float1 is equal to 1. (_float1==1)
// Note: NotEqualTo is also a valid argument which will do the opposite of this example.EditableIf(_float1, NotEqualTo, 1)  (NotEqualTo != 1)
// 
// x Greater than value
// EditableIf(_Float1,GreaterThan,1)
// This will allow the value to be edited if  the property _float1 is Greater than 1. (_float1>1)
// 
// x Greater Than Or Equal to value
// EditableIf(_Float1,GreaterThanOrEqualTo,1)
// This will allow the value to be edited if  the property _float1 is Greater than or equal to 1. (_float1>=1)
// 
// 
// x Less Than value
// EditableIf(_Float1,LessThan,1)
// This will allow the value to be edited if  the property _float1 is Less than 1. (_float1<1)
// 
// x Less Than Or Equal to value
// EditableIf(_Float1,LessThanOrEqualTo,1)
// This will allow the value to be edited if  the property _float1 is Less than or equal to 1. (_float1<=1)
// 
// 
// Colour:
// 
// x Equals r,g,b,a
// EditableIf(_Color0,EqualTo,255,255,255,255)
// This will allow the value to be edited, if the property _Color0 R,G,B and A value all Equal 255. (_Color0.R==255 && _Color0.G==255 & _Color0.B == 255 && _Color0.A == 255)
// 
// x Equals alpha
// EditableIf(_Color0,EqualTo,null,null,null,255)
// This will allow the value to be edited, if the property _Color0 Alpha value is Equal to 255. (_Color0.A == 255)
// 
// a Greater than blue
// EditableIf(_Color0,GreaterThan,null,null,125)
// This will allow the value to be edited, if the property _Color0 Blue value is Greater Than 125. (_Color0.B > 125)
// Note: as I do not want to check the Red or Green Values, i have entered "null" for the parameter
// Note: I have not inputted a value to check for Alpha, as i do not want to check it. Simularly, if I wanted to Only check the Red Value I could have used EditableIf(_Color0,GreaterThan,125)
// 
// Like wise with floats GreaterThanOrEqualTo, LessThan, LessThanOrEqualTo
// 
// Vector:
// Vector Checks work the same as colour checks
// 
// Texture:
// x Does Not have a Texture
// EditableIf(_TextureSample0,Equals,null)
// This will allow the value to be edited, if the property _TextureSample0 does NOT have a texture
// 
// x Does have a Texture
// EditableIf(_TextureSample0,NotEqualTo,null)
// This will allow the value to be edited, if the property _TextureSample0 does have a texture

using UnityEngine;
using UnityEditor;
using System;

public enum ComparisonOperators
{
	EqualTo, NotEqualTo, GreaterThan, LessThan, EqualsOrGreaterThan, EqualsOrLessThan, ContainsFlags,
	DoesNotContainsFlags
}

public class EditableIf : MaterialPropertyDrawer
{
	ComparisonOperators op;
	string FieldName = "";
	object ExpectedValue;
	bool InputError;
	public EditableIf()
	{
		InputError = true;
	}
	public EditableIf( object fieldname, object comparison, object expectedvalue )
	{
		if( expectedvalue.ToString().ToLower() == "true" )
		{
			expectedvalue = (System.Single)1;
		}
		else if( expectedvalue.ToString().ToLower() == "false" )
		{
			expectedvalue = (System.Single)0;

		}
		Init( fieldname, comparison, expectedvalue );

	}
	public EditableIf( object fieldname, object comparison, object expectedvaluex, object expectedvaluey )
	{
		float? x = expectedvaluex as float?;
		float? y = expectedvaluey as float?;
		float? z = float.NegativeInfinity;
		float? w = float.NegativeInfinity;
		x = GetVectorValue( x );
		y = GetVectorValue( y );

		Init( fieldname, comparison, new Vector4( x.Value, y.Value, z.Value, w.Value ) );
	}
	public EditableIf( object fieldname, object comparison, object expectedvaluex, object expectedvaluey, object expectedvaluez )
	{
		float? x = expectedvaluex as float?;
		float? y = expectedvaluey as float?;
		float? z = expectedvaluez as float?;
		float? w = float.NegativeInfinity;
		x = GetVectorValue( x );
		y = GetVectorValue( y );
		z = GetVectorValue( z );

		Init( fieldname, comparison, new Vector4( x.Value, y.Value, z.Value, w.Value ) );

	}
	public EditableIf( object fieldname, object comparison, object expectedvaluex, object expectedvaluey, object expectedvaluez, object expectedvaluew )
	{
		var x = expectedvaluex as float?;
		var y = expectedvaluey as float?;
		var z = expectedvaluez as float?;
		var w = expectedvaluew as float?;
		x = GetVectorValue( x );
		y = GetVectorValue( y );
		z = GetVectorValue( z );
		w = GetVectorValue( w );

		Init( fieldname, comparison, new Vector4( x.Value, y.Value, z.Value, w.Value ) );

	}

	private void Init( object fieldname, object comparison, object expectedvalue )
	{
		FieldName = fieldname.ToString();
		var names = Enum.GetNames( typeof( ComparisonOperators ) );
		var name = comparison.ToString().ToLower().Replace( " ", "" );

		for( int i = 0; i < names.Length; i++ )
		{
			if( names[ i ].ToLower() == name )
			{
				op = (ComparisonOperators)i;
				break;
			}
		}

		ExpectedValue = expectedvalue;
	}

	private static float? GetVectorValue( float? x )
	{
		if( x.HasValue == false )
		{
			x = float.NegativeInfinity;
		}

		return x;
	}

	// Draw the property inside the given rect
	public override void OnGUI( Rect position, MaterialProperty prop, String label, MaterialEditor editor )
	{
		if( InputError )
		{
			EditorGUI.LabelField( position, "EditableIf Attribute Error: Input parameters are invalid!" );
			return;
		}
		var LHSprop = MaterialEditor.GetMaterialProperty( prop.targets, FieldName );
		if( string.IsNullOrEmpty( LHSprop.name ) )
		{
			LHSprop = MaterialEditor.GetMaterialProperty( prop.targets, "_" + FieldName.Replace( " ", "" ) );
			if( string.IsNullOrEmpty( LHSprop.name ) )
			{
				EditorGUI.LabelField( position, "EditableIf Attribute Error: " + FieldName + " Does not exist!" );
				return;
			}
		}
		object LHSVal = null;

		bool test = false;
		switch( LHSprop.type )
		{
			case MaterialProperty.PropType.Color:
			case MaterialProperty.PropType.Vector:
			LHSVal = LHSprop.type == MaterialProperty.PropType.Color ? (Vector4)LHSprop.colorValue : LHSprop.vectorValue;
			var v4 = ExpectedValue as Vector4?;
			v4 = v4.HasValue ? v4 : new Vector4( (System.Single)ExpectedValue, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity );

			if( LHSprop.type == MaterialProperty.PropType.Color )
			{
				test = VectorCheck( (Vector4)LHSVal, op, v4 / 255 );

			}
			else
				test = VectorCheck( (Vector4)LHSVal, op, v4 );
			break;
			case MaterialProperty.PropType.Range:
			case MaterialProperty.PropType.Float:
			LHSVal = LHSprop.floatValue;
			test = ( Check( LHSVal, op, ExpectedValue ) );
			break;
			case MaterialProperty.PropType.Texture:
			LHSVal = LHSprop.textureValue;
			test = ( CheckObject( LHSVal, op, ExpectedValue ) );
			break;
		}

		GUI.enabled = test;
		editor.DefaultShaderProperty( position, prop, label );
		GUI.enabled = true;
	}

	private bool VectorCheck( Vector4 LHS, ComparisonOperators op, object expectedValue )
	{
		var RHS = (Vector4)expectedValue;
		if( RHS.x != float.NegativeInfinity )
		{
			if( !Check( LHS.x, op, RHS.x ) )
				return false;
		}
		if( RHS.y != float.NegativeInfinity )
		{
			if( !Check( LHS.y, op, RHS.y ) )
				return false;
		}
		if( RHS.z != float.NegativeInfinity )
		{
			if( !Check( LHS.z, op, RHS.z ) )
				return false;
		}
		if( RHS.w != float.NegativeInfinity )
		{
			if( !Check( LHS.w, op, RHS.w ) )
				return false;
		}

		return true;
	}

	protected bool Check( object LHS, ComparisonOperators op, object RHS )
	{
		if( !( LHS is IComparable ) || !( RHS is IComparable ) )
			throw new Exception( "Check using non basic type" );

		switch( op )
		{
			case ComparisonOperators.EqualTo:
			return ( (IComparable)LHS ).CompareTo( RHS ) == 0;

			case ComparisonOperators.NotEqualTo:
			return ( (IComparable)LHS ).CompareTo( RHS ) != 0;

			case ComparisonOperators.EqualsOrGreaterThan:
			return ( (IComparable)LHS ).CompareTo( RHS ) >= 0;

			case ComparisonOperators.EqualsOrLessThan:
			return ( (IComparable)LHS ).CompareTo( RHS ) <= 0;

			case ComparisonOperators.GreaterThan:
			return ( (IComparable)LHS ).CompareTo( RHS ) > 0;

			case ComparisonOperators.LessThan:
			return ( (IComparable)LHS ).CompareTo( RHS ) < 0;
			case ComparisonOperators.ContainsFlags:
			return ( (int)LHS & (int)RHS ) != 0; // Dont trust LHS values, it has been casted to a char and then to an int again, first bit will be the sign
			case ComparisonOperators.DoesNotContainsFlags:
			return ( ( (int)LHS & (int)RHS ) == (int)LHS ); // Dont trust LHS values, it has been casted to a char and then to an int again, first bit will be the sign

			default:
			break;
		}
		return false;
	}
	private bool CheckObject( object LHS, ComparisonOperators comparasonOperator, object RHS )
	{
		switch( comparasonOperator )
		{
			case ComparisonOperators.EqualTo:
			return ( LHS == null );

			case ComparisonOperators.NotEqualTo:
			return ( LHS != null );
		}
		return true;
	}

}
