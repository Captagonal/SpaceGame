using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

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
		if (currentGameMode == GameMode.timeTrial)
		{
			Label timer = HUD.GetNode<Control>("ReferenceRect").GetNode<Label>("Time");
			timer.Visible = true;
			Timer time = timer.GetNode<Timer>("TimeTrial");
			time.Timeout += () => timeTrialEnd();
		}
	}

	public void timeTrialEnd()
	{
		GD.Print("Time Trial Ended! Final Score: " + score);
		Label timer = HUD.GetNode<Control>("ReferenceRect").GetNode<Label>("Time");
		timer.Visible = true;
		Timer time = timer.GetNode<Timer>("TimeTrial");
		time.Stop();
	}
	float time = 0;
	public override void _PhysicsProcess(double delta)
	{
		float d = (float)delta;
		time += d;
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
		Control deltaIcon = HUD.GetNode<Control>("ReferenceRect").GetNode<Control>("SpaceStationDelta");
		deltaIcon.GetChildren().Clear();
		Control omegaIcon = HUD.GetNode<Control>("ReferenceRect").GetNode<Control>("SpaceStationOmega");
		omegaIcon.GetChildren().Clear();
		int SpaceStationThetaCount = 0;
		int SpaceStationDeltaCount = 0;
		int SpaceStationOmegaCount = 0;
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
					thetaTexture.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
					thetaTexture.Size = new Vector2(100, 100);
					break;
				case Passenger.Destinations.SpaceStationDelta:
					deltaIcon = HUD.GetNode<Control>("ReferenceRect").GetNode<Control>("SpaceStationDelta");
					deltaIcon.Visible = true;
					SpaceStationDeltaCount++;
					TextureRect deltaTexture = new TextureRect();
					deltaTexture.Texture = passenger.passengerTexture;
					deltaIcon.AddChild(deltaTexture);
					deltaTexture.Position = new Vector2(0, (SpaceStationDeltaCount) * 125);
					deltaTexture.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
					deltaTexture.Size = new Vector2(100, 100);
					break;
				case Passenger.Destinations.SpaceStationOmega:
					omegaIcon = HUD.GetNode<Control>("ReferenceRect").GetNode<Control>("SpaceStationOmega");
					omegaIcon.Visible = true;
					SpaceStationOmegaCount++;
					TextureRect omegaTexture = new TextureRect();
					omegaTexture.Texture = passenger.passengerTexture;
					omegaIcon.AddChild(omegaTexture);
					omegaTexture.Position = new Vector2(0, (SpaceStationOmegaCount) * 125);
					omegaTexture.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
					omegaTexture.Size = new Vector2(100, 100);

					break;
				// case Passenger.Destinations.Planet:

				// 	break;
				// case Passenger.Destinations.SpaceShip:

				// 	break;
				default:

					break;
			}

		}

		if (currentObjective == Objectives.DeliverToStation && passengers.Count > 0)
		{
			Control arrow = HUD.GetNode<Control>("ReferenceRect").GetNode<Control>("Arrow");
			var station = GetParent().GetNode<Node3D>("SpaceStationTheta").GlobalPosition;
			var shipPos = GlobalPosition;
			station.Y = 0;
			shipPos.Y = 0;

			arrow.Rotation = 0;
		}
		if (currentGameMode == GameMode.timeTrial)
		{
			Label timer = HUD.GetNode<Control>("ReferenceRect").GetNode<Label>("Time");
			timer.Visible = true;
			Timer time = timer.GetNode<Timer>("TimeTrial");
			timer.Text = "Time: " + time.TimeLeft.ToString("F2") + "s";
			Label scoreDisplay = HUD.GetNode<Control>("ReferenceRect").GetNode<Label>("Score");
			scoreDisplay.Visible = true;
			scoreDisplay.Text = "Score: " + score.ToString();

		}
	}
	GameMode currentGameMode = GameMode.timeTrial;
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
			// GD.Print("Exiting Ship");
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
			// GD.Print("Entering Ship");
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
	int score;
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
		if (targetDockingPoint.GetParent().Name == "SpaceStationTheta")
		{
			RemoveAllPassengers(Passenger.Destinations.SpaceStationTheta);
		}
		else if (targetDockingPoint.GetParent().Name == "SpaceStationDelta")
		{
			RemoveAllPassengers(Passenger.Destinations.SpaceStationDelta);
		}
		else if (targetDockingPoint.GetParent().Name == "SpaceStationOmega")
		{
			RemoveAllPassengers(Passenger.Destinations.SpaceStationOmega);
		}

		tween.Finished += () => SetPhysicsProcess(true);
	}

	public void RemoveAllPassengers(Passenger.Destinations destinations)
	{
		if (passengers.Count == 0)
		{
			return;
		}
		for (int i = passengers.Count - 1; i >= 0; i--)
		{
			if (passengers[i].destination == destinations)
			{
				DisplayTransmission("Thanks for the ride!");
				score += passengers[i].getPoints(GlobalTransform.Origin);
				passengers[i].QueueFree();
				passengers.RemoveAt(i);
				Control icon = HUD.GetNode<Control>("ReferenceRect").GetNode<Control>(destinations.ToString());
				icon.GetChildren().Clear();
				icon.Visible = false;
			}
		}

		if (passengers.Count == 0)
		{
			currentObjective = Objectives.RescueStrandedPerson;
		}


	}

	public void pickUpPassenger(Passenger passenger)
	{
		passengers.Add(passenger);
		passenger.Visible = false;
		passenger.GetNode<RigidBody3D>("Person").GetNode<Area3D>("Area3D").SetDeferred("monitorable", false);
		// passenger.GetParent().RemoveChild(passenger);
		// AddChild(passenger);
		currentObjective = Objectives.DeliverToStation;
		Label timer = HUD.GetNode<Control>("ReferenceRect").GetNode<Label>("Time");
		timer.Visible = true;
		Timer timeTrial = timer.GetNode<Timer>("TimeTrial");

		timeTrial.WaitTime = timeTrial.TimeLeft + 20 - .15 * time;
		timeTrial.Start();
		DisplayTransmission(passenger.message);
	}


	public void PickUpPassenger2(Passenger passenger)
	{
		GD.Print("Attempting to pick up passenger...");
		if (passengers.Count >= MaxPassengers)
		{
			DisplayTransmission("Can't pick up passenger, ship is at max capacity!");
			return;
		}
		passenger.PickedUp();
		passengers.Add(passenger);
		DisplayTransmission(passenger.message);
		passenger.GetNode<RigidBody3D>("Person").Freeze = false;
		switch (passengers.Count){
			case 1:
				GetNode<Generic6DofJoint3D>("Generic6DOFJoint3D").NodeB = passenger.GetNode<RigidBody3D>("Person").GetPath();
				break;
			case 2:
				score += 250;
				break;
			case 3:
				score += 500;
				break;
			case 4:
				score += 1000;
				break;
		}

		currentObjective = Objectives.DeliverToStation;
		if (currentGameMode == GameMode.timeTrial)
		{
			Label timer = HUD.GetNode<Control>("ReferenceRect").GetNode<Label>("Time");
			timer.Visible = true;
			Timer timeTrial = timer.GetNode<Timer>("TimeTrial");
			timeTrial.WaitTime = timeTrial.TimeLeft + 20 - .15f * time;
			timeTrial.Start();
		}

	}

	enum Objectives
	{
		RescueStrandedPerson,
		DeliverToStation,

	}

	private Objectives currentObjective = Objectives.DeliverToStation;

	public void passenger(Area3D passengerArea)
	{
		// GD.Print("Passenger Area Triggered");
		if (passengerArea.GetParent().GetParent() is Passenger passenger && passengers.Count < MaxPassengers)
		{
			pickUpPassenger(passenger);
		}

	}
}
