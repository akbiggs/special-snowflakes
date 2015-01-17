/*       INFINITY CODE 2013         */
/*   http://www.infinity-code.com   */

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(TextureSequence))]
public class TextureSequenceEditor : Editor 
{
	private readonly string[] aviableTextureNames = new string[]{"_MainTex", "_BumpMap", "_ParallaxMap", "_DecalTex", "_Illum", "_LightMap", "_Mask"};
	private readonly string[] aviableTextureTitles = new string[]{"Base Texture", "Normal Map", "Parallax Map", "Decal Texture", "Illumin", "Light Map", "Culling Mask"};
	
	public TextureSequence seq;
	
	private GUIStyle errorStyle
	{
		get 
		{
			GUIStyle style = new GUIStyle();
			style.normal.textColor = Color.red;
			return style;
		}
	}
	
	private string[] FindExternalTextures()
	{
		List<string> textureNames = new List<string>();
		if (seq.texturePath == "") { Debug.Log("Path is not specified"); return textureNames.ToArray(); }
		DirectoryInfo targetDir = new DirectoryInfo(seq.texturePath);
		if (!targetDir.Exists) { Debug.Log("Path does not exist"); return textureNames.ToArray(); }
		FileInfo[] files = targetDir.GetFiles();
		foreach(FileInfo file in files)
		{
			string ext = file.Extension.ToLower().Substring(1);
			if (ext == "png" || ext == "jpg" || ext == "jpeg" || ext == "bmp" || ext == "gif" || ext == "tga" || ext == "psd" || ext == "tif" || ext == "tiff") textureNames.Add(file.Name);
		}
		return textureNames.ToArray();
	}
	
	private string[] FindTextures()
	{
		List<string> textureNames = new List<string>();
		string resPath = Path.Combine(Application.dataPath, "Resources");
		if (!(new DirectoryInfo(resPath).Exists)) { Debug.Log("Resources directory does not exist"); return textureNames.ToArray(); }
		string targetPath = Path.Combine(resPath, seq.texturePath);
		DirectoryInfo targetDir = new DirectoryInfo(targetPath);
		if (!targetDir.Exists) { Debug.Log("Path in Resources does not exist"); return textureNames.ToArray(); }
		FileInfo[] files = targetDir.GetFiles();
		foreach(FileInfo file in files)
		{
			string ext = file.Extension.ToLower().Substring(1);
			if (ext == "png" || ext == "jpg" || ext == "jpeg" || ext == "bmp" || ext == "gif" || ext == "tga" || ext == "psd" || ext == "tif" || ext == "tiff") textureNames.Add(file.Name.Substring(0, file.Name.LastIndexOf(".")));
		}
		return textureNames.ToArray();
	}
	
	private string[] GetAviableTextureName()
	{
		List<string> avTxt = new List<string>();
		Material mat = seq.targetObject.renderer.sharedMaterials[seq.materialID];
		for (int i = 0; i < aviableTextureNames.Length; i++) if (mat.HasProperty(aviableTextureNames[i])) avTxt.Add(aviableTextureNames[i]);
		return avTxt.ToArray();
	}
	
	private string[] GetAviableTextureTitle()
	{
		List<string> avTxt = new List<string>();
		Material mat = seq.targetObject.renderer.sharedMaterials[seq.materialID];
		for (int i = 0; i < aviableTextureNames.Length; i++) if (mat.HasProperty(aviableTextureNames[i])) avTxt.Add(aviableTextureTitles[i]);
		return avTxt.ToArray();
	}
	
	private int GetTextureID(string[] avTxt, string selTxt)
	{
		for (int i = 0; i < avTxt.Length; i++) if (avTxt[i] == selTxt) return i;
		return 0;
	}
	
	void OnDrawAtlasGUI()
	{
		Texture2D newTexture = (Texture2D)EditorGUILayout.ObjectField("Atlas: ", seq.atlasTexture, typeof(Texture2D), false);
		
		if (newTexture != seq.atlasTexture)
		{
			seq.atlasTexture = newTexture;
			if (seq.atlasTextureOriginalSize == Vector2.zero && newTexture != null) seq.atlasTextureOriginalSize = new Vector2(newTexture.width, newTexture.height);
		}
		
		if (seq.atlasTexture != null && !TextureSequence.isReadableTexture(seq.atlasTexture))
		{
			GUILayout.Box("Texture not readable.\nGo to Texture import settings.\nSet 'Texture type' - 'Advanced' and enable 'Read / Write Enabled'.", GUILayout.ExpandWidth(true));
		}
		
		seq.atlasSettings = (TextAsset)EditorGUILayout.ObjectField("Settings (XML): ", seq.atlasSettings, typeof(TextAsset), true);
		seq.atlasTextureOriginalSize = EditorGUILayout.Vector2Field("Original image size: ", seq.atlasTextureOriginalSize);
	}
	
