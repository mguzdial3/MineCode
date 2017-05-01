using UnityEngine;
using System.Collections;

public class NPCBodyTexturer : MonoBehaviour {
	public Renderer hat, head, nose, body, legLeft, legRight;
	public Material[] materialList;	


	public void TextureNPC(string hatTexture, string headTexture, string noseTexture, string bodyTexture, string leftTexture, string rightTexture){
		string[] textureStrings = new string[]{hatTexture, headTexture, noseTexture, bodyTexture, leftTexture, rightTexture};
		Renderer[] renderers = new Renderer[]{hat, head, nose, body, legLeft, legRight};

		for(int i = 0; i<textureStrings.Length; i++){
			Material materialToUse = null;

			//Search through list for material to use
			foreach(Material m in materialList){
				if (m.name==textureStrings[i]){
					materialToUse = m;
					break;
				}
			}

			//If material does not exist
			if (materialToUse==null){
				//Turn off renderer
				renderers[i].enabled = false;
			}
			else{
				renderers[i].material = materialToUse;
			}
		}
	}
}
