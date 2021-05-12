/* TILE WORLD CREATOR
 * Copyright (c) 2015 doorfortyfour OG
 * 
 * Create awesome tile worlds in seconds.
 *
 *
 * Documentation: http://tileworldcreator.doofortyfour.com
 * Like us on Facebook: http://www.facebook.com/doorfortyfour2013
 * Web: http://www.doorfortyfour.com
 * Contact: mail@doorfortyfour.com Contact us for help, bugs or 
 * share your awesome work you've made with TileWorldCreator
*/

using UnityEngine;
using System.Collections;


namespace TileWorld
{
	
	public class TileWorldNeighbourCounter : MonoBehaviour 
	{


		public static int CountCrossNeighbours(bool[,] map, int _x, int _y, int _step, bool _createFloor, bool invert)
		{

			TileWorldCreator.orientation = "";
			int _count = 0;
			
			for(int y = -_step; y < _step + 1; y ++)
			{
				for(int x = -_step; x < _step + 1; x ++)
				{
					
					int neighbour_x = _x + x;
					int neighbour_y = _y + y;
					
					//do not count middle point
					if (x == 0 && y == 0)
					{
						
					}
					else if ((x == -_step && y == 0) || (x == _step && y == 0) || (x == 0 && y == -_step) || (x == 0 && y == _step))
					{
						//off the edge of the map
						if(neighbour_x < 0 || neighbour_y < 0 || neighbour_x >= map.GetLength(0) || neighbour_y >= map.GetLength(1))
						{
							_count = _count + 1;
							
							
							//north
							if (x == 0 && y == -_step)
							{
								TileWorldCreator.orientation += "n";
							}
								//west
							if (x == -_step && y == 0)
							{
								TileWorldCreator.orientation += "w";
							}
								//east
							if (x == _step && y == 0)
							{
								TileWorldCreator.orientation += "e";
							}
								//south
							if (x == 0 && y == _step)
							{
								TileWorldCreator.orientation += "s";
							}
						}
						//build island
						else if (!invert)
						{
							//if floor is deactive do not count floor cells
							if (!_createFloor && map[neighbour_x, neighbour_y])
							{
								_count = _count + 1;
								
								//set the orientation according to where a neighbour is
								//and where we are looking
								
								//north
								if (x == 0 && y == -_step)
								{
									TileWorldCreator.orientation += "n";
								}
								//west
								if (x == -_step && y == 0)
								{
									TileWorldCreator.orientation += "w";
								}
								//east
								if (x == _step && y == 0)
								{
									TileWorldCreator.orientation += "e";
								}
								//south
								if (x == 0 && y == _step)
								{
									TileWorldCreator.orientation += "s";
								}
							}
							//count floor cells
							if (_createFloor && !map[neighbour_x, neighbour_y])
							{
								_count = _count + 1;
								
								//set the orientation according to where a neighbour is
								//and where we are looking
								
								//north
								if (x == 0 && y == -_step)
								{
									TileWorldCreator.orientation += "n";
								}
								//west
								if (x == -_step && y == 0)
								{
									TileWorldCreator.orientation += "w";
								}
								//east
								if (x == _step && y == 0)
								{
									TileWorldCreator.orientation += "e";
								}
								//south
								if (x == 0 && y == _step)
								{
									TileWorldCreator.orientation += "s";
								}
							}
						}
						//build dungeon
						else if (invert)
						{
							//do not count floor if deactivated
							if (!_createFloor && !map[neighbour_x, neighbour_y])
							{
								_count = _count + 1;
								
								//set the orientation according to where a neighbour is
								//and where we are looking
								
								//north
								if (x == 0 && y == -_step)
								{
									TileWorldCreator.orientation += "n";
								}
								//west
								if (x == -_step && y == 0)
								{
									TileWorldCreator.orientation += "w";
								}
								//east
								if (x == _step && y == 0)
								{
									TileWorldCreator.orientation += "e";
								}
								//south
								if (x == 0 && y == _step)
								{
									TileWorldCreator.orientation += "s";
								}
							}
							
							//count floor cells
							if (_createFloor && map[neighbour_x, neighbour_y])
							{
								_count = _count + 1;
								
								//set the orientation according to where a neighbour is
								//and where we are looking
								
								//north
								if (x == 0 && y == -_step)
								{
									TileWorldCreator.orientation += "n";
								}
								//west
								if (x == -_step && y == 0)
								{
									TileWorldCreator.orientation += "w";
								}
								//east
								if (x == _step && y == 0)
								{
									TileWorldCreator.orientation += "e";
								}
								//south
								if (x == 0 && y == _step)
								{
									TileWorldCreator.orientation += "s";
								}
							}
						}
						
					}	
					
				}
			}
			return _count;
		}
		
