using Godot;
using System;
using System.Collections.Generic;

public partial class SpaceShip : CharacterBody3D
{
	[Export] public float MaxSpeed { get; set; } = 150.0f;
	[Export] public float Acceleration { get; set; } = 1f;
	[Export] public float Friction { get; set; } = 10.0f;
	[Export] public float CameraSensitivity { get; set; } = 0.002f;
	[Export] public float TurnSpeed { get; set; } = 1.5f;

	private float speed = 0;
	private Node3D _head;
	private Camera3D _camera;
	private Timer shipTimer;

	private Player player;

	private MeshInstance3D rope;
	private ImmediateMesh ropeMesh;

	private Node2D HUD;
	Label transmissionLabel;
	Timer transmissionTimer;
	AudioStreamPlayer transmissionSound;
	List<Passenger> passengers = new List<Passenger>();

	int MaxPassengers = 4;

	public enum GameMode
	{
		timeTrial
	}
	public override void _Ready()
	{
		_head = GetNode<Node3D>("Head");
		shipTimer = GetNode<Timer>("ShipTimer");
		_camera = _head.GetNode<Camera3D>("Camera");
		player = GetNode<Player>("Player");
		rope = GetNode<MeshInstance3D>("Rope");
		ropeMesh = rope.Mesh as ImmediateMesh;
		player.Visible = false;
		player.SetPhysicsProcess(false);
		Input.MouseMode = Input.MouseModeEnum.Captured;

		HUD = GetNode<CanvasLayer>("CanvasLayer").GetNode<Node2D>("HUD");
		transmissionLabel = HUD.GetNode<Control>("ReferenceRect").GetNode<Label>("Transmission");
		transmissionTimer = transmissionLabel.GetNode<Timer>("TransmissionTimer");
		transmissionSound = transmissionLabel.GetNode<AudioStreamPlayer>("TransmissionSound");
		transmissionTimer.Timeout += MakeMessage;

		// PackedScene a = GD.Load<PackedScene>("res://Scenes/Objectives/StrandedPerson.tscn");
		// Passenger newPassenger = a.Instantiate<Passenger>();
		// newPassenger.instantiatePassenger(GD.Load<Texture2D>("res://icon.svg"), Passenger.Destinations.SpaceStationTheta);
		// Passenger newPassenger2 = a.Instantiate<Passenger>();
		// newPassenger2.instantiatePassenger(GD.Load<Texture2D>("res://icon.svg"), Passenger.Destinations.SpaceStationTheta);
		// passengers.Add(newPassenger);
		// passengers.Add(newPassenger2);
	}
	public override void _PhysicsProcess(double delta)
	{
		float d = (float)delta;

		float turnInput = Input.GetAxis("MoveRight", "MoveLeft");

		if (turnInput != 0)
		{
			// This rotates the ship around its own Up axis
			RotateY(turnInput * TurnSpeed * d);
		}

		Vector3 localInput = Vector3.Zero;

		localInput.X = Input.GetAxis("MoveBack", "MoveForward");

		localInput.Y = Input.GetActionStrength("MoveUp") - Input.GetActionStrength("MoveDown");

		Vector3 worldDirection = GlobalTransform.Basis * localInput;

		//normalize so diagonal movement isn't faster
		if (worldDirection.Length() > 0)
		{
			worldDirection = worldDirection.Normalized();

		}
		// 3. APPLY VELOCITY
		Vector3 targetVelocity = worldDirection * MaxSpeed;

		if (worldDirection != Vector3.Zero)
		{
			Velocity = Velocity.Lerp(targetVelocity, Acceleration * d);
		}
		else
		{
			Velocity = Velocity.Lerp(Vector3.Zero, Friction * d);
		}
		speed = Velocity.Dot(GlobalTransform.Basis.X.Normalized());
		MoveAndSlide();
		_camera.Fov = Mathf.Lerp(_camera.Fov, 70 + speed * 0.3f, 0.1f);
		HUD.GetNode<Control>("ReferenceRect").GetNode<Label>("Speed").Text = "Current Speed: " + speed.ToString("F2") + " m/s";

		if (Input.IsActionJustPressed("EnterShip"))
		{
			shipTimer.Start();
		}
		else if (Input.IsActionJustReleased("EnterShip"))
		{
			shipTimer.Stop();
		}
		Control thetaIcon = HUD.GetNode<Control>("ReferenceRect").GetNode<Control>("SpaceStationTheta");
		thetaIcon.GetChildren().Clear();
		int SpaceStationThetaCount = 0;
		//----Passenger HUD
		GD.Print(passengers.Count);
		foreach (Passenger passenger in passengers)
		{
			switch (passenger.destination)
			{
				case Passenger.Destinations.SpaceStationTheta:
					thetaIcon = HUD.GetNode<Control>("ReferenceRect").GetNode<Control>("SpaceStationTheta");
					thetaIcon.Visible = true;
					SpaceStationThetaCount++;
					TextureRect thetaTexture = new TextureRect();
					thetaTexture.Texture = passenger.passengerTexture;
					thetaIcon.AddChild(thetaTexture);
					thetaTexture.Position = new Vector2(0, (SpaceStationThetaCount) * 125);
					break;
				case Passenger.Destinations.Planet:

					break;
				case Passenger.Destinations.SpaceShip:

					break;
				default:

					break;
			}

		}

		if (currentObjective == Objectives.DeliverToStation && passengers.Count > 0)
		{
			Control arrow = HUD.GetNode<Control>("ReferenceRect").GetNode<Control>("Arrow");
			var station = GetParent().GetNode<Node3D>("SpaceStationTheta2").GlobalPosition;
			var shipPos = GlobalPosition;
			station.Y = 0;
			shipPos.Y = 0;

			arrow.Rotation = 0;
		}
	}

