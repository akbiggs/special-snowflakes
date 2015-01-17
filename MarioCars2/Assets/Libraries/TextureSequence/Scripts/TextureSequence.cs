/*       INFINITY CODE 2013         */
/*   http://www.infinity-code.com   */
/*                                  */
/*        Texture Sequence          */
/*          Version 1.3.0           */

using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Xml;

[AddComponentMenu("Infinity Code/Texture Sequence")]
public class TextureSequence : MonoBehaviour 
{
	#region events
	
	
	/// <summary>
	/// Occurs when frames ended.
	/// </summary>
	public event OnCompleteHandler OnComplete;
	public delegate void OnCompleteHandler();
	
	/// <summary>
	/// Occurs when on loop or pingpong.
	/// </summary>
	public event OnLoopHandler OnLoop;
	public delegate void OnLoopHandler();
	
	/// <summary>
	/// Occurs each time the updated frame.
	/// </summary>
	public event OnUpdateHandler OnUpdate;
	public delegate void OnUpdateHandler();
	
	#endregion
	
	#region public variables
	
	/// <summary>
	/// XML, as used in Starking or generated in Adobe Flash.
	/// </summary>
	public TextAsset atlasSettings;
	
	/// <summary>
	/// Not used. Reserve for future developments.
	/// </summary>
	public TextureSequenceAtlasType atlasSettingsType = TextureSequenceAtlasType.Starling;
	
	/// <summary>
	/// Texture atlas.
	/// </summary>
	public Texture2D atlasTexture;
	
	/// <summary>
	/// The original size of the image.
	/// </summary>
	public Vector2 atlasTextureOriginalSize;
	
	/// <summary>
	/// Automatic destroy component after complete.
	/// </summary>
	public bool autoDestroy = false;
	
	/// <summary>
	/// Autostart on enter playmode.
	/// </summary>
	public bool autoStart = true;
	
	/// <summary>
	/// The duration of all frames in second.
	/// </summary>
	public float duration = 1;
	
	/// <summary>
	/// Folder name in "Resources" folder.
	/// </summary>
	public string texturePath;
	
	/// <summary>
	/// Do not change.
	/// </summary>
	public bool hardLoadTextureTarget = false;
	
	/// <summary>
	/// Specifies when to load the texture.
	/// </summary>
	public TextureSequenceLoadType loadType = TextureSequenceLoadType.atStart;
	
	/// <summary>
	/// Type load images: from resources, manual or none.
	/// </summary>
	public TextureSequenceLoadTarget loadTextureTarget = TextureSequenceLoadTarget.none;
	
	/// <summary>
	/// Repeat type. None, loop or PingPong.
	/// </summary>
	public TextureSequenceLoop loop = TextureSequenceLoop.loop;
	
	/// <summary>
	/// Material ID, which will receive the image.
	/// </summary>
	public int materialID = 0;
	
	/// <summary>
	/// Sync type sounds.
	/// </summary>
	public TextureSequenceSoundStart soundStartType = TextureSequenceSoundStart.none;
	
	/// <summary>
	/// Sounds.
	/// </summary>
	public TextureSequenceSound[] sounds;
	
	/// <summary>
	/// The factor of time. May be zoro or negative.
	/// </summary>
	public float speed = 1;
	
	/// <summary>
	/// Target. Can be: GUITexture or Material.
	/// </summary>
	public TextureSequenceTarget target = TextureSequenceTarget.guiTexture;
	
	/// <summary>
	/// Target gameobject. 
	/// </summary>
	public GameObject targetObject;
	
	/// <summary>
	/// Texture name, which will be applied to the image.
	/// </summary>
	public string textureName = "_MainTex";
	
	/// <summary>
	/// Textures list.
	/// </summary>
	public Object[] textures;
	
	/// <summary>
	/// List of files that will be loaded texture.
	/// </summary>
	public string[] textureFiles;
	
	/// <summary>
	/// [Read only] Returns the texture of the current frame.
	/// </summary>
	public Texture2D activeTexture
	{
		get 
		{
			if (loadType == TextureSequenceLoadType.atShow && (loadTextureTarget == TextureSequenceLoadTarget.fromResourcesFolder || loadTextureTarget == TextureSequenceLoadTarget.fromExternalFolder)) return _activeTexture;
			return (Texture2D) textures[_frame]; 
		}
	}
	
