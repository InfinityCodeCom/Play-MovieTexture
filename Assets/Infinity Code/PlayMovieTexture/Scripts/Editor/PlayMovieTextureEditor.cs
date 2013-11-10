﻿/*       INFINITY CODE 2013         */
/*   http://www.infinity-code.com   */
/*                                  */
/*        Play MovieTexture         */
/*          Version 1.2.2           */

using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(PlayMovieTexture))]
public class PlayMovieTextureEditor : Editor 
{
	private PlayMovieTexture pmt;
	private GUITexture guiTexture;
	
    private PlayMovieTextureMask getMask()
	{
		PlayMovieTextureMask mask = new PlayMovieTextureMask();
		
		if (pmt.target == PlayMovieTextureTarget.gameobject)
		{
			if (pmt.targetObject.renderer != null)
			{
				for (int i = 0; i < pmt.targetObject.renderer.sharedMaterials.Length; i++)
				{
					Material mat = pmt.targetObject.renderer.sharedMaterials[i];
					if (mat != null)
					{
						for (int j = 0; j < PlayMovieTexture.aviableTextureNames.Length; j++) 
						{
							if (mat.HasProperty(PlayMovieTexture.aviableTextureNames[j])) mask.addTexture(mat, mat.GetTexture(PlayMovieTexture.aviableTextureNames[j]), PlayMovieTexture.aviableTextureTitles[j]);
						}
					}
				}
			}
			if (guiTexture != null) mask.addTexture(null, guiTexture.texture, "GUITexture");
		}
		else if (pmt.target == PlayMovieTextureTarget.scene)
		{
			Renderer[] rs = (Renderer[])FindObjectsOfType(typeof(Renderer));
			GUITexture[] ts = (GUITexture[])FindObjectsOfType(typeof(GUITexture));
			
			foreach (Renderer r in rs)
			{
				for (int i = 0; i < r.sharedMaterials.Length; i++)
				{
					Material mat = r.sharedMaterials[i];
					if (mat != null)
					{
						for (int j = 0; j < PlayMovieTexture.aviableTextureNames.Length; j++) 
						{
							if (mat.HasProperty(PlayMovieTexture.aviableTextureNames[j])) mask.addTexture(mat, mat.GetTexture(PlayMovieTexture.aviableTextureNames[j]), r.name + ". " + PlayMovieTexture.aviableTextureTitles[j]);
						}
					}
				}
			}
			foreach (GUITexture t in ts)
			{
				if (t != null) mask.addTexture(null, t.texture, t.name + ". " + "GUITexture");
			}
		}
		
		return mask;
	}

    private static bool isSceneObject(GameObject target)
    {
        return FindSceneObjectsOfType(typeof(GameObject)).Contains(target);
    }

    void OnEnable()
    {
        pmt = (PlayMovieTexture)target;
    }

    public override void OnInspectorGUI()
    {
        PlayMovieTextureTarget newTarget = (PlayMovieTextureTarget)EditorGUILayout.EnumPopup("Target: ", pmt.target);
        if (newTarget != pmt.target)
        {
            pmt.target = newTarget;
            pmt.flag = -1;
        }

        if (pmt.target == PlayMovieTextureTarget.gameobject)
        {
            if (pmt.targetObject == null) pmt.targetObject = pmt.gameObject;
            guiTexture = pmt.targetObject.guiTexture;

            pmt.targetObject = (GameObject)EditorGUILayout.ObjectField("Target Object: ", pmt.targetObject, typeof(GameObject), true);

            if (pmt.targetObject.renderer == null && guiTexture == null)
            {
                GUIStyle errorStyle = new GUIStyle();
                errorStyle.normal.textColor = Color.red;
                EditorGUILayout.LabelField("Need Renderer component or GUITexture!!!", errorStyle);
                return;
            }
        }

        PlayMovieTextureMask mask = getMask();
        pmt.flag = EditorGUILayout.MaskField("Textures: ", pmt.flag, mask.getTitles());
        pmt.autostart = (PlayMovieTextureAutostartEnum)EditorGUILayout.EnumPopup("Play: ", pmt.autostart);
        if (pmt.autostart == PlayMovieTextureAutostartEnum.delayed) pmt.delay = EditorGUILayout.FloatField("Delay: ", pmt.delay);
        pmt.loop = EditorGUILayout.Toggle("Loop: ", pmt.loop);

        if (!pmt.loop)
        {
            pmt.afterStop = (PlayMovieTextureStopEnum)EditorGUILayout.EnumPopup("On stop: ", pmt.afterStop);
            if (pmt.afterStop == PlayMovieTextureStopEnum.customAction)
            {
                GameObject newActionTarget = (GameObject)EditorGUILayout.ObjectField("Action Gameobject: ", pmt.customActionTarget, typeof(GameObject), true);
                if (newActionTarget == null || isSceneObject(newActionTarget)) pmt.customActionTarget = newActionTarget;
                else EditorUtility.DisplayDialog("Warning", "You can only use the GameObjects in the scene.", "OK");

                pmt.customActionMethod = EditorGUILayout.TextField("Action method:", pmt.customActionMethod);
            }
        }

        pmt.movieTextures = mask.getTextures(pmt.flag);

        if (EditorApplication.isPlaying)
        {
            if (GUILayout.Button("Start movies")) pmt.StartMovies();
            if (GUILayout.Button("Stop movies")) pmt.StopMovies();
        }
        else if (pmt.autostart == PlayMovieTextureAutostartEnum.manual) EditorGUILayout.LabelField("Switch to PlayMode to start button");
    }
}

public class PlayMovieTextureMask
{
	private List<PlayMovieTextureMaskItem> items;
	
	public PlayMovieTextureMask()
	{
		items = new List<PlayMovieTextureMaskItem>();
	}
	
	public void addTexture(Material material, Texture texture, string title)
	{
		items.Add(new PlayMovieTextureMaskItem(material, texture, title));
	}
	
	public MovieTexture[] getTextures(int flag)
	{
		BitArray bArray = new BitArray(System.BitConverter.GetBytes(flag));
		List<MovieTexture> mts = new List<MovieTexture>();
		
		bool haveWarning = false;
		GUIStyle warningStyle = new GUIStyle
		{
		    normal = {textColor = EditorStyles.label.normal.textColor},
		    margin = new RectOffset(5, 5, 0, 0),
		    wordWrap = true
		};

	    for (int i = 0; i < items.Count; i++) 
		{
			if (items[i].texture is MovieTexture)
			{
				if (mts.Contains((MovieTexture)items[i].texture))
				{
					if (!haveWarning)
					{
						EditorGUILayout.LabelField("Warnings: ", EditorStyles.boldLabel);
						haveWarning = true;
					}
					EditorGUILayout.LabelField("MovieTexture «" + items[i].texture.name + "» located in several places. They will run together.", warningStyle);
				}
				else if (bArray.Get(i)) 
				{
					mts.Add((MovieTexture)items[i].texture);
				}
			}
		}
		return mts.ToArray();
	}
	
	public string[] getTitles()
	{
		string[] retV = new string[items.Count];
		for (int i = 0; i < items.Count; i++) 
		{
			if (items[i].material != null) retV[i] = items[i].material.name + ". " + items[i].title;
			else retV[i] = items[i].title;
		}
		return retV;
	}
}

public class PlayMovieTextureMaskItem
{
	public Material material;
	public Texture texture;
	public string title;
	
	public PlayMovieTextureMaskItem(Material _material, Texture _texture, string _title)
	{
		material = _material;
		texture = _texture;
		title = _title;
	}
}