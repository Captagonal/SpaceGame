using Godot;
using System;

public partial class Intro : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var config = new ConfigFile();

		// Load data from a file.
		Error err = config.Load("user://config.cfg");

		// If the file didn't load, ignore it.
		if (err != Error.Ok)
		{
			return;
		}

		var audioBus = AudioServer.GetBusIndex("Master");

		AudioServer.SetBusVolumeDb(audioBus, Mathf.LinearToDb((float)config.GetValue("Audio", "Volume", 1f)));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void introFinished(){
		GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
	}
	public void skipIntro(){
		GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
	}
}