	/// <summary>
	/// Current frame (0 to length - 1). 
	/// </summary>
	public int frame
	{
		get { return _frame; }
		set 
		{
			if (loop == TextureSequenceLoop.loop) _frame = Mathf.RoundToInt(Mathf.Repeat(value, length));
			else if (loop == TextureSequenceLoop.pingPong)
			{
				if (value < 0) { speed = Mathf.Abs(speed); DispatchLoop(); }
				else if (value >= length - 1) { speed = -Mathf.Abs(speed); DispatchLoop(); }
				
				_frame = Mathf.RoundToInt(Mathf.PingPong(value, length));
			}
			else if (value >= length - 1 && speed > 0) 
			{ 
				_frame = length - 1; 
				if (isPlayed) DispatchComplete(); 
				isPlayed = false; 
			}
			else if (value <= 0 && speed < 0) 
			{ 
				_frame = 0; 
				if (isPlayed) DispatchComplete(); 
				isPlayed = false; 
			}
			else _frame = value;
			currentTime = _frame * timeStep;
		}
	}
	
	/// <summary>
	/// [Read only] Number of images.
	/// </summary>
	public int length
	{
		get 
		{ 
			if (loadType == TextureSequenceLoadType.atShow && (loadTextureTarget == TextureSequenceLoadTarget.fromResourcesFolder || loadTextureTarget == TextureSequenceLoadTarget.fromExternalFolder)) return (textureFiles != null)? textureFiles.Length: 0;
			return (textures != null)? textures.Length: 0; 
		}
	}
	
	/// <summary>
	/// [Read only] Current time.
	/// </summary>
	public float time
	{
		get { return currentTime; }
	}
	
	/// <summary>
	/// [Read only] Progress (0 to 1)
	/// </summary>
	public float progress
	{
		get 
		{ 
			if (length == 0) return 0;
			return (float)_frame / length; 
		}
	}
	
	#endregion
	
	#region private variables
	private Texture2D _activeTexture;
	private int _frame = 0;
	private float currentTime = 0;
	private bool isPlayed = false;
	private bool loaded = false;
	private float timeStep;
	#endregion
	
	#region public functions
	
	/// <summary>
	/// Gets the texture from current atlas by index.
	/// </summary>
	/// <returns>
	/// The texture from atlas.
	/// </returns>
	/// <param name='textureIndex'>
	/// Texture index.
	/// </param>
	public Texture2D GetTextureFromAtlas(uint textureIndex)
	{
		if (atlasTexture == null || atlasSettings == null || atlasTextureOriginalSize == Vector2.zero) return null;
		return GetTextureFromAtlas(atlasTexture, atlasSettings, atlasTextureOriginalSize, textureIndex);
	}
	
	/// <summary>
	/// Gets the texture from current atlas by name.
	/// </summary>
	/// <returns>
	/// The texture from atlas.
	/// </returns>
	/// <param name='textureName'>
	/// Texture name.
	/// </param>
	public Texture2D GetTextureFromAtlas(string textureName)
	{
		if (atlasTexture == null || atlasSettings == null || atlasTextureOriginalSize == Vector2.zero) return null;
		return GetTextureFromAtlas(atlasTexture, atlasSettings, atlasTextureOriginalSize, textureName);
	}
	
	/// <summary>
	/// Load textures from resource folder.
	/// </summary>
	/// <param name='path'>
	/// Path in "Resources".
	/// </param>
	public void Load(string path)
	{
		if (path == null) return;
		
		textures = Resources.LoadAll(path, typeof(Texture2D));
		
		if (textures.Length == 0) return;
		
		loaded = true;
		loadTextureTarget = TextureSequenceLoadTarget.fromResourcesFolder;
		currentTime = 0;
		_frame = 0;
		
		if (speed < 0) currentTime = duration;
	}
	
	/// <summary>
	/// Loads a texture, and starts playing.
	/// </summary>
	/// <param name='path'>
	/// Path in "Resources".
	/// </param>
	public void LoadAndPlay(string path)
	{
		Load(path);
		Play();
	}
	
	/// <summary>
	/// Loads a texture, and starts playing the specified frame.
	/// </summary>
	/// <param name='path'>
	/// Path in "Resources".
	/// </param>
	/// <param name='FRAME'>
	/// The frame on which to start playback.
	/// </param>
	public void LoadAndPlay(string path, int FRAME)
	{
		Load(path);
		Play(FRAME);
	}
	
	/// <summary>
	/// Loads a texture, sets the duration and speed, and starts playback.
	/// </summary>
	/// <param name='path'>
	/// Path in "Resources".
	/// </param>
	/// <param name='Duration'>
	/// Duration in seconds.
	/// </param>
	/// <param name='Speed'>
	/// Speed.
	/// </param>
	public void LoadAndPlay(string path, float Duration, float Speed)
	{
		duration = Duration;
		speed = Speed;
		LoadAndPlay(path);
	}
	
