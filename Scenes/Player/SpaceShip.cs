using Godot;
using System;

public partial class SpaceShip : CharacterBody3D
{
	[Export] public float MaxSpeed { get; set; } = 100.0f;
	[Export] public float Acceleration { get; set; } = 1.0f;
	[Export] public float Friction { get; set; } = 10.0f;
	[Export] public float CameraSensitivity { get; set; } = 0.002f;
	[Export] public float TurnSpeed { get; set; } = 1.5f;

	private float speed = 0;
	private Node3D _head;
	private Camera3D _camera;
	private Timer shipTimer;

	private Player player;

	public override void _Ready()
	{
		_head = GetNode<Node3D>("Head");
		shipTimer = GetNode<Timer>("ShipTimer");
		_camera = _head.GetNode<Camera3D>("Camera");
		player = GetNode<Player>("Player");
		player.Visible = false;
		player.SetPhysicsProcess(false);
		Input.MouseMode = Input.MouseModeEnum.Captured;
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

		if (Input.IsActionJustPressed("EnterShip"))
		{
			shipTimer.Start();
		}
		else if (Input.IsActionJustReleased("EnterShip"))
		{
			shipTimer.Stop();
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
	Boolean inShip = true;
	public void ExitShip()
	{
		if (inShip)
		{
			inShip = false;
			GD.Print("Exiting Ship");
			player.GlobalTransform = this.GlobalTransform.Translated(this.GlobalTransform.Basis.X * 2);
			player.Visible = true;
			player.SetPhysicsProcess(true);
			player.GetNode<Camera3D>("Camera3D").Current = true;
			SetPhysicsProcess(false);
		} else {
			inShip = true;
			GD.Print("Entering Ship");
			player.Visible = false;
			player.SetPhysicsProcess(false);
			_camera.Current = true;
			SetPhysicsProcess(true);
		}

	}
}
