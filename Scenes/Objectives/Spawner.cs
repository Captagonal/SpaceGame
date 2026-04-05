using Godot;
using System;

public partial class Spawner : Node3D
{
	// Called when the node enters the scene tree for the first time.
	PackedScene Person = GD.Load<PackedScene>("res://Scenes/Objectives/StrandedPerson.tscn");

	public override void _Ready()
	{
		Passenger newPassenger = Person.Instantiate<Passenger>();
		newPassenger.instantiatePassenger(GD.Load<Texture2D>("res://icon.svg"), Passenger.Destinations.SpaceStationTheta);
		AddChild(newPassenger);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