	/// <summary>
	/// Loads a texture, sets the duration, speed and loop, and starts playback.
	/// </summary>
	/// <param name='path'>
	/// Path in "Resources".
	/// </param>
	/// <param name='Duration'>
	/// Duration in seconds.
	/// </param>
	/// <param name='Speed'>
	/// Speed.
	/// </param>
	/// <param name='Loop'>
	/// Loop type.
	/// </param>
	public void LoadAndPlay(string path, float Duration, float Speed, TextureSequenceLoop Loop)
	{
		loop = Loop;
		LoadAndPlay(path, Duration, Speed);
	}
	
	/// <summary>
	/// Start playing back images.
	/// </summary>
	public void Play()
	{
		if (length == 0 || duration <= 0) return;
		
		if (speed > 0 && frame == length - 1) frame = 0;
		else if (speed < 0 && frame == 0) frame = length - 1;
		isPlayed = true;
	}
	
	/// <summary>
	/// Start playing back images with the specified frame.
	/// </summary>
	/// <param name='FRAME'>
	/// The frame on which to start playback.
	/// </param>
	public void Play(int FRAME)
	{
		if (length == 0 || duration <= 0) return;
		
		frame = FRAME;
		isPlayed = true;
	}
	
	/// <summary>
	/// Set texture atlas.
	/// </summary>
	/// <param name='texture'>
	/// Texture atlas.
	/// </param>
	/// <param name='settings'>
	/// Atlas settings.
	/// </param>
	/// <param name='originalTextureSize'>
	/// Original texture size.
	/// </param>
	public void SetAtlas(Texture2D texture, TextAsset settings, Vector2 originalTextureSize)
	{
		atlasTexture = texture;
		atlasSettings = settings;
		atlasTextureOriginalSize = originalTextureSize;
		
		textures = ConvertAtlasToTextures(texture, settings, originalTextureSize);
		if (length > 0) 
		{
			loaded = true;
			loadTextureTarget = TextureSequenceLoadTarget.atlas;
		}
	}
	
	/// <summary>
	/// Loads settings atlas from "Resources" folder and sets the texture atlas
	/// </summary>
	/// <param name='texture'>
	/// Texture atlas.
	/// </param>
	/// <param name='settingsPath'>
	/// Atlas settings file in "Resources" folder.
	/// </param>
	/// <param name='originalTextureSize'>
	/// Original texture size.
	/// </param>
	public void SetAtlas(Texture2D texture, string settingsPath, Vector2 originalTextureSize)
	{
		TextAsset settings = (TextAsset) Resources.Load(settingsPath);
		SetAtlas(texture, settings, originalTextureSize);
	}
	
	/// <summary>
	/// Sets the texture atlas, and starts playback.
	/// </summary>
	/// <param name='texture'>
	/// Texture atlas.
	/// </param>
	/// <param name='settings'>
	/// Atlas settings.
	/// </param>
	/// <param name='originalTextureSize'>
	/// Original texture size.
	/// </param>
	public void SetAtlasAndPlay(Texture2D texture, TextAsset settings, Vector2 originalTextureSize)
	{
		SetAtlas(texture, settings, originalTextureSize);
		Play();
	}
	
	/// <summary>
	/// Sets the texture atlas, duration, speed, and starts playback.
	/// </summary>
	/// <param name='texture'>
	/// Texture atlas.
	/// </param>
	/// <param name='settings'>
	/// Atlas settings.
	/// </param>
	/// <param name='originalTextureSize'>
	/// Original texture size.
	/// </param>
	/// <param name='Duration'>
	/// Duration in seconds.
	/// </param>
	/// <param name='Speed'>
	/// Speed.
	/// </param>
	public void SetAtlasAndPlay(Texture2D texture, TextAsset settings, Vector2 originalTextureSize, float Duration, float Speed)
	{
		duration = Duration;
		speed = Speed;
		SetAtlasAndPlay(texture, settings, originalTextureSize);
	}
	
	/// <summary>
	/// Sets the texture atlas, duration, speed, loop, and starts playback.
	/// </summary>
	/// <param name='texture'>
	/// Texture atlas.
	/// </param>
	/// <param name='settings'>
	/// Atlas settings.
	/// </param>
	/// <param name='originalTextureSize'>
	/// Original texture size.
	/// </param>
	/// <param name='Duration'>
	/// Duration in seconds.
	/// </param>
	/// <param name='Speed'>
	/// Speed.
	/// </param>
	/// <param name='Loop'>
	/// Loop type.
	/// </param>
	public void SetAtlasAndPlay(Texture2D texture, TextAsset settings, Vector2 originalTextureSize, float Duration, float Speed, TextureSequenceLoop Loop)
	{
		loop = Loop;
		SetAtlasAndPlay(texture, settings, originalTextureSize, Duration, Speed);
	}
	
