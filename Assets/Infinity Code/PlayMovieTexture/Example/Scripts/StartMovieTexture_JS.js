#pragma strict

function OnGUI () 
{
	var x = Screen.width - 210;
	if (GUI.Button(new Rect(x, 80, 200, 30), "Start sphere parallax movies JS")) SendMessage("StartMovies");
	if (GUI.Button(new Rect(x, 115, 200, 30), "Stop sphere parallax movies JS")) SendMessage("StopMovies");
}