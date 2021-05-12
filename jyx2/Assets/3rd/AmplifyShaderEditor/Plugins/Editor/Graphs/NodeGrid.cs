// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AmplifyShaderEditor
{
	public class NodeGrid
	{
		private bool m_debugGrid = false;
		private const float GRID_SIZE_X = 100;
		private const float GRID_SIZE_Y = 100;

		private const float GRID_AREA_X = 1000;
		private const float GRID_AREA_Y = 1000;

		private Dictionary<int, Dictionary<int, List<ParentNode>>> m_grid;

		private int m_xMin = int.MaxValue;
		private int m_yMin = int.MaxValue;

		private int m_xMax = int.MinValue;
		private int m_yMax = int.MinValue;

		public NodeGrid()
		{
			m_grid = new Dictionary<int, Dictionary<int, List<ParentNode>>>();
		}
		
		public void AddNodeToGrid( ParentNode node )
		{
			Rect pos = node.Position;
			if ( Mathf.Abs( pos.width ) < 0.001f || Mathf.Abs( pos.height ) < 0.001f )
			{
				return;
			}
			
			float initialXf = pos.x / GRID_SIZE_X;
			float initialYf = pos.y / GRID_SIZE_Y;

			int endX = Mathf.CeilToInt( initialXf + pos.width / GRID_SIZE_X );
			int endY = Mathf.CeilToInt( initialYf + pos.height / GRID_SIZE_Y );

			int initialX = Mathf.FloorToInt( initialXf );
			int initialY = Mathf.FloorToInt( initialYf );


			if ( initialX < m_xMin )
			{
				m_xMin = initialX;
			}

			if ( initialY < m_yMin )
			{
				m_yMin = initialY;
			}
			
			if ( endX > m_xMax )
			{
				m_xMax = endX;
			}

			if ( endY > m_yMax )
			{
				m_yMax = endY;
			}

			for ( int x = initialX; x < endX; x += 1 )
			{
				for ( int y = initialY; y < endY; y += 1 )
				{
					if ( !m_grid.ContainsKey( x ) )
					{
						m_grid.Add( x, new Dictionary<int, List<ParentNode>>() );

					}

					if ( !m_grid[ x ].ContainsKey( y ) )
					{
						m_grid[ x ].Add( y, new List<ParentNode>() );
					}

					m_grid[ x ][ y ].Add( node );
				}
			}
			node.IsOnGrid = true;
			//DebugLimits();
		}

		public void RemoveNodeFromGrid( ParentNode node, bool useCachedPos )
		{
			Rect pos = useCachedPos ? node.CachedPos : node.Position;
			if ( Mathf.Abs( pos.width ) < 0.001f || Mathf.Abs( pos.height ) < 0.001f )
			{
				return;
			}

			float initialXf = pos.x / GRID_SIZE_X;
			float initialYf = pos.y / GRID_SIZE_Y;

			int endX = Mathf.CeilToInt( initialXf + pos.width / GRID_SIZE_X );
			int endY = Mathf.CeilToInt( initialYf + pos.height / GRID_SIZE_Y );

			int initialX = Mathf.FloorToInt( initialXf );
			int initialY = Mathf.FloorToInt( initialYf );
			bool testLimits = false;

			int xMinCount = 0;
			int xMaxCount = 0;

			int yMinCount = 0;
			int yMaxCount = 0;


			for ( int x = initialX; x < endX; x += 1 )
			{
				for ( int y = initialY; y < endY; y += 1 )
				{
					if ( m_grid.ContainsKey( x ) )
					{
						if ( m_grid[ x ].ContainsKey( y ) )
						{
							m_grid[ x ][ y ].Remove( node );
							node.IsOnGrid = false;

							if ( initialX == m_xMin && x == initialX )
							{
								testLimits = true;
								if ( m_grid[ x ][ y ].Count != 0 )
								{
									xMinCount += 1;
								}
							}

							if ( endX == m_xMax && x == endX )
							{
								testLimits = true;
								if ( m_grid[ x ][ y ].Count != 0 )
								{
									xMaxCount += 1;
								}
							}

							if ( initialY == m_yMin && y == initialY )
							{
								testLimits = true;
								if ( m_grid[ x ][ y ].Count != 0 )
								{
									yMinCount += 1;
								}
							}

							if ( endY == m_yMax && y == endY )
							{
								testLimits = true;
								if ( m_grid[ x ][ y ].Count != 0 )
								{
									yMaxCount += 1;
								}
							}
						}
					}
				}
			}
			

			if ( testLimits )
			{
				if ( xMinCount == 0 || xMaxCount == 0 || yMinCount == 0 || yMaxCount == 0 )
				{
					m_xMin = int.MaxValue;
					m_yMin = int.MaxValue;

					m_xMax = int.MinValue;
					m_yMax = int.MinValue;
					foreach ( KeyValuePair<int, Dictionary<int, List<ParentNode>>> entryX in m_grid )
					{
						foreach ( KeyValuePair<int, List<ParentNode>> entryY in entryX.Value )
						{
							if ( entryY.Value.Count > 0 )
							{
								if ( entryX.Key < m_xMin )
								{
									m_xMin = entryX.Key;
								}

								if ( entryY.Key < m_yMin )
								{
									m_yMin = entryY.Key;
								}

								if ( entryX.Key > m_xMax )
								{
									m_xMax = entryX.Key;
								}

								if ( entryY.Key > m_yMax )
								{
									m_yMax = entryY.Key;
								}
							}
						}
					}
					// The += 1 is to maintain consistence with AddNodeToGrid() ceil op on max values
					m_xMax += 1;
					m_yMax += 1;
				}
			}
			//DebugLimits();
		}

		public void DebugLimits()
		{
			Debug.Log( "[ " + m_xMin + " , " + m_yMin + " ] " + "[ " + m_xMax + " , " + m_yMax + " ] " );
		}
		
		//pos must be the transformed mouse position to local canvas coordinates
		public List<ParentNode> GetNodesOn( Vector2 pos )
		{
			int x = Mathf.FloorToInt( pos.x / GRID_SIZE_X );
			int y = Mathf.FloorToInt( pos.y / GRID_SIZE_Y );

			if ( m_grid.ContainsKey( x ) )
			{
				if ( m_grid[ x ].ContainsKey( y ) )
				{
					return m_grid[ x ][ y ];
				}
			}

			return null;
		}

		public List<ParentNode> GetNodesOn( int x, int y )
		{
			if ( m_grid.ContainsKey( x ) )
			{
				if ( m_grid[ x ].ContainsKey( y ) )
				{
					return m_grid[ x ][ y ];
				}
			}
			return null;
		}

		public void DrawGrid( DrawInfo drawInfo )
		{
			if ( m_debugGrid )
			{
				Handles.CircleHandleCap( 0, drawInfo.InvertedZoom * ( new Vector3( drawInfo.CameraOffset.x, drawInfo.CameraOffset.y, 0f ) ), Quaternion.identity, 5,EventType.Layout );
				for ( int x = -( int ) GRID_AREA_X; x < GRID_AREA_X; x += ( int ) GRID_SIZE_X )
				{
					Handles.DrawLine( drawInfo.InvertedZoom * ( new Vector3( x + drawInfo.CameraOffset.x, drawInfo.CameraOffset.y - GRID_AREA_Y, 0 ) ), drawInfo.InvertedZoom * ( new Vector3( drawInfo.CameraOffset.x + x, drawInfo.CameraOffset.y + GRID_AREA_Y, 0 ) ) );
				}

				for ( int y = -( int ) GRID_AREA_Y; y < GRID_AREA_X; y += ( int ) GRID_SIZE_Y )
				{
					Handles.DrawLine( drawInfo.InvertedZoom * ( new Vector3( drawInfo.CameraOffset.x - GRID_AREA_X, drawInfo.CameraOffset.y + y, 0 ) ), drawInfo.InvertedZoom * ( new Vector3( drawInfo.CameraOffset.x + GRID_AREA_X, drawInfo.CameraOffset.y + y, 0 ) ) );
				}
			}
		}

		public void Destroy()
		{
			foreach ( KeyValuePair<int, Dictionary<int, List<ParentNode>>> entryX in m_grid )
			{
				foreach ( KeyValuePair<int, List<ParentNode>> entryY in entryX.Value )
				{
					entryY.Value.Clear();
				}
				entryX.Value.Clear();
			}
			m_grid.Clear();
		}

		public float MaxNodeDist
		{
			get { return Mathf.Max( ( m_xMax - m_xMin )*GRID_SIZE_X, ( m_yMax - m_yMin )*GRID_SIZE_Y ); }
		}
	}
}
