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
	public String message = "Hello :3";

	public Vector3 startPosition = new Vector3(0, 0, 0);
	public void instantiatePassenger(Texture2D texture, Destinations destination, String message){
	
		this.passengerTexture = texture;
		this.destination = destination;
		this.message = message;
		startPosition = this.GlobalTransform.Origin;

	}

	public int getPoints(Vector3 currentPosition){
		switch (destination){
			case Destinations.SpaceStationTheta:
				return (int)currentPosition.DistanceTo(startPosition);
				break;
			case Destinations.Planet:
				//add points for planet
				break;
			case Destinations.SpaceShip:
				//add points for spaceship
				break;
		}
		return 1;
	}
}