		public static int CountDiagonalNeighbours(bool[,] map, int _x, int _y, int _range, bool _createFloor, bool invert)
		{
			TileWorldCreator.orientation = "";
			int _count = 0;
			
			for(int y = -_range; y < _range + 1; y ++)
			{
				for(int x = -_range; x < _range + 1; x ++)
				{
					
					int neighbour_x = _x + x;
					int neighbour_y = _y + y;
					
					//do not count middle point
					if (x == 0 && y == 0)
					{
						
					}
					else if ((x == -_range && y == _range) || (x == _range && y == -_range) || (x == -_range && y == -_range) || (x == _range && y == _range))
					{
						//off the edge of the map
						if(neighbour_x < 0 || neighbour_y < 0 || neighbour_x >= map.GetLength(0) || neighbour_y >= map.GetLength(1))
						{
						}
						else if (!invert)
						{
							//if floor is deactive do not count floor cells
							if (!_createFloor && map[neighbour_x, neighbour_y])
							{
								_count ++;
								
								//north west
								if (x == -1 && y == 1)
								{
									TileWorldCreator.orientation += "nw";
								}
								
								//north east
								if (x == 1 && y == 1)
								{
									TileWorldCreator.orientation += "ne";
								}
								
								//south west
								if (x == -1 && y == -1)
								{
									TileWorldCreator.orientation += "sw";
								}
								
								//south east
								if (x == 1 && y == -1)
								{
									TileWorldCreator.orientation += "se";
								}
								
							}
							//count floor cells
							if (_createFloor && !map[neighbour_x, neighbour_y])
							{
								_count ++;
								
								//north west
								if (x == -1 && y == 1)
								{
									TileWorldCreator.orientation += "nw";
								}
								
								//north east
								if (x == 1 && y == 1)
								{
									TileWorldCreator.orientation += "ne";
								}
								
								//south west
								if (x == -1 && y == -1)
								{
									TileWorldCreator.orientation += "sw";
								}
								
								//south east
								if (x == 1 && y == -1)
								{
									TileWorldCreator.orientation += "se";
								}
							}
							
						}
						else if (invert)
						{
							////do not count floor if deactivated
							if (!_createFloor && !map[neighbour_x, neighbour_y])
							{
								_count ++;
								
									//north west
								if (x == -1 && y == 1)
								{
									TileWorldCreator.orientation += "nw";
								}
								
									//north east
								if (x == 1 && y == 1)
								{
									TileWorldCreator.orientation += "ne";
								}
								
									//south west
								if (x == -1 && y == -1)
								{
									TileWorldCreator.orientation += "sw";
								}
								
									//south east
								if (x == 1 && y == -1)
								{
									TileWorldCreator.orientation += "se";
								}
								
							}
							
							//count floor cells
							if (_createFloor && map[neighbour_x, neighbour_y])
							{
								_count ++;
								
								//north west
								if (x == -1 && y == 1)
								{
									TileWorldCreator.orientation += "nw";
								}
								
								//north east
								if (x == 1 && y == 1)
								{
									TileWorldCreator.orientation += "ne";
								}
								
								//south west
								if (x == -1 && y == -1)
								{
									TileWorldCreator.orientation += "sw";
								}
								
								//south east
								if (x == 1 && y == -1)
								{
									TileWorldCreator.orientation += "se";
								}
							}
						}
						
					}
					
				}
				
			}
			
			return _count;
		}
		
		public static int CountAllNeighbours(bool[,] map, int _x, int _y, int _range, bool _invert)
		{
			int _count = 0;
			for(int y = -_range; y < _range + 1; y ++)
			{
				for(int x = -_range; x < _range + 1; x ++)
				{
					
					int neighbour_x = _x + x;
					int neighbour_y = _y + y;
					
	            	//looking at middle point do nothing
					if(y == 0 && x == 0)
					{
						
					}
	        		//off the edge of the map
					else if(neighbour_x < 0 || neighbour_y < 0 || neighbour_x >= map.GetLength(0) || neighbour_y >= map.GetLength(1))
					{
						_count = _count + 1;		
					}
					
	            	//normal check of the neighbour
					else if (map[neighbour_x, neighbour_y] == _invert)
					{			
						_count = _count + 1;
					}
				}
			}
			return _count;
		}
		
		
		//Count inner terrain blocks
		//--------------------------
		//count inner terrain blocks according to
		//range / distance from the edge of the world
		//does not differ much from count all neighbours
		//only have to check if invert is on or off
		public static int CountInnerTerrainBlocks(bool [,] map, int _x, int _y, int _range, bool _invert)
		{
			int _count = 0;
			TileWorldCreator.orientation = "";
			for(int y = -_range; y < _range + 1; y ++)
			{
				for(int x = -_range; x < _range + 1; x ++)
				{
					
					int neighbour_x = _x + x;
					int neighbour_y = _y + y;
					
				//do nothing when checking ourselves
					if(y == 0 && x == 0)
					{
						
					} 
				//In case the index we're looking at it is off the edge of the map
					else if(neighbour_x < 0 || neighbour_y < 0 || neighbour_x >= map.GetLength(0) || neighbour_y >= map.GetLength(1))
					{
						_count ++;
					}
					
					else if (!_invert && !map[neighbour_x, neighbour_y])
					{
						_count ++;
					}
					else if (_invert && map[neighbour_x, neighbour_y])
					{
						_count ++;
						
					}
					
				}
				
			}
			return _count;
		}
	}
	
}