	void OnDrawSetImage()
	{
		if (seq.loadTextureTarget == TextureSequenceLoadTarget.none) return;
		
		Texture2D texture = null;
		
		if (GUILayout.Button("Set first image to target"))
		{
			if (seq.loadTextureTarget == TextureSequenceLoadTarget.atlas && seq.atlasTexture != null && TextureSequence.isReadableTexture(seq.atlasTexture) && seq.atlasSettings != null && seq.atlasTextureOriginalSize != Vector2.zero)
			{
				 texture = seq.GetTextureFromAtlas(0);
			}
			else if (seq.loadTextureTarget == TextureSequenceLoadTarget.manual && seq.length > 0)
			{
				texture = (Texture2D)seq.textures[0];
			}
			else if (seq.loadTextureTarget == TextureSequenceLoadTarget.fromResourcesFolder)
			{
				string[] ts = FindTextures();
				if (ts.Length > 0) texture = seq.GetTextureByPath(seq.texturePath, ts[0]);
			}
			else if (seq.loadTextureTarget == TextureSequenceLoadTarget.fromExternalFolder)
			{
				string[] ts = FindExternalTextures();
				if (ts.Length > 0) 
				{
					texture = new Texture2D(32, 32);
					texture.LoadImage(seq.GetFileByExternalPath(seq.texturePath, ts[0]));
				}
			}
			
			if (texture != null)
			{
				if (seq.target == TextureSequenceTarget.guiTexture) seq.targetObject.guiTexture.texture = texture;
				else if (seq.target == TextureSequenceTarget.material) seq.renderer.sharedMaterials[seq.materialID].SetTexture(seq.textureName, texture);
			}
		}
	}
	
	void OnDrawSounds()
	{
		EditorGUILayout.Space();
		
		seq.soundStartType = (TextureSequenceSoundStart) EditorGUILayout.EnumPopup("Sounds sync: ", seq.soundStartType);
		
		if (seq.soundStartType != TextureSequenceSoundStart.none)
		{
			int index = 1;
			foreach(TextureSequenceSound snd in seq.sounds)
			{
				GUILayout.BeginHorizontal();
				
				GUILayout.Label(index.ToString(), GUILayout.ExpandWidth(false));
				snd.clip = (AudioClip) EditorGUILayout.ObjectField(snd.clip, typeof(AudioClip), false);
				if (GUILayout.Button("Del", GUILayout.ExpandWidth(false)))
				{
					List<TextureSequenceSound> newSounds = new List<TextureSequenceSound>(seq.sounds);
					newSounds.Remove(snd);
					seq.sounds = newSounds.ToArray();
					return;
				}
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				
				GUILayout.Space(20);
				
				if (seq.soundStartType == TextureSequenceSoundStart.byFrame) snd.frame = EditorGUILayout.IntField("Frame: ", snd.frame);
				else if (seq.soundStartType == TextureSequenceSoundStart.byTime) snd.time = EditorGUILayout.FloatField("Time: ", snd.time);
				
				GUILayout.EndHorizontal();
			}
			
			if (GUILayout.Button("Add sound"))
			{
				List<TextureSequenceSound> newSounds = new List<TextureSequenceSound>(seq.sounds);
				newSounds.Add(new TextureSequenceSound());
				seq.sounds = newSounds.ToArray();
			}
		}
	}
	
	void OnDrawTargetGUI()
	{
		seq.target = (TextureSequenceTarget) EditorGUILayout.EnumPopup("Target: ", seq.target);
		if (seq.target == TextureSequenceTarget.guiTexture && seq.targetObject.guiTexture == null && seq.targetObject.renderer) seq.target = TextureSequenceTarget.material;
		if (seq.target == TextureSequenceTarget.material && seq.targetObject.renderer == null && seq.targetObject.guiTexture) seq.target = TextureSequenceTarget.guiTexture;
		
		if (seq.target == TextureSequenceTarget.material)
		{
			if (seq.targetObject.renderer != null)
			{
				seq.materialID = EditorGUILayout.IntField("Material ID: ", seq.materialID);
				if (seq.materialID < 0) seq.materialID = 0;
				else if (seq.materialID >= seq.targetObject.renderer.sharedMaterials.Length) seq.materialID = seq.targetObject.renderer.sharedMaterials.Length - 1;
				
				string[] avTxtT = GetAviableTextureTitle();
				string[] avTxtN = GetAviableTextureName();
				int selID = EditorGUILayout.Popup("Texture Name: ", GetTextureID(avTxtN, seq.textureName), avTxtT);
				seq.textureName = avTxtN[selID];
			}
			else 
			{
				#if OLD_LABELFIELD
					GUILayout.Label("Need Renderer Component", errorStyle);
				#else
					EditorGUILayout.LabelField("Need Renderer Component", errorStyle);
				#endif
			}
		}
		else if (seq.target == TextureSequenceTarget.guiTexture)
		{
			if (seq.targetObject.guiTexture == null) 
			{
				EditorGUILayout.LabelField("Need GUITexture Component", errorStyle);
			}
		}
	}
	
