using UnityEngine;
using System.Collections;

public class LessonTwoGenerator : Generator {
	private const int caveSize = 10;
	private const int worldSize = 20;


	//Make the cave
	public override void GenerateMap (){
		for(int x = -worldSize; x<=worldSize; x++){
			for(int z = -worldSize; z<=worldSize; z++){
				if(Mathf.Abs(x)<=caveSize && Mathf.Abs(z)<=caveSize){
					int height = 0;

					if(Mathf.Abs(x)>4){
						height = Mathf.Abs(x)/2-1;
					}
					if(Mathf.Abs(z)>4){
						height = Mathf.Abs(z)/2-1;
					}

					if (caveSize==Mathf.Abs(x) || caveSize==Mathf.Abs(z)){
						height = 10;
					}

					while(height>-5){
						MapBuilderHelper.BuildBlock ("Stone2", x , height , z );
						height-=1;
					}

					MapBuilderHelper.BuildBlock ("Stone2", x , 10 , z );
				}
				else{
					//Dirt area outside of cave
					if(Mathf.Abs(x)<worldSize-2 && Mathf.Abs(z)<worldSize-2){
						MapBuilderHelper.BuildBlock ("Grass", x , 3 , z );
					}
					else{
						MapBuilderHelper.BuildBlock ("Grass", x , 2, z );
					}
				}
			}
		}

		//CreateDoor
		for(int xPlus = 0; xPlus<2; xPlus++){
			for(int yPlus = 0; yPlus<3; yPlus++){
				MapBuilderHelper.BuildBlock ("Door", -1+xPlus , 6-yPlus,10 );
			}
		}
	}	
}
