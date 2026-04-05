using Godot;
using System;
using System.Collections.Generic;

public partial class Spawner : Node3D
{
	// Called when the node enters the scene tree for the first time.
	PackedScene Person = GD.Load<PackedScene>("res://Scenes/Objectives/StrandedPerson.tscn");
	public Texture2D[] Icons = {
		GD.Load<Texture2D>("res://icon.svg")
	};
	public Passenger.Destinations randomDestination()
	{
		return Passenger.Destinations.SpaceStationTheta;
	}

	public Texture2D randomIcon(){
		return Icons[GD.Randi()%Icons.Length];
	}

	public override void _Ready()
	{
		if ((GD.Randi() % 10) >= 8)
		{
			return;
		}
		Passenger newPassenger = Person.Instantiate<Passenger>();
		newPassenger.instantiatePassenger(GD.Load<Texture2D>("res://icon.svg"), randomDestination(),"helo");
		AddChild(newPassenger);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
