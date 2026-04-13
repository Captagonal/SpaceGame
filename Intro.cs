using Godot;
using System;

public partial class Intro : Control
{
	VideoStreamPlayer videoPlayer2;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		videoPlayer2 = GetNode<VideoStreamPlayer>("2");
		var config = new ConfigFile();

		// Load data from a file.
		Error err = config.Load("user://config.cfg");

		// If the file didn't load, ignore it.
		if (err != Error.Ok)
		{
			return;
		}

		var audioBus = AudioServer.GetBusIndex("Master");

		AudioServer.SetBusVolumeDb(audioBus, Mathf.LinearToDb((float)config.GetValue("Audio", "Volume", 1f)));
		transmissionLabel = GetNode<Label>("Transmission");
		transmissionTimer = transmissionLabel.GetNode<Timer>("TransmissionTimer");
		transmissionSound = transmissionLabel.GetNode<AudioStreamPlayer>("TransmissionSound");
		transmissionTimer.Timeout += MakeMessage;
		transmissionLabel.Text = "";
		DisplayTransmission("Welcome to your new job");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	public void part1Done()
	{
		videoPlayer2.Play();
	}
	public void introFinished()
	{
		GetTree().CreateTimer(5).Timeout += skipIntro;
	}
	public void skipIntro()
	{
		GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
	}

	int messageIndex = 0;
	const int charsOnScreen = 17;
	char[] messageChars;
	Label transmissionLabel;
	Timer transmissionTimer;
	AudioStreamPlayer transmissionSound;
	public void DisplayTransmission(String message)
	{
		transmissionSound.Play();
		// 
		transmissionLabel.Show();

		transmissionTimer.Start();
		messageChars = message.ToCharArray();
		messageIndex = 0;


	}

	public void MakeMessage()
	{
		// GD.Print("Transmission Timer Tick");
		// GD.Print("Current Transmission Text: " + transmissionLabel.Text);
		// GD.Print("Message Index: " + messageIndex);
		if (messageIndex < messageChars.Length)
		{
			messageIndex++;
			transmissionLabel.Text += messageChars[messageIndex - 1];
			if (transmissionLabel.Text.Length > charsOnScreen)
			{
				transmissionLabel.Text = transmissionLabel.Text.Substring(transmissionLabel.Text.Length - charsOnScreen);
			}
		}
		else
		{
			transmissionTimer.Stop();
			// transmissionLabel.Hide();
		}
	}
	int textIndex = 0;
	public void nextText(){
	switch(textIndex){
		case 0:
			DisplayTransmission("  With the advent of readily available space travel tons of new jobs opened up");
			break;
		case 1:
			DisplayTransmission("  Some of them of them feel like small deals but really your saving lives");
			break;
		case 2:
			DisplayTransmission("  Welcome to the Space Guard");
			break;
		default:
			introFinished();
			break;
	}
	textIndex++;
	}

	public void StepTimer(){
		GetNode<AudioStreamPlayer>("Step").Play();
	}
	int notifIndex = 0;
	public void notifTimer(){
		notifIndex++;
		if (notifIndex > 8){
			return;
		}
		GetNode<AudioStreamPlayer>("Notif").Play();
		GetTree().CreateTimer(.2f).Timeout += notifTimer;
	}
}
