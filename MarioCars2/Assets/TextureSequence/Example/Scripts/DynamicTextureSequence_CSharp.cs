using UnityEngine;
using System.Collections;

public class DynamicTextureSequence_CSharp : MonoBehaviour 
{
	public Texture2D[] pictures;
	
	void OnGUI()
	{
		if (GUI.Button(new Rect(5, 5, 300, 30), "Create dynamic texture sequence C#"))
		{
			GameObject go = new GameObject("Grandfather");
			go.transform.localScale = new Vector3(0, 0, 1);
			go.AddComponent<GUITexture>();
			
			Rect layout = new Rect(Random.Range(0, Screen.width - 120), Random.Range(0, Screen.height - 120), 120, 120);
			go.guiTexture.pixelInset = layout;
			/*TextureSequence ts = go.AddComponent<TextureSequence>();
			ts.target = TextureSequenceTarget.guiTexture;
			ts.duration = 1.5f;
			ts.SetTextures(pictures);
			ts.Play();*/
			TextureSequence.CreateAndPlay(go, pictures, 1.5f, 1, TextureSequenceLoop.loop);
		}
	}
}
