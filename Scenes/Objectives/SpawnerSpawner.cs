using Godot;
using System;

public partial class SpawnerSpawner : Node3D
{
	// Called when the node enters the scene tree for the first time.
	int NumSpawners = 200;
	int minx = -50;
	int maxx = 50;
	int minz = -50;
	int maxz = 50;
	int miny = -50;
	int maxy = 50;
	PackedScene spawnerScene = GD.Load<PackedScene>("res://Scenes/Objectives/Spawner.tscn");

	public override void _Ready()
	{
		for (int i = 0; i < NumSpawners; i++)
		{
			Spawner spawner = spawnerScene.Instantiate<Spawner>();
			AddChild(spawner);
			spawner.Position = getRandomSpot();
		}
	}

	public Vector3 getRandomSpot(){
		float x =(float) GD.RandRange(-1500,1500);
		float y =(float) GD.RandRange(-1500,1500);
		float z =(float) GD.RandRange(-1500,1500);
		return new Vector3(x,y,z);
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
