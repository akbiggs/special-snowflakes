//Before using, put the script "TextureSequence.cs" in "Standard Assets" folder

/*#pragma strict

var pictures:Texture[];

function OnGUI()
{
	if (GUI.Button(new Rect(5, 45, 300, 30), "Create dynamic texture sequence JS"))
	{
		var go:GameObject = new GameObject("Grandfather");
		go.transform.localScale = new Vector3(0, 0, 1);
		go.AddComponent("GUITexture");
		
		var layout:Rect = new Rect(Random.Range(0, Screen.width - 120), Random.Range(0, Screen.height - 120), 120, 120);
		go.guiTexture.pixelInset = layout;
		TextureSequence.CreateAndPlay(go, pictures, 1.5f, 1, TextureSequenceLoop.loop);
	}
}*/