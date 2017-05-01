using UnityEngine;
using System.Collections;

//Singleton to help with building up map for the lessons (since Map.Instance is confusing)
public class MapBuilderHelper : MonoBehaviour {

	//Builds a block of the specified name `block' at location x, y, z
	public static void BuildBlock(string block, int x, int y, int z){
		Block b = Map.Instance.GetBlockSet ().GetBlock (block);

		//Default behavior
		if (b == null) {
			b = Map.Instance.GetBlockSet ().GetBlock ("Grass");
		}

		Map.Instance.SetBlockNoSave (new BlockData(b), x, y, z);
	}

	public static bool IsPositionOpen(float x, float y, float z){
		return Map.Instance.IsPositionOpen(new Vector3i(x,y,z));
	}
}
