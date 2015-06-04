/*       INFINITY CODE 2013-2015         */
/*     http://www.infinity-code.com      */

#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6
#define UNITY_4X
#endif

using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Infinity Code/Play MovieTexture")]
public class PlayMovieTexture : MonoBehaviour 
{
	#region public variables

    public PlayMovieTextureStopEnum afterStop = PlayMovieTextureStopEnum.none;

	/// <summary>
	/// Startup type: at start, deleyed or manual.
	/// </summary>
	public PlayMovieTextureAutostartEnum autostart = PlayMovieTextureAutostartEnum.atStart;

    public string customActionMethod;
    public GameObject customActionTarget;
	
	/// <summary>
	/// Delay before starting.
	/// </summary>
	public float delay;
	
	/// <summary>
	/// Do not change. Used for the editor.
	/// </summary>
	public int flag = -1;
	
	/// <summary>
	/// Loop videos.
	/// </summary>
	public bool loop = true;
	
	/// <summary>
	/// List detected MovieTextures.
	/// </summary>
	public MovieTexture[] movieTextures;

	public PlayMovieTextureTarget target = PlayMovieTextureTarget.gameobject;
	
	/// <summary>
	/// Custom target gameobject.
	/// </summary>
	public GameObject targetObject;
	
	#endregion
	
	#region private variables

    private AudioListener audioListener;

	/// <summary>
	/// The aviable texture names. Do not change.
	/// </summary>
	public static readonly string[] aviableTextureNames = {"_MainTex", "_BumpMap", "_ParallaxMap", "_DecalTex", "_Illum", "_LightMap", "_Mask"};
	
	/// <summary>
	/// The aviable texture titles. Do not change.
	/// </summary>
	public static readonly string[] aviableTextureTitles = {"Base Texture", "Normal Map", "Parallax Map", "Decal Texture", "Illumin", "Light Map", "Culling Mask"};
	
	#endregion
	
	#region public functions
	
	/// <summary>
	/// Pause selected MovieTextures.
	/// </summary>
	public void PauseMovies()
	{
		PauseMovies(movieTextures);
	}
	
	/// <summary>
	/// Start playing selected MovieTextures.
	/// </summary>
	/// <returns>
	/// Selected MovieTextures.
	/// </returns>
	public MovieTexture[] StartMovies()
	{
		List<MovieTexture> mts = new List<MovieTexture>();
		
		foreach(MovieTexture mt in movieTextures)
		{
			if (mt != null)
			{
				mts.Add(mt);
				mt.loop = loop;
				mt.Play();
                if (!loop && afterStop != PlayMovieTextureStopEnum.none) StartCoroutine(WaitStopMovie(mt));
			}
		}
		
		return mts.ToArray();
	}

    /// <summary>
	/// Stop playing MovieTextures.
	/// </summary>
	public void StopMovies()
	{
		StopMovies(movieTextures);
	}
	
	#endregion
	
	#region public static functions
	
	/// <summary>
	/// Gets all MovieTexture in scene.
	/// </summary>
	/// <returns>
	/// MovieTexture array.
	/// </returns>
	public static MovieTexture[] GetMovieTextures()
	{
		List<MovieTexture> mts = new List<MovieTexture>();
		
		GUITexture[] guiTextures = FindObjectsOfType<GUITexture>();
	    Renderer[] renderers = FindObjectsOfType<Renderer>();
		
        mts.AddRange(guiTextures.Where(t=>t.texture is MovieTexture).Select(t=>(MovieTexture)t.texture));
		foreach (Renderer r in renderers)
		{
			for (int i = 0; i < r.sharedMaterials.Length; i++)
			{
				Material mat = r.sharedMaterials[i];
				if (mat != null)
				{
					for (int j = 0; j < aviableTextureNames.Length; j++) 
					{
						if (mat.HasProperty(aviableTextureNames[j])) 
						{
							Texture t = mat.GetTexture(aviableTextureNames[j]);
							if (t && t is MovieTexture) mts.Add((MovieTexture)t);
						}
					}
				}
			}
		}
		
		return mts.ToArray();
	}
	
