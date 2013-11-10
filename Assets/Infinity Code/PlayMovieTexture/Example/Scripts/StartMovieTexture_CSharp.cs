using UnityEngine;
using System.Collections;

public class StartMovieTexture_CSharp : MonoBehaviour 
{
	public GameObject target;
	
	void OnGUI()
	{
		int x = Screen.width - 210;
		if (GUI.Button(new Rect(x, 10, 200, 30), "Start sphere parallax movies C#")) SendMessage("StartMovies");
		if (GUI.Button(new Rect(x, 45, 200, 30), "Stop sphere parallax movies C#")) SendMessage("StopMovies");
		if (GUI.Button(new Rect(x - 310, 10, 300, 30), "Start all videos with delay 1 sec")) PlayMovieTexture.StartAllMovies(1);
		if (GUI.Button(new Rect(x - 310, 45, 300, 30), "Stop all videos")) PlayMovieTexture.StopAllMovies();
	}
}
