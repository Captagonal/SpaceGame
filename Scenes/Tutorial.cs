using Godot;
using System;

public partial class Tutorial : Node3D
{
	// Called when the node enters the scene tree for the first time.
	SpaceShip spaceShip;
	public override void _Ready()
	{
		spaceShip = GetNode<SpaceShip>("SpaceShip");
		spaceShip.setMode(SpaceShip.GameMode.Tutorial);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