	/// <summary>
	/// Get all MovieTextures assigned to GameObject
	/// </summary>
	/// <returns>
	/// MovieTexture array.
	/// </returns>
	/// <param name='target'>
	/// Target GameObject.
	/// </param>
	public static MovieTexture[] GetMovieTextures(GameObject target)
	{
		List<MovieTexture> mts = new List<MovieTexture>();

#if UNITY_4X
	    Renderer renderer = target.renderer;
#else
        Renderer renderer = target.GetComponent<Renderer>();
#endif
	    if (renderer != null)
		{
			for (int i = 0; i < renderer.sharedMaterials.Length; i++)
			{
				Material mat = renderer.sharedMaterials[i];
				if (mat != null)
				{
					for (int j = 0; j < aviableTextureNames.Length; j++) 
					{
						if (mat.HasProperty(aviableTextureNames[j])) 
						{
							Texture t = mat.GetTexture(aviableTextureNames[j]);
							if (t && t is MovieTexture) mts.Add((MovieTexture)t);
						}
					}
				}
			}
		}

#if UNITY_4X
	    GUITexture guiTexure = target.guiTexture;
#else
        GUITexture guiTexure = target.GetComponent<GUITexture>();
#endif
	    if (guiTexure != null && guiTexure.texture is MovieTexture) mts.Add((MovieTexture)guiTexure.texture);
		
		return mts.ToArray();
	}
	
	/// <summary>
	/// Pause all MovieTexture in scene.
	/// </summary>
	public static void PauseAllMovies()
	{
		PauseMovies(GetMovieTextures());
	}
	
	/// <summary>
	/// Pause custom MovieTexture.
	/// </summary>
	/// <param name='mt'>
	/// MovieTexture.
	/// </param>
	public static void PauseMovie(MovieTexture mt)
	{
		mt.Pause();
	}
	
	/// <summary>
	/// Pause custom MovieTexture array.
	/// </summary>
	/// <param name='mts'>
	/// MovieTextures
	/// </param>
	public static void PauseMovies(MovieTexture[] mts)
	{
		foreach (MovieTexture mt in mts) mt.Pause();
	}
	
	/// <summary>
	/// Pause all MovieTexture assigned to GameObject.
	/// </summary>
	/// <param name='target'>
	/// Target GameObject.
	/// </param>
	public static void PauseMovies(GameObject target)
	{
		PauseMovies(GetMovieTextures(target));
	}
	
	/// <summary>
	/// Start playing all MovieTextures in scene.
	/// </summary>
	/// <returns>
	/// MovieTextures
	/// </returns>
	public static MovieTexture[] StartAllMovies()
	{
		return StartMovies(GetMovieTextures());
	}
	
	/// <summary>
	/// Start playing all MovieTextures in scene with loop.
	/// </summary>
	/// <returns>
	/// MovieTextures.
	/// </returns>
	/// <param name='loop'>
	/// Loop.
	/// </param>
	public static MovieTexture[] StartAllMovies(bool loop)
	{
		return StartMovies(GetMovieTextures(), loop);
	}
	
	/// <summary>
	/// Start playing all MovieTexture in scene with delay.
	/// </summary>
	/// <returns>
	/// MovieTextures.
	/// </returns>
	/// <param name='delay'>
	/// Delay.
	/// </param>
	public static MovieTexture[] StartAllMovies(float delay)
	{
		return StartMovies(GetMovieTextures(), delay);
	}
	
	/// <summary>
	/// Start playing all MovieTexture in scene with delay and loop.
	/// </summary>
	/// <returns>
	/// MovieTextures
	/// </returns>
	/// <param name='loop'>
	/// Loop.
	/// </param>
	/// <param name='delay'>
	/// Delay.
	/// </param>
	public static MovieTexture[] StartAllMovies(float delay, bool loop)
	{
		return StartMovies(GetMovieTextures(), delay, loop);
	}
	
	/// <summary>
	/// Starts playing custom MovieTexture.
	/// </summary>
	/// <returns>
	/// MovieTexture.
	/// </returns>
	/// <param name='mt'>
	/// MovieTexture to start playing.
	/// </param>
	public static MovieTexture StartMovie(MovieTexture mt)
	{
		mt.Play();
		return mt;
	}
	
	/// <summary>
	/// Starts playing custom MovieTexture with loop.
	/// </summary>
	/// <returns>
	/// MovieTexture.
	/// </returns>
	/// <param name='mt'>
	/// MovieTexture.
	/// </param>
	/// <param name='loop'>
	/// Loop.
	/// </param>
	public static MovieTexture StartMovie(MovieTexture mt, bool loop)
	{
		mt.loop = loop;
		return StartMovie(mt);
	}
	