	public override void _Input(InputEvent @event)
	{
		// Vector2 inputCam = Input.GetVector("camera_left", "camera_right", "camera_up", "camera_down");
		if (@event is InputEventMouseMotion mouseMotion)
		{

			_head.RotateY(-mouseMotion.Relative.X * CameraSensitivity);
			_camera.RotateObjectLocal(Vector3.Right, -mouseMotion.Relative.Y * CameraSensitivity);

			Vector3 headRotation = _head.Rotation;
			headRotation.Y = Mathf.Clamp(headRotation.Y, Mathf.DegToRad(-70), Mathf.DegToRad(70));
			_head.Rotation = headRotation;



			Vector3 camera3Drot = _camera.Rotation;
			camera3Drot.X = Mathf.Clamp(camera3Drot.X, Mathf.DegToRad(-40), Mathf.DegToRad(40));
			_camera.Rotation = camera3Drot;

		}
		else if (@event is InputEventKey keyEvent && keyEvent.IsPressed() && keyEvent.Keycode == Key.Escape)
		{


			Input.MouseMode = Input.MouseModeEnum.Visible;
			// GetParent().GetNode<Control>("Settings").Visible = true;
			GetTree().Paused = true;

		}
	}
	bool inShip = true;
	public void ExitShip()
	{
		var ropeLength = (new Vector3(0, 0, -1.252f) - new Vector3(player.Position.X, player.Position.Y - .5f, player.Position.Z)).Length();

		if (inShip)
		{
			inShip = false;
			GD.Print("Exiting Ship");
			player.Position = new Vector3(0, 0, -2.265f);
			player.Visible = true;
			player.GetNode<CanvasLayer>("CanvasLayer").Visible = true;
			player.SetPhysicsProcess(true);
			player.GetNode<Camera3D>("Camera3D").Current = true;
			GetNode<CanvasLayer>("CanvasLayer").Visible = false;

			SetPhysicsProcess(false);
			rope.Visible = true;
		}
		else
		{
			if (ropeLength > 5)
			{
				return;
			}
			inShip = true;
			GD.Print("Entering Ship");
			player.Visible = false;
			player.SetPhysicsProcess(false);
			player.GetNode<CanvasLayer>("CanvasLayer").Visible = false;
			GetNode<CanvasLayer>("CanvasLayer").Visible = true;
			_camera.Current = true;
			SetPhysicsProcess(true);
			rope.Visible = false;
			ropeMesh.ClearSurfaces();
		}

	}

	int messageIndex = 0;
	const int charsOnScreen = 35;
	char[] messageChars;
	public void DisplayTransmission(String message)
	{
		transmissionSound.Play();
		transmissionLabel.Text = "Receiving Data...";
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
			GetTree().CreateTimer(2.0f).Timeout += () => transmissionLabel.Hide();

		}
	}

	public void Docking(Area3D targetDockingPoint)
	{
		//Fly towards station, then when close enough, snap to station and disable movement until undocked
		SetPhysicsProcess(false);

		// Create the tween
		Tween tween = GetTree().CreateTween().SetParallel(true);

		// EaseOut means it starts fast and slows down as it reaches the port
		tween.SetTrans(Tween.TransitionType.Cubic);
		tween.SetEase(Tween.EaseType.Out);

		// Slide to position and match rotation
		tween.TweenProperty(this, "global_position", targetDockingPoint.GlobalPosition, 3.0f);
		tween.TweenProperty(this, "global_rotation", targetDockingPoint.GlobalRotation, 3.0f);
		RemoveAllPassengers();
		tween.Finished += () => SetPhysicsProcess(true);
	}

	public void RemoveAllPassengers()
	{
		if (passengers.Count > 0)
		{
			DisplayTransmission("Thank you for the ride Guardian");
			foreach (Passenger passenger in passengers)
			{
				passenger.QueueFree();
			}
			passengers.Clear();
			Control thetaIcon = HUD.GetNode<Control>("ReferenceRect").GetNode<Control>("SpaceStationTheta");
			thetaIcon.Visible = false;
			thetaIcon.GetChildren().Clear();
			currentObjective = Objectives.RescueStrandedPerson;
		}

	}

	public void pickUpPassenger(Passenger passenger)
	{
		passengers.Add(passenger);
		passenger.Visible = false;
		passenger.GetNode<RigidBody3D>("Person").GetNode<Area3D>("Area3D").SetDeferred("monitorable",false);
		// passenger.GetParent().RemoveChild(passenger);
		// AddChild(passenger);
		currentObjective = Objectives.DeliverToStation;
		DisplayTransmission(passenger.message);
	}

	enum Objectives
	{
		RescueStrandedPerson,
		DeliverToStation,

	}

	private Objectives currentObjective = Objectives.DeliverToStation;

	public void passenger(Area3D passengerArea)
	{
		GD.Print("Passenger Area Triggered");
		if (passengerArea.GetParent().GetParent() is Passenger passenger && passengers.Count < MaxPassengers)
		{
			pickUpPassenger(passenger);
		}

	}
}