	/// <summary>
	/// Loads settings atlas of resources folder and sets the texture atlas, and starts playback.
	/// </summary>
	/// <param name='texture'>
	/// Texture atlas.
	/// </param>
	/// <param name='settingsPath'>
	/// Atlas settings file in "Resources" folder.
	/// </param>
	/// <param name='originalTextureSize'>
	/// Original texture size.
	/// </param>
	public void SetAtlasAndPlay(Texture2D texture, string settingsPath, Vector2 originalTextureSize)
	{
		SetAtlas(texture, settingsPath, originalTextureSize);
		Play();
	}
	
	/// <summary>
	/// Loads settings atlas of resources folder and sets the texture atlas, duration, speed, and starts playback.
	/// </summary>
	/// <param name='texture'>
	/// Texture atlas.
	/// </param>
	/// <param name='settingsPath'>
	/// Settings file in "Resources" folder.
	/// </param>
	/// <param name='originalTextureSize'>
	/// Original texture size.
	/// </param>
	/// <param name='Duration'>
	/// Duration in seconds.
	/// </param>
	/// <param name='Speed'>
	/// Speed.
	/// </param>
	public void SetAtlasAndPlay(Texture2D texture, string settingsPath, Vector2 originalTextureSize, float Duration, float Speed)
	{
		duration = Duration;
		speed = Speed;
		SetAtlasAndPlay(texture, settingsPath, originalTextureSize);
	}
	
	/// <summary>
	/// Loads settings atlas of resources folder and sets the texture atlas, duration, speed, loop, and starts playback.
	/// </summary>
	/// <param name='texture'>
	/// Texture atlas.
	/// </param>
	/// <param name='settingsPath'>
	/// Settings file in "Resources" folder.
	/// </param>
	/// <param name='originalTextureSize'>
	/// Original texture size.
	/// </param>
	/// <param name='Duration'>
	/// Duration in seconds.
	/// </param>
	/// <param name='Speed'>
	/// Speed.
	/// </param>
	/// <param name='Loop'>
	/// Loop type.
	/// </param>
	public void SetAtlasAndPlay(Texture2D texture, string settingsPath, Vector2 originalTextureSize, float Duration, float Speed, TextureSequenceLoop Loop)
	{
		loop = Loop;
		SetAtlasAndPlay(texture, settingsPath, originalTextureSize, Duration, Speed);
	}
	
	/// <summary>
	/// Set textures list.
	/// </summary>
	/// <param name='newTextures'>
	/// New textures array.
	/// </param>
	public void SetTextures(Object[] newTextures)
	{
		textures = newTextures;
		loaded = textures != null && textures.Length > 0;
		if (loaded)
		{
			loadTextureTarget = TextureSequenceLoadTarget.none;
			currentTime = 0;
			_frame = 0;
		}
	}
	
	/// <summary>
	/// Set textures list, and starts playback.
	/// </summary>
	/// <param name='newTextures'>
	/// New textures array.
	/// </param>
	public void SetTexturesAndPlay(Object[] newTextures)
	{
		SetTextures(newTextures);
		Play();
	}
	
	/// <summary>
	/// Set textures list, duration, speed, and starts playback.
	/// </summary>
	/// <param name='newTextures'>
	/// New textures array.
	/// </param>
	/// <param name='Duration'>
	/// Duration in seconds.
	/// </param>
	/// <param name='Speed'>
	/// Speed.
	/// </param>
	public void SetTexturesAndPlay(Object[] newTextures, float Duration, float Speed)
	{
		duration = Duration;
		speed = Speed;
		SetTexturesAndPlay(newTextures);
	}
	
	/// <summary>
	/// Set textures list, duration, speed, loop, and starts playback.
	/// </summary>
	/// <param name='newTextures'>
	/// New textures array.
	/// </param>
	/// <param name='Duration'>
	/// Duration in seconds.
	/// </param>
	/// <param name='Speed'>
	/// Speed.
	/// </param>
	/// <param name='Loop'>
	/// Loop type.
	/// </param>
	public void SetTexturesAndPlay(Object[] newTextures, float Duration, float Speed, TextureSequenceLoop Loop)
	{
		loop = Loop;
		SetTexturesAndPlay(newTextures, Duration, Speed);
	}
	
	/// <summary>
	/// Stop playing back images.
	/// </summary>
	public void Stop()
	{
		isPlayed = false;
	}
	
