using UnityEngine;
using System.Collections;

public class MysteryGenerator : Generator {

	//This function is called to generate out the map
	public override void GenerateMap (){
		//Floor
		BuildCube ("Trunk", -10, 0, -10, 60, 1, 20);
		//Wall1
		BuildCube("StackStone", -10, 0, -10, 60,10,1);
		//Wall2
		BuildCube("StackStone", 50, 0, -10, 1,10,20);
		//Wall3
		BuildCube("StackStone", -10, 0, -10, 1,10,20);
		//Wall4
		BuildCube("StackStone", -10, 0, 10, 60,10,1);
		//Ceiling
		BuildCube("StackStone", -10, 10, -10, 60, 1, 20);

		//Wall between main area and rooms
		BuildCube("StackStone", 5, 3, -10, 1,7,20);
		BuildCube("StackStone", 5, 1, -10, 1,3,9);
		BuildCube("StackStone", 5, 1, 2, 1,3,9);

		//Bookshelves
		BuildCube("Bookshelf", -9, 1, -9, 14,6,1);
		BuildCube("Bookshelf", -9, 1, -9, 1,6,18);
		BuildCube("Bookshelf", -9, 1, 9, 14,6,1);

		//Rooms
		BuildCube("StackStone", 6, 3, -3, 44,6,1);//Room Wall
		BuildCube("StackStone", 6, 3, 3, 44,6,1);//Room Wall
		BuildCube("StackStone", 19, 1, -9, 1,8,6);
		BuildCube("StackStone", 33, 1, -9, 1,8,6);

		BuildCube("StackStone", 19, 1, 4, 1,8,6);
		BuildCube("StackStone", 33, 1, 4, 1,8,6);

		//Door Walls
		BuildCube("StackStone", 9, 1, -3, 19,2,1);
		BuildCube("StackStone", 9, 1, 3, 19,2,1);

		BuildCube("StackStone", 30, 1, -3, 17,2,1);
		BuildCube("StackStone", 30, 1, 3, 17,2,1);

		BuildCube("Bookshelf", 38, 1, -6, 3,2,3);

	}

	public void BuildCube(string type, int x, int y, int z, int sizeX, int sizeY, int sizeZ){
		for (int xi = 0; xi < sizeX; xi++) {
			for (int yi = 0; yi < sizeY; yi++) {
				for (int zi = 0; zi < sizeZ; zi++) {
					MapBuilderHelper.BuildBlock (type, x + xi, y + yi, z + zi);
				}
			}
		}
	}
}
