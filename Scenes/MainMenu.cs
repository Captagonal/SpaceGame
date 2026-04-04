using Godot;
using System;

public partial class MainMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void start(){
		GetTree().ChangeSceneToFile("res://Scenes/TestLevel.tscn");
	}

	public void quit(){
		GetTree().Quit();
	}

	public void settings(){
		GetTree().ChangeSceneToFile("res://Scenes/Settings.tscn");
	}
}