	void OnEnable()
	{
		seq = (TextureSequence)target;
	}
	
	public override void OnInspectorGUI()
	{
		if (!seq.hardLoadTextureTarget && seq.loadTextureTarget == TextureSequenceLoadTarget.none && !EditorApplication.isPlaying) seq.loadTextureTarget = TextureSequenceLoadTarget.fromResourcesFolder;
		
		seq.targetObject = (GameObject)EditorGUILayout.ObjectField("Target Object: ", seq.targetObject, typeof(GameObject), true);
		
		if (seq.targetObject == null) seq.targetObject = seq.gameObject;
	
		TextureSequenceLoadTarget newLoadTarget = (TextureSequenceLoadTarget)EditorGUILayout.EnumPopup("Textures: ", seq.loadTextureTarget);
		bool isNewLoadTarget = newLoadTarget != seq.loadTextureTarget;
		if (isNewLoadTarget) 
		{ 
			seq.loadTextureTarget = newLoadTarget; seq.hardLoadTextureTarget = true; 
			if (isNewLoadTarget) seq.textureFiles = new string[0];
		}
		
		if (seq.loadTextureTarget == TextureSequenceLoadTarget.fromResourcesFolder || seq.loadTextureTarget == TextureSequenceLoadTarget.fromExternalFolder) seq.loadType = (TextureSequenceLoadType)EditorGUILayout.EnumPopup("Load: ", seq.loadType);
	
		if (seq.loadTextureTarget == TextureSequenceLoadTarget.fromResourcesFolder) 
		{
			string path = EditorGUILayout.TextField("Path in Resources: ", seq.texturePath);
			if (seq.loadType == TextureSequenceLoadType.atShow)
			{
				if (path != seq.texturePath) seq.textureFiles = new string[0];
				if (GUILayout.Button("Find textures")) seq.textureFiles = FindTextures();
				GUILayout.Label("Number of textures: " + seq.textureFiles.Length.ToString());
			}
			seq.texturePath = path;
		}
		else if (seq.loadTextureTarget == TextureSequenceLoadTarget.fromExternalFolder)
		{
			string path = EditorGUILayout.TextField("Path: ", seq.texturePath);
			if (path != seq.texturePath)
			{
				seq.textureFiles = new string[0];
				seq.texturePath = path;
			}
			if (GUILayout.Button("Find textures")) seq.textureFiles = FindExternalTextures();
			GUILayout.Label("Number of Textures: " + seq.textureFiles.Length.ToString());
		}
		else if (seq.loadTextureTarget == TextureSequenceLoadTarget.manual) OnManualTextures();
		else if (seq.loadTextureTarget == TextureSequenceLoadTarget.atlas) OnDrawAtlasGUI();
		
		EditorGUILayout.Space();
		
		seq.duration = EditorGUILayout.FloatField("Duration (sec): ", seq.duration);
		if (seq.duration < 0) seq.duration = 0;
		seq.speed = EditorGUILayout.FloatField("Speed: ", seq.speed);
		if (seq.loadTextureTarget != TextureSequenceLoadTarget.none) seq.autoStart = EditorGUILayout.Toggle("Autostart: ", seq.autoStart);
		seq.loop = (TextureSequenceLoop)EditorGUILayout.EnumPopup("Loop: ", seq.loop);
		if (seq.loop == TextureSequenceLoop.none) seq.autoDestroy = EditorGUILayout.Toggle("AutoDestroy: ", seq.autoDestroy);
		
		OnDrawTargetGUI();
		OnDrawSetImage();
		OnDrawSounds();
	}
	
	private void OnManualTextures()
	{
		Object texture;
		List<Object> textures = new List<Object>();
		if (seq.textures == null) seq.textures = new Object[0];
		for (int i = 0; i < seq.textures.Length; i++)
		{
			texture = EditorGUILayout.ObjectField((i + 1).ToString() + ": ", seq.textures[i], typeof(Texture), false);
			
			if (texture != null) textures.Add(texture);
		}
		
		texture = EditorGUILayout.ObjectField((seq.textures.Length + 1).ToString() + ": ", null, typeof(Texture), false);
		
		if (texture != null) textures.Add(texture);
		
		seq.textures = textures.ToArray();
	}
}
