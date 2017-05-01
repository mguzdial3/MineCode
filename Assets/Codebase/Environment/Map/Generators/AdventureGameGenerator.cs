using UnityEngine;

public class AdventureGameGenerator : Generator {
	
	//This function is called to generate out the map
	public override void GenerateMap (){
		MapBuilderHelper.BuildBlock ("Grass", 0, 0, 0);
	}

}

