using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class MainMenu : Control
{
	private Label title;
	private Label scoreBoard;
	const string SavePath = "user://scores.save";
	private Texture2D[] backgrounds = {
		GD.Load<Texture2D>("res://Assets/Textures/SpaceStationDelta.png"),
		GD.Load<Texture2D>("res://Assets/Textures/SpaceStationOmega.png"),
	};
	//Top Scores
	// 1: Sawyer: 1102
	// 2: You: 223
	// 3: You: 234
	// 4:
	// 5:
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		title = GetNode<Label>("Title");
		scoreBoard = GetNode<Label>("ScoreBoard");
		scoreBoard.Text = "";
		scoreBoard.Text += "Top Scores:\n";
		loadScores();
		 
		 var backgroundSprite = GetNode<TextureRect>("Background");
		 backgroundSprite.Texture = backgrounds[GD.Randi() % backgrounds.Length];
	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void start()
	{
		GetTree().ChangeSceneToFile("res://Scenes/TestLevel.tscn");
	}

	public void quit()
	{
		GetTree().Quit();
	}

	public void settings()
	{
		GetTree().ChangeSceneToFile("res://Scenes/Settings.tscn");
	}
	public void loadScores()
	{
		if (!FileAccess.FileExists(SavePath)) return;

		using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
		var saveData = (Dictionary)file.GetVar();

		if (!saveData.ContainsKey("scores"))
		{
			return;
		}

		var scoreList = (Godot.Collections.Array)saveData["scores"];

		var sortedList = scoreList.Select(x => (Godot.Collections.Dictionary)x)
						  .OrderByDescending(x => (int)x["score"])
						  .ToList();


		int count = Mathf.Min(scoreList.Count, 5);
		for (int i = 0; i < count; i++)
		{
			string name = sortedList[i]["name"].ToString();
			int val = (int)sortedList[i]["score"];

			scoreBoard.Text += $"{i + 1}. {name} - {val}\n";
		}
	}
}
