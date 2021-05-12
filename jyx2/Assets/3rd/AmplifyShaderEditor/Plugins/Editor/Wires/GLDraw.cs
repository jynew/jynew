using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	public class GLDraw
	{
		/*
		* Clipping code: http://forum.unity3d.com/threads/17066-How-to-draw-a-GUI-2D-quot-line-quot?p=230386#post230386
		* Thick line drawing code: http://unifycommunity.com/wiki/index.php?title=VectorLine
		*/
		public static Material LineMaterial = null;
		public static bool MultiLine = false;
		private static Shader LineShader = null;
		private static Rect BoundBox = new Rect();
		private static Vector3[] Allv3Points = new Vector3[] { };
		private static Vector2[] AllPerpendiculars = new Vector2[] { };
		private static Color[] AllColors = new Color[] { };
		private static Vector2 StartPt = Vector2.zero;
		private static Vector2 EndPt = Vector2.zero;

		private static Vector3 Up = new Vector3( 0, 1, 0 );
		private static Vector3 Zero = new Vector3( 0, 0, 0 );

		private static Vector2 Aux1Vec2 = Vector2.zero;

		private static int HigherBoundArray = 0;

		public static void CreateMaterial()
		{
			if( (object)LineMaterial != null && (object)LineShader != null )
				return;

			LineShader = AssetDatabase.LoadAssetAtPath<Shader>( AssetDatabase.GUIDToAssetPath( "50fc796413bac8b40aff70fb5a886273" ) );
			LineMaterial = new Material( LineShader );

			LineMaterial.hideFlags = HideFlags.HideAndDontSave;
		}

		public static void DrawCurve( Vector3[] allPoints, Vector2[] allNormals, Color[] allColors, int pointCount )
		{
			CreateMaterial();
			LineMaterial.SetPass( ( MultiLine ? 1 : 0 ) );

			GL.Begin( GL.TRIANGLE_STRIP );
			for( int i = 0; i < pointCount; i++ )
			{
				GL.Color( allColors[ i ] );
				GL.TexCoord( Zero );
				GL.Vertex3( allPoints[ i ].x - allNormals[ i ].x, allPoints[ i ].y - allNormals[ i ].y, 0 );
				GL.TexCoord( Up );
				GL.Vertex3( allPoints[ i ].x + allNormals[ i ].x, allPoints[ i ].y + allNormals[ i ].y, 0 );
			}
			GL.End();

		}

		public static Rect DrawBezier( Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent, Color color, float width, int type = 1 )
		{
			int segments = Mathf.FloorToInt( ( start - end ).magnitude / 20 ) * 3; // Three segments per distance of 20
			return DrawBezier( start, startTangent, end, endTangent, color, width, segments, type );
		}

		public static Rect DrawBezier( Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent, Color color, float width, int segments, int type = 1 )
		{
			return DrawBezier( start, startTangent, end, endTangent, color, color, width, segments, type );
		}

		public static Rect DrawBezier( Vector2 start, Vector2 startTangent, Vector2 end, Vector2 endTangent, Color startColor, Color endColor, float width, int segments, int type = 1 )
		{
			int pointsCount = segments + 1;
			int linesCount = segments;

			HigherBoundArray = HigherBoundArray > pointsCount ? HigherBoundArray : pointsCount;

			Allv3Points = Handles.MakeBezierPoints( start, end, startTangent, endTangent, pointsCount );
			if( AllColors.Length < HigherBoundArray )
			{
				AllColors = new Color[ HigherBoundArray ];
				AllPerpendiculars = new Vector2[ HigherBoundArray ];
			}

			startColor.a = ( type * 0.25f );
			endColor.a = ( type * 0.25f );

			float minX = Allv3Points[ 0 ].x;
			float minY = Allv3Points[ 0 ].y;
			float maxX = Allv3Points[ 0 ].x;
			float maxY = Allv3Points[ 0 ].y;

			float amount = 1 / (float)linesCount;
			for( int i = 0; i < pointsCount; i++ )
			{
				if( i == 0 )
				{
					AllColors[ 0 ] = startColor;
					StartPt.Set( startTangent.y, start.x );
					EndPt.Set( start.y, startTangent.x );
				}
				else if( i == pointsCount - 1 )
				{
					AllColors[ pointsCount - 1 ] = endColor;
					StartPt.Set( end.y, endTangent.x );
					EndPt.Set( endTangent.y, end.x );
				}
				else
				{
					AllColors[ i ] = Color.LerpUnclamped( startColor, endColor, amount * i );

					minX = ( Allv3Points[ i ].x < minX ) ? Allv3Points[ i ].x : minX;
					minY = ( Allv3Points[ i ].y < minY ) ? Allv3Points[ i ].y : minY;
					maxX = ( Allv3Points[ i ].x > maxX ) ? Allv3Points[ i ].x : maxX;
					maxY = ( Allv3Points[ i ].y > maxY ) ? Allv3Points[ i ].y : maxY;
					StartPt.Set( Allv3Points[ i + 1 ].y, Allv3Points[ i - 1 ].x );
					EndPt.Set( Allv3Points[ i - 1 ].y, Allv3Points[ i + 1 ].x );
				}
				Aux1Vec2.Set( StartPt.x - EndPt.x, StartPt.y - EndPt.y );
				FastNormalized( ref Aux1Vec2 );
				//aux1Vec2.FastNormalized();
				Aux1Vec2.Set( Aux1Vec2.x * width, Aux1Vec2.y * width );
				AllPerpendiculars[ i ] = Aux1Vec2;
			}

			BoundBox.Set( minX, minY, ( maxX - minX ), ( maxY - minY ) );

			DrawCurve( Allv3Points, AllPerpendiculars, AllColors, pointsCount );
			return BoundBox;
		}

		private static void FastNormalized( ref Vector2 v )
		{
			float len = Mathf.Sqrt( v.x * v.x + v.y * v.y );
			v.Set( v.x / len, v.y / len );
		}

		public static void Destroy()
		{
			GameObject.DestroyImmediate( LineMaterial );
			LineMaterial = null;

			Resources.UnloadAsset( LineShader );
			LineShader = null;
		}
	}

	//public static class VectorEx
	//{
	//	public static void FastNormalized( this Vector2 v )
	//	{
	//		float len = Mathf.Sqrt( v.x * v.x + v.y * v.y );
	//		v.Set( v.x / len, v.y / len );
	//	}
	//}
}