	#endregion
	
	#region public static functions
	
	/// <summary>
	/// Converts the texture atlas to texture array.
	/// </summary>
	/// <returns>
	/// Texture array.
	/// </returns>
	/// <param name='atlasTexture'>
	/// Texture atlas.
	/// </param>
	/// <param name='atlasSettings'>
	/// Atlas settings.
	/// </param>
	/// <param name='originalImageSize'>
	/// Original image size.
	/// </param>
	public static Texture2D[] ConvertAtlasToTextures(Texture2D atlasTexture, TextAsset atlasSettings, Vector2 originalImageSize)
	{
		XmlElement fnode = GetAtlasSettingsFirstElement(atlasTexture, atlasSettings, originalImageSize);
		Vector2 scaler = new Vector2(atlasTexture.width / originalImageSize.x, atlasTexture.height / originalImageSize.y);
		
		List<Texture2D> newTextures = new List<Texture2D>();
		foreach(XmlNode node in fnode.ChildNodes) if (node.NodeType == XmlNodeType.Element) newTextures.Add(GetAtlasTexture(atlasTexture, (XmlElement)node, scaler));
		
		return newTextures.ToArray();
	}
	
	/// <summary>
	/// Creates an instance of the component loads the texture of the resource folder, sets the parameters and starts playback.
	/// </summary>
	/// <returns>
	/// This instance.
	/// </returns>
	/// <param name='target'>
	/// Target GameObject.
	/// </param>
	/// <param name='texturesPath'>
	/// Folder where to load the textures in "Resources".
	/// </param>
	/// <param name='duration'>
	/// Duration in seconds.
	/// </param>
	/// <param name='speed'>
	/// Speed.
	/// </param>
	/// <param name='loop'>
	/// Loop type.
	/// </param>
	public static TextureSequence CreateAndPlay(GameObject target, string texturesPath, float duration, float speed, TextureSequenceLoop loop)
	{
		TextureSequence seq = CreateTextureSequence(target);
		if (seq == null) return null;
		
		seq.LoadAndPlay(texturesPath, duration, speed, loop);
		
		return seq;
	}
	
	/// <summary>
	/// Creates an instance of the component sets the texture sets the parameters and starts playback.
	/// </summary>
	/// <returns>
	/// This instance.
	/// </returns>
	/// <param name='target'>
	/// Target GameObject.
	/// </param>
	/// <param name='textures'>
	/// Texture array.
	/// </param>
	/// <param name='duration'>
	/// Duration in seconds.
	/// </param>
	/// <param name='speed'>
	/// Speed.
	/// </param>
	/// <param name='loop'>
	/// Loop type.
	/// </param>
	public static TextureSequence CreateAndPlay(GameObject target, Object[] textures, float duration, float speed, TextureSequenceLoop loop)
	{
		TextureSequence seq = CreateTextureSequence(target);
		if (seq == null) return null;
		
		seq.SetTexturesAndPlay(textures, duration, speed, loop);
		return seq;
	}
	
	/// <summary>
	/// Creates an instance of the component sets texture atlas sets the parameters and starts playback.
	/// </summary>
	/// <returns>
	/// This instance.
	/// </returns>
	/// <param name='target'>
	/// Target GameObject.
	/// </param>
	/// <param name='atlasTexture'>
	/// Texture atlas.
	/// </param>
	/// <param name='atlasSettings'>
	/// Atlas settings.
	/// </param>
	/// <param name='originalImageSize'>
	/// Original image size.
	/// </param>
	/// <param name='duration'>
	/// Duration in seconds.
	/// </param>
	/// <param name='speed'>
	/// Speed.
	/// </param>
	/// <param name='loop'>
	/// Loop type.
	/// </param>
	public static TextureSequence CreateAndPlay(GameObject target, Texture2D atlasTexture, TextAsset atlasSettings, Vector2 originalImageSize, float duration, float speed, TextureSequenceLoop loop)
	{
		TextureSequence seq = CreateTextureSequence(target);
		if (seq == null) return null;
		
		seq.SetAtlasAndPlay(atlasTexture, atlasSettings, originalImageSize, duration, speed, loop);
		return seq;
	}
	
