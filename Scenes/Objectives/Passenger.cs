using Godot;
using System;

public partial class Passenger : Node3D
{
	public enum Destinations
	{
		SpaceStationTheta,
		Planet,
		SpaceShip
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public Texture2D passengerTexture = GD.Load<Texture2D>("res://icon.svg");
	public Destinations destination = Destinations.SpaceStationTheta;
	public void instantiatePassenger(Texture2D texture, Destinations destination)
	{
		this.passengerTexture = texture;
		this.destination = destination;
		
	}
}