	/// <summary>
	/// Start playing custom MovieTexture with delay.
	/// </summary>
	/// <returns>
	/// MovieTexture.
	/// </returns>
	/// <param name='mt'>
	/// MovieTexture.
	/// </param>
	/// <param name='delay'>
	/// Delay.
	/// </param>
	public static MovieTexture StartMovie(MovieTexture mt, float delay)
	{
		GameObject go = new GameObject("_PlayMovieTextureIEnumenator");
		PlayMovieTexture pmt = go.AddComponent<PlayMovieTexture>();
		pmt.StartCoroutine(pmt._StartMovie(mt, delay));
		return mt;
	}
	
	/// <summary>
	/// tart playing custom MovieTexture with delay and loop.
	/// </summary>
	/// <returns>
	/// MovieTexture.
	/// </returns>
	/// <param name='mt'>
	/// MovieTexture.
	/// </param>
	/// <param name='delay'>
	/// Delay.
	/// </param>
	/// <param name='loop'>
	/// Loop.
	/// </param>
	public static MovieTexture StartMovie(MovieTexture mt, float delay, bool loop)
	{
		mt.loop = loop;
		return StartMovie(mt, delay);
	}
	
	/// <summary>
	/// Start playing custom MovieTexture array.
	/// </summary>
	/// <returns>
	/// MovieTextures.
	/// </returns>
	/// <param name='mts'>
	/// MovieTextures.
	/// </param>
	public static MovieTexture[] StartMovies(MovieTexture[] mts)
	{
		foreach(MovieTexture mt in mts) StartMovie(mt);
		return mts;
	}
	
	/// <summary>
	/// Start playing custom MovieTexture array with loop.
	/// </summary>
	/// <returns>
	/// MovieTextures.
	/// </returns>
	/// <param name='mts'>
	/// MovieTextures.
	/// </param>
	/// <param name='loop'>
	/// Loop.
	/// </param>
	public static MovieTexture[] StartMovies(MovieTexture[] mts, bool loop)
	{
		foreach (MovieTexture mt in mts) mt.loop = loop;
		return StartMovies(mts);
	}
	
	/// <summary>
	/// Start playing custom MovieTexture array with delay.
	/// </summary>
	/// <returns>
	/// MovieTextures.
	/// </returns>
	/// <param name='mts'>
	/// MovieTextures.
	/// </param>
	/// <param name='delay'>
	/// Delay.
	/// </param>
	public static MovieTexture[] StartMovies(MovieTexture[] mts, float delay)
	{
		GameObject go = new GameObject("_PlayMovieTextureIEnumenator");
		PlayMovieTexture pmt = go.AddComponent<PlayMovieTexture>();
		pmt.StartCoroutine(pmt._StartMovies(mts, delay));
		return mts;
	}
	
	/// <summary>
	/// Start playing custom MovieTexture array with delay and loop.
	/// </summary>
	/// <returns>
	/// MovieTextures.
	/// </returns>
	/// <param name='mts'>
	/// MovieTextures.
	/// </param>
	/// <param name='delay'>
	/// Delay.
	/// </param>
	/// <param name='loop'>
	/// Loop.
	/// </param>
	public static MovieTexture[] StartMovies(MovieTexture[] mts, float delay, bool loop)
	{
		foreach (MovieTexture mt in mts) mt.loop = loop;
		return StartMovies(mts, delay);
	}
	
	/// <summary>
	/// Start playing all MovieTexture, assigned to GameObject.
	/// </summary>
	/// <returns>
	/// MovieTextures.
	/// </returns>
	/// <param name='target'>
	/// Target GameObject.
	/// </param>
	public static MovieTexture[] StartMovies(GameObject target)
	{
		MovieTexture[] mts = GetMovieTextures(target);
		return StartMovies(mts);
	}
	
	/// <summary>
	/// tart playing all MovieTexture, assigned to GameObject with loop.
	/// </summary>
	/// <returns>
	/// MovieTextures.
	/// </returns>
	/// <param name='target'>
	/// Target GameObject.
	/// </param>
	/// <param name='loop'>
	/// Loop.
	/// </param>
	public static MovieTexture[] StartMovies(GameObject target, bool loop)
	{
		MovieTexture[] mts = GetMovieTextures(target);
		foreach (MovieTexture mt in mts) mt.loop = loop;
		return StartMovies(mts);
	}
	
