using Godot;
using System;
using System.Collections.Generic;

public partial class Spawner : Node3D
{
	public enum PassengerTypes
	{
		person,
		Bloblin
	}
	// Called when the node enters the scene tree for the first time.
	PackedScene Person = GD.Load<PackedScene>("res://Scenes/Objectives/StrandedPerson.tscn");
	public Texture2D[] Icons = {
		GD.Load<Texture2D>("res://Assets/Textures/Passenger.png"),
		GD.Load<Texture2D>("res://Assets/Textures/Bloblin.png"),
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
		PassengerTypes type = (PassengerTypes)(GD.Randi() % Enum.GetNames(typeof(PassengerTypes)).Length);
		Passenger newPassenger = Person.Instantiate<Passenger>();
		Passenger.Destinations destinations = randomDestination();
		AddChild(newPassenger);
		newPassenger.instantiatePassenger(Icons[(int)type], destinations,randomMessage(destinations));
		switch (type){
			case PassengerTypes.person:
				newPassenger.GetNode<Node3D>("Person").GetNode<Node3D>("PersonToSave").Visible = true;
				newPassenger.GetNode<Node3D>("Person").GetNode<Node3D>("SpaceBloblin").Visible = false;
				break;
			case PassengerTypes.Bloblin:
				newPassenger.GetNode<Node3D>("Person").GetNode<Node3D>("PersonToSave").Visible = false;
				newPassenger.GetNode<Node3D>("Person").GetNode<Node3D>("SpaceBloblin").Visible = true;
				break;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