	/// <summary>
	/// Creates an instance of the component texture atlas, loads settings atlas, sets the parameters and starts playback.
	/// </summary>
	/// <returns>
	/// This instance.
	/// </returns>
	/// <param name='target'>
	/// Target GameObject.
	/// </param>
	/// <param name='atlasTexture'>
	/// Texture atlas.
	/// </param>
	/// <param name='atlasSettingsPath'>
	/// Atlas settings file in "Resources" folder.
	/// </param>
	/// <param name='originalImageSize'>
	/// Original image size.
	/// </param>
	/// <param name='duration'>
	/// Duration in seconds.
	/// </param>
	/// <param name='speed'>
	/// Speed.
	/// </param>
	/// <param name='loop'>
	/// Loop type.
	/// </param>
	public static TextureSequence CreateAndPlay(GameObject target, Texture2D atlasTexture, string atlasSettingsPath, Vector2 originalImageSize, float duration, float speed, TextureSequenceLoop loop)
	{
		TextureSequence seq = CreateTextureSequence(target);
		if (seq == null) return null;
				
		seq.SetAtlasAndPlay(atlasTexture, atlasSettingsPath, originalImageSize, duration, speed, loop);
		return seq;
	}
	
	/// <summary>
	/// Gets the texture from atlas by index.
	/// </summary>
	/// <returns>
	/// The texture from atlas.
	/// </returns>
	/// <param name='atlasTexture'>
	/// Texture atlas.
	/// </param>
	/// <param name='atlasSettings'>
	/// Atlas settings.
	/// </param>
	/// <param name='originalImageSize'>
	/// Original image size.
	/// </param>
	/// <param name='textureIndex'>
	/// Texture index.
	/// </param>
	/// <exception cref='System.Exception'>
	/// If index incorrect is thrown when the exception.
	/// </exception>
	public static Texture2D GetTextureFromAtlas(Texture2D atlasTexture, TextAsset atlasSettings, Vector2 originalImageSize, uint textureIndex)
	{
		XmlElement fnode = GetAtlasSettingsFirstElement(atlasTexture, atlasSettings, originalImageSize);
		Vector2 scaler = new Vector2(atlasTexture.width / originalImageSize.x, atlasTexture.height / originalImageSize.y);
		
		uint index = 0;
		foreach(XmlNode node in fnode.ChildNodes) 
		{
			if (node.NodeType == XmlNodeType.Element) 
			{
				if (index == textureIndex) return GetAtlasTexture(atlasTexture, (XmlElement)node, scaler);
				index++;
			}
		}
		
		throw new System.Exception(string.Format("Texture index '{0}' incorrect.", textureIndex));
	}
	
	/// <summary>
	/// Gets the texture from atlas by name.
	/// </summary>
	/// <returns>
	/// The texture from atlas.
	/// </returns>
	/// <param name='atlasTexture'>
	/// Texture atlas.
	/// </param>
	/// <param name='atlasSettings'>
	/// Atlas settings.
	/// </param>
	/// <param name='originalImageSize'>
	/// Original image size.
	/// </param>
	/// <param name='textureName'>
	/// Texture name.
	/// </param>
	/// <exception cref='System.Exception'>
	/// If name not found is thrown when the exception.
	/// </exception>
	public static Texture2D GetTextureFromAtlas(Texture2D atlasTexture, TextAsset atlasSettings, Vector2 originalImageSize, string textureName)
	{
		XmlElement fnode = GetAtlasSettingsFirstElement(atlasTexture, atlasSettings, originalImageSize);
		Vector2 scaler = new Vector2(atlasTexture.width / originalImageSize.x, atlasTexture.height / originalImageSize.y);
		
		foreach(XmlNode node in fnode.ChildNodes)
		{
			if (node.NodeType == XmlNodeType.Element)
			{
				XmlElement el = (XmlElement) node;
				if (el.GetAttribute("name") == textureName) return GetAtlasTexture(atlasTexture, el, scaler);
			}
		}
		
		throw new System.Exception(string.Format("Texture name '{0}' not found.", textureName));
	}
	
	/// <summary>
	/// Checks if the texture readable.
	/// </summary>
	/// <returns>
	/// The readable texture.
	/// </returns>
	/// <param name='texture'>
	/// Texture for check.
	/// </param>
	public static bool isReadableTexture(Texture2D texture)
	{
		try
		{
			texture.GetPixels(0, 0, 1, 1);
		}
		catch (System.Exception)
		{
			 return false;
		}
		return true;
	}
	
	#endregion
	
	#region private functions
	
	private void DispatchComplete()
	{
		if (OnComplete != null) OnComplete();
		
		if (autoDestroy) Destroy(this);
	}
	
	private void DispatchLoop()
	{
		if (soundStartType != TextureSequenceSoundStart.none) foreach(TextureSequenceSound snd in sounds) { snd.Stop(); snd.started = false; }
		if (OnLoop != null) OnLoop();
	}
	
