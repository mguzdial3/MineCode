using UnityEngine;

public class AdventureGameGenerator : Generator {
	
	//This function is called to generate out the map
	public override void GenerateMap (){
		BuildRectangle (0, 0, 0, 20, 20, "Grass");
	}

	void BuildRectangle(int x, int y, int z, int sizeX, int sizeY, string stringVal){
		int bX = 0; 
		while (bX < sizeX) {
			int bZ = 0;
			while (bZ < sizeY) {
				MapBuilderHelper.BuildBlock (stringVal, bX + x, y, bZ + z);
				bZ = bZ + 1; 
			}
			bX = bX + 1; 
		}
	}

}

