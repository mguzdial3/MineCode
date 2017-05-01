using UnityEngine;
using System.Collections;

public class PlatformerGenerator : Generator {

	public override void GenerateMap (){
		BuildCube ("Grass", -2, -2, -2, 4, 1, 4);//Platform

		BuildCube ("Grass", 4, -2, -2, 4, 1, 4);//Platform

		BuildCube ("Grass", 10, -2, -2, 4, 1, 4);//Platform

		BuildCube ("Grass", 16, -2, -2, 4, 1, 4);//Platform

		BuildCube ("Grass", 24, -2, -2, 4, 1, 4);//Platform

		BuildCube ("Grass", 32, -8, -2, 4, 1, 4);//Platform

		BuildCube ("Grass", 40, -8, -2, 4, 1, 4);//Platform

		BuildCube ("Grass", 48, -8, -2, 4, 1, 4);//Platform

		BuildCube ("Grass", 56, -8, -2, 4, 1, 4);//Platform

		BuildCube ("Grass", 66, -14, -2, 4, 1, 4);//Platform

		BuildCube ("Grass", 76, -20, -2, 4, 1, 4);//Platform

		BuildCube ("Grass", 88, -30, -2, 10, 1, 4);//Platform
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
