using Godot;
using System;
 
public partial class Passenger : Node3D
{
	public Vector3 randomDirection;
	public enum Destinations
	{
		SpaceStationTheta,
		SpaceStationDelta,
		SpaceStationOmega,
		// Planet,
		// SpaceShip
	}
		Timer timerScale;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timerScale = GetNode<Timer>("Timer");
		meshInstance = GetNode<RigidBody3D>("Person").GetNode<MeshInstance3D>("MeshInstance3D2");
		timerScale.Timeout += timer;
		randomDirection = new Vector3((float)(GD.Randf() - 0.5), (float)(GD.Randf() - 0.5), (float)(GD.Randf() - 0.5)).Normalized();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GetNode<Node3D>("Person").GetNode<Node3D>("SpaceBloblin").RotateObjectLocal(randomDirection, (float)delta);
		GetNode<Node3D>("Person").GetNode<Node3D>("PersonToSave").RotateObjectLocal(randomDirection, (float)delta);
	}

	public Texture2D passengerTexture = GD.Load<Texture2D>("res://Assets/Textures/Bloblin.png");
	public Destinations destination = Destinations.SpaceStationDelta;
	public String message = "Hello :3";

	public Vector3 startPosition = new Vector3(0, 0, 0);
	
	MeshInstance3D meshInstance;
	public void instantiatePassenger(Texture2D texture, Destinations destination, String message){
	
		this.passengerTexture = texture;
		this.destination = destination;
		this.message = message;
		startPosition = this.GlobalTransform.Origin;

	}
	bool big = false;
	public void timer(){
		Node3D Notifer = GetNode<Node3D>("Person").GetNode<Node3D>("MeshInstance3D2");
		big = !big;
		Tween tween = GetTree().CreateTween().SetParallel(true);

		// EaseOut means it starts fast and slows down as it reaches the port
		tween.SetTrans(Tween.TransitionType.Linear);
		tween.SetEase(Tween.EaseType.InOut);
		if (big){
			tween.TweenProperty(Notifer, "scale", new Vector3(2,2,2), timerScale.WaitTime);
		}
		else{
			tween.TweenProperty(Notifer, "scale", new Vector3(1,1,1), timerScale.WaitTime);
		}
	}

	public int getPoints(Vector3 currentPosition){
		switch (destination){
			case Destinations.SpaceStationTheta:
				return (int)currentPosition.DistanceTo(startPosition);
				break;
			case Destinations.SpaceStationDelta:
				return (int)currentPosition.DistanceTo(startPosition);
				break;
			case Destinations.SpaceStationOmega:
				return (int)currentPosition.DistanceTo(startPosition);
				break;
			// case Destinations.Planet:
			// 	//add points for planet
			// 	break;
			// case Destinations.SpaceShip:
			// 	//add points for spaceship
			// 	break;
		}
		return 1;
	}

	public void PickedUp(){
		RigidBody3D rigidBody3D = GetNode<RigidBody3D>("Person");
		rigidBody3D.GetNode<Area3D>("Area3D").SetDeferred("monitorable", false);
		rigidBody3D.GetNode<Area3D>("Area3D").SetDeferred("monitoring", false);
		// rigidBody3D.GetNode<CollisionShape3D>("CollisionShape3D").SetDeferred("disabled", true);
		rigidBody3D.GetNode<MeshInstance3D>("MeshInstance3D2").SetDeferred("visible", false);
		rigidBody3D.Freeze = true;
		Tween tween = GetTree().CreateTween().SetParallel(true);
		tween.SetTrans(Tween.TransitionType.Linear);
		tween.SetEase(Tween.EaseType.InOut);
		tween.TweenProperty(this, "scale", new Vector3(.1f,.1f,.1f), 3);
		Tween tween2 = GetTree().CreateTween().SetParallel(true);
		tween2.SetTrans(Tween.TransitionType.Linear);
		tween2.SetEase(Tween.EaseType.InOut);
		tween2.TweenProperty(this, "rotation", new Vector3(20,0,0), 3);
		Tween tween3 = GetTree().CreateTween().SetParallel(true);
		tween3.SetTrans(Tween.TransitionType.Linear);
		tween3.SetEase(Tween.EaseType.InOut);
		tween3.TweenProperty(this, "position", new Vector3(0,1,0), 3);
		GetTree().CreateTimer(3).Timeout += () => {
			Visible = false;
		};
	}
}