	/// <summary>
	/// Start playing all MovieTexture, assigned to GameObject with delay.
	/// </summary>
	/// <returns>
	/// MovieTexture.
	/// </returns>
	/// <param name='target'>
	/// Target GameObject.
	/// </param>
	/// <param name='delay'>
	/// Delay.
	/// </param>
	public static MovieTexture[] StartMovies(GameObject target, float delay)
	{
		MovieTexture[] mts = GetMovieTextures(target);
		return StartMovies(mts, delay);
	}
	
	/// <summary>
	/// Start playing all MovieTexture, assigned to GameObject with delay and loop.
	/// </summary>
	/// <returns>
	/// MovieTextures.
	/// </returns>
	/// <param name='target'>
	/// Target GameObject.
	/// </param>
	/// <param name='delay'>
	/// Delay.
	/// </param>
	/// <param name='loop'>
	/// Loop.
	/// </param>
	public static MovieTexture[] StartMovies(GameObject target, float delay, bool loop) 
	{
		MovieTexture[] mts = GetMovieTextures(target);
		foreach (MovieTexture mt in mts) mt.loop = loop;
		return StartMovies(mts, delay);
	}
	
	/// <summary>
	/// Stop playing all MovieTexture in scene.
	/// </summary>
	public static void StopAllMovies()
	{
		StopMovies(GetMovieTextures());
	}
	
	/// <summary>
	/// Stop playing custom MovieTexture.
	/// </summary>
	/// <param name='mt'>
	/// MovieTexture.
	/// </param>
	public static void StopMovie(MovieTexture mt) 
	{
		if (mt != null) mt.Stop();
	}
	
	/// <summary>
	/// Stop playing custom MovieTexture array.
	/// </summary>
	/// <param name='mts'>
	/// MovieTextures.
	/// </param>
	public static void StopMovies(MovieTexture[] mts)
	{
		foreach(MovieTexture mt in mts) StopMovie(mt);
	}
	
	/// <summary>
	/// Stop playing all MovieTexture, assigned to GameObject.
	/// </summary>
	/// <param name='target'>
	/// Target GameObject.
	/// </param>
	public static void StopMovies(GameObject target)
	{
		foreach(MovieTexture mt in GetMovieTextures(target)) StopMovie(mt);
	}
	
	#endregion
	
	#region private functions
    private void Start()
	{
		if (movieTextures == null) return;
		
		if (autostart == PlayMovieTextureAutostartEnum.atStart) StartMovies();
		else if (autostart == PlayMovieTextureAutostartEnum.delayed) StartCoroutine(WaitStartMovies());
	}

    private IEnumerator WaitStartMovies()
	{
		yield return new WaitForSeconds(delay);
		StartMovies();
	}

    private IEnumerator WaitStopMovie(MovieTexture mt)
    {
        do
        {
            yield return new WaitForSeconds(0.1f);
        }
        while (mt.isPlaying);

        if (afterStop == PlayMovieTextureStopEnum.disableGameobject) gameObject.SetActive(false);
        else if (afterStop == PlayMovieTextureStopEnum.destroyGameobject) Destroy(gameObject);
        else if (afterStop == PlayMovieTextureStopEnum.customAction)
        {
            if (customActionTarget == null) customActionTarget = gameObject;
            if (customActionMethod != "") customActionTarget.SendMessage(customActionMethod);
        }
    }

    private IEnumerator _StartMovie(MovieTexture mt, float delay)
	{
		yield return new WaitForSeconds(delay);
		StartMovie(mt);
		Destroy(gameObject);
	}

    private IEnumerator _StartMovies(MovieTexture[] mts, float _delay)
	{
		yield return new WaitForSeconds(_delay);
		StartMovies(mts);
		Destroy(gameObject);
	}
	
	#endregion
}

/// <summary>
/// Startup type enumerate
/// </summary>
public enum PlayMovieTextureAutostartEnum
{
	/// <summary>
	/// Constant at start.
	/// </summary>
	atStart,
	/// <summary>
	/// Constant delayed.
	/// </summary>
	delayed,
	/// <summary>
	/// Constant manual.
	/// </summary>
	manual
}

public enum PlayMovieTextureStopEnum
{
    none,
    disableGameobject,
    destroyGameobject,
    customAction
}

public enum PlayMovieTextureTarget
{
	gameobject = 0,
	scene = 1
}