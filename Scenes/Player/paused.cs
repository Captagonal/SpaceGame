using Godot;
using System;

public partial class paused : Label
{
	// Called when the node enters the scene tree for the first time.
	public bool isPaused = false;
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Pause"))
		{
			
			// Input.MouseMode = Input.MouseModeEnum.Visible;
			// HUD.GetNode<ReferenceRect>("ReferenceRect").GetNode<Label>("Pause").Visible = true;
			// GD.Print("Game Paused");
			TogglePause();
			// GetTree().Paused = true;

		}

	}
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.IsPressed() && keyEvent.Keycode == Key.Escape)
		{
			
		}
	}
	public void TogglePause()
	{
		isPaused = !isPaused;
		GetTree().Paused = isPaused;
		this.Visible = isPaused;
		Input.MouseMode = isPaused ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
	}
}