	public byte[] GetFileByExternalPath(string folder, string filename)
	{
        #if UNITY_STANDALONE
		    return File.ReadAllBytes(Path.Combine(folder, filename));
        #else
            return new byte[0];
        #endif
	}
	
	public Texture2D GetTextureByPath(string folder, string filename)
	{
		if (folder.Length > 0)
		{
			string lastChar = folder.Substring(folder.Length - 1);
			if (lastChar != "\\" && lastChar != "/") folder += "/";
		}
		return (Texture2D)Resources.Load(folder + filename);
	}
	
	void Awake()
	{
		if (targetObject == null) targetObject = gameObject;
		
		if (loadTextureTarget == TextureSequenceLoadTarget.fromResourcesFolder)
		{
			if (loadType == TextureSequenceLoadType.atStart) Load(texturePath);
			else if (textureFiles.Length > 0) 
			{
				_activeTexture = GetTextureByPath(texturePath, textureFiles[0]);
				loaded = true;
			}
			if (autoStart) Play();
		}
		else if (loadTextureTarget == TextureSequenceLoadTarget.fromExternalFolder)
		{
			if (textureFiles.Length > 0) 
			{
				if (loadType == TextureSequenceLoadType.atStart)
				{
					List<Object> ts = new List<Object>();
					foreach (string fn in textureFiles)
					{
						Texture2D texture = new Texture2D(32, 32);
						texture.LoadImage(GetFileByExternalPath(texturePath, fn));
						ts.Add(texture);
					}
					textures = ts.ToArray();
					loaded = textures.Length > 0;
				}
				else
				{
					_activeTexture = new Texture2D(32, 32);
					_activeTexture.LoadImage(GetFileByExternalPath(texturePath, textureFiles[0]));
					loaded = true;
				}
				if (autoStart) Play(); 
			}
		}
		else if (loadTextureTarget == TextureSequenceLoadTarget.manual)
		{
			if (length != 0)
			{
				loaded = true;
				if (autoStart) Play();
			}
		}
		else if (loadTextureTarget == TextureSequenceLoadTarget.atlas)
		{
			textures = ConvertAtlasToTextures(atlasTexture, atlasSettings, atlasTextureOriginalSize);
			if (length > 0)
			{
				loaded = true;
				if (autoStart) Play();
			}
		}
	}
	
	void Update()
	{
		if (!loaded || length == 0) return;
		if (target == TextureSequenceTarget.guiTexture && targetObject.guiTexture == null) return;
		if (target == TextureSequenceTarget.material && targetObject.renderer == null) return;
		
		if (isPlayed && duration > 0) 
		{
			currentTime += Time.deltaTime * speed;
			int _lastFrame = _frame;
			if (loop == TextureSequenceLoop.loop) 
			{
				if (currentTime > duration) DispatchLoop();
				currentTime = Mathf.Repeat(currentTime, duration);
			}
			else if (loop == TextureSequenceLoop.pingPong) 
			{
				if (currentTime > duration) { speed = -Mathf.Abs(speed); DispatchLoop(); }
				else if (currentTime < 0) { speed = Mathf.Abs(speed); DispatchLoop(); }
				
				currentTime = Mathf.PingPong(currentTime, duration);
			}
			
			if (currentTime > 0 && currentTime < duration)
			{
				timeStep = duration / length;
				float t = currentTime % duration;
				_frame = Mathf.FloorToInt(t / timeStep);
			}
			else if (currentTime >= duration) frame = length - 1;
			else frame = 0;
			
			if (_lastFrame != _frame) 
			{
				if (loadType == TextureSequenceLoadType.atShow)
				{
					if (loadTextureTarget == TextureSequenceLoadTarget.fromResourcesFolder) _activeTexture = GetTextureByPath(texturePath, textureFiles[_frame]);
					else if (loadTextureTarget == TextureSequenceLoadTarget.fromExternalFolder) _activeTexture.LoadImage(GetFileByExternalPath(texturePath, textureFiles[_frame]));
				}
				if (OnUpdate != null) OnUpdate();
			}
			
			if (soundStartType != TextureSequenceSoundStart.none)
			{
				foreach(TextureSequenceSound snd in sounds)
				{
					if (!snd.started)
					{
						if (soundStartType == TextureSequenceSoundStart.byFrame && _frame >= snd.frame) snd.Play();
						else if (soundStartType == TextureSequenceSoundStart.byTime && time >= snd.time) snd.Play();
					}
					else if (!snd.isPlaying)
					{
						snd.Stop();
					}
				}
			}
		}
		
		if (target == TextureSequenceTarget.guiTexture) targetObject.guiTexture.texture = activeTexture;
		else if (target == TextureSequenceTarget.material) targetObject.renderer.materials[materialID].SetTexture(textureName, activeTexture);
	}
	
