using Godot;
using System;
using System.Collections.Generic;

public partial class Spawner : Node3D
{
	// Called when the node enters the scene tree for the first time.
	PackedScene Person = GD.Load<PackedScene>("res://Scenes/Objectives/StrandedPerson.tscn");
	public Texture2D[] Icons = {
		GD.Load<Texture2D>("res://Assets/Textures/Passenger.png")
	};
	public Passenger.Destinations randomDestination()
	{
		return (Passenger.Destinations)(GD.Randi() % Enum.GetNames(typeof(Passenger.Destinations)).Length);
	}

	public Texture2D randomIcon(){
		return Icons[GD.Randi()%Icons.Length];
	}
 
	public String randomMessage(Passenger.Destinations destination){
		switch (destination){
			case Passenger.Destinations.SpaceStationTheta:
				return "Help I need a ride to space station theta";
			case Passenger.Destinations.SpaceStationDelta:
				return "Help I need a ride to Space station delta";
			case Passenger.Destinations.SpaceStationOmega:
				return "Help I need a ride to Space station omega";
			// case Passenger.Destinations.Planet:
			// 	return "yo gurt, gurt yo";
			// case Passenger.Destinations.SpaceShip:
				// return "test";
		}
		return "BAD DESTINATION";
	}

	public override void _Ready()
	{
		if ((GD.Randi() % 10) >= 8)
		{
			return;
		}
		Passenger newPassenger = Person.Instantiate<Passenger>();
		Passenger.Destinations destinations = randomDestination();
		AddChild(newPassenger);
		newPassenger.instantiatePassenger(randomIcon(), destinations,randomMessage(destinations));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
