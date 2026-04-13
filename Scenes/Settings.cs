using Godot;
using System;

public partial class Settings : Control
{
	// Called when the node enters the scene tree for the first time.
	private Label volumeLabel;
	private bool horseModeEnabled = false;
	public override void _Ready()
	{

		volumeLabel = GetNode<Label>("Volume");
		HSlider slider = volumeLabel.GetNode<HSlider>("HSlider");
		slider.Value = Mathf.DbToLinear(AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex("Master")));
		volumeLabel.GetNode<Label>("percent").Text = $"{(int)(slider.Value * 100)}%";

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void back()
	{
		var config = new ConfigFile();
		HSlider slider = volumeLabel.GetNode<HSlider>("HSlider");
		// Store some values.
		config.SetValue("Audio", "Volume", slider.Value);
		config.SetValue("Gameplay", "HorseMode", horseModeEnabled);

		// Save it to a file (overwrite if already exists).
		config.Save("user://config.cfg");

		GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
	}
	public void _on_h_slider_value_changed(float value)
	{
		var audioBus = AudioServer.GetBusIndex("Master");
		AudioServer.SetBusVolumeDb(audioBus, Mathf.LinearToDb(value));
		volumeLabel.GetNode<Label>("percent").Text = $"{(int)(value * 100)}%";

	}

	public void horseMode(bool horseMode){
		horseModeEnabled = horseMode;
	}
}