	#endregion
	
	#region private static functions
	
	private static TextureSequence CreateTextureSequence(GameObject target)
	{
		if (target.renderer == null && target.guiTexture == null) throw new System.Exception("GameObject dont have Renderer or GUITexture components");
		
		TextureSequence seq = target.AddComponent<TextureSequence>();
		seq.targetObject = target;
		if (target.renderer != null) seq.target = TextureSequenceTarget.material;
		else seq.target = TextureSequenceTarget.guiTexture;
		
		return seq;
	}
	
	private static XmlElement GetAtlasSettingsFirstElement(Texture2D atlasTexture, TextAsset atlasSettings, Vector2 originalImageSize)
	{
		if (atlasTexture == null || atlasSettings == null) throw new System.Exception("Incorrect arguments.");
		if (originalImageSize.x <= 0 || originalImageSize.y <= 0) throw new System.Exception("Incorrect original image size.");
		if (!isReadableTexture(atlasTexture)) throw new System.Exception("Texture not readable. Go to Texture import settings. Set 'Texture type' - 'Advanced' and enable 'Read / Write Enabled'.");
		
		XmlDocument doc = new XmlDocument();
		doc.LoadXml(atlasSettings.text);
		
		XmlNode fnode = null;
		foreach(XmlNode node in doc.ChildNodes) if (node.NodeType == XmlNodeType.Element) { fnode = node; break; }
		
		if (fnode == null) throw new System.Exception("Incorrent atlas settings.");
		return (XmlElement) fnode;
	}
	
	private static Texture2D GetAtlasTexture(Texture2D atlasTexture, XmlElement node, Vector2 scaleFactor)
	{
		int x = Mathf.RoundToInt(int.Parse(node.GetAttribute("x")) * scaleFactor.x);
		int y = Mathf.RoundToInt(int.Parse(node.GetAttribute("y")) * scaleFactor.y) ;
		int w = Mathf.RoundToInt(int.Parse(node.GetAttribute("width")) * scaleFactor.x);
		int h = Mathf.RoundToInt(int.Parse(node.GetAttribute("height")) * scaleFactor.y);
		
		if (x + w >= atlasTexture.width || y + h >= atlasTexture.height) throw new System.Exception("Incorrect original image size.");
		
		Texture2D img = new Texture2D(w, h);
		img.SetPixels(atlasTexture.GetPixels(x, atlasTexture.height - y - h, w, h));
		img.Apply();
		
		return img;
	}
	
	#endregion
}

[System.Serializable]
public class TextureSequenceSound
{
	private static GameObject _container;
	
	public AudioClip clip;
	public int frame = 0;
	public AudioSource source;
	public bool started = false;
	public GameObject target;
	public float time = 0;
	
	private GameObject container
	{
		get
		{
			if (_container == null) 
			{
				_container = GameObject.Find("_TextureSequence_Sounds_Container_");
				if (_container == null) _container = new GameObject("_TextureSequence_Sounds_Container_");
			}
			return _container;
		}
	}
	
	public bool isPlaying
	{
		get 
		{ 
			return source != null && source.isPlaying; 
		}
	}
	
	public void Play()
	{
		if (clip != null)
		{
			target = new GameObject();
			target.transform.parent = container.transform;
			source = target.AddComponent<AudioSource>();
			source.clip = clip;
			source.Play();
		}
		started = true;
	}
	
	public void Stop()
	{
		if (source != null) source.Stop();
		if (target != null) GameObject.Destroy(target);
	}
}

/// <summary>
/// Atlas type. Not used. Reserve for future developments.
/// </summary>
public enum TextureSequenceAtlasType
{
	Starling = 0
}

/// <summary>
/// Load target.
/// </summary>
public enum TextureSequenceLoadTarget
{
	none = 0,
	manual = 10,
	atlas = 20,
	fromResourcesFolder = 30,
	fromExternalFolder = 40
}

public enum TextureSequenceLoadType
{
	atStart = 0,
	atShow = 1
}

/// <summary>
/// Type of loop.
/// </summary>
public enum TextureSequenceLoop
{
	none = 0,
	loop = 1,
	pingPong = 2
}

/// <summary>
/// Apply target.
/// </summary>
public enum TextureSequenceTarget
{
	material = 1,
	guiTexture = 2
}

/// <summary>
/// Sync type sounds enum.
/// </summary>
public enum TextureSequenceSoundStart
{
	none = 0,
	byFrame = 10,
	byTime = 20
}