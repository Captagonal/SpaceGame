using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export]
	public float Acelleration { get; set; } = .5f;
	[Export]
	public float VerticalAcelleration { get; set; } = .4f;
	public float MaxSpeed { get; set; } = 2;
	public float CameraSensitivity { get; set; } = .006f;

	public float currentSpeed = 0;
	public Camera3D camera3D;
	private Timer shipTimer;

	private MeshInstance3D rope;
	private ImmediateMesh ropeMesh;

	private float MAXropeLength = 14;
	private float ropeLength = 0;

	public override void _Ready()
	{
		camera3D = GetNode<Camera3D>("Camera3D");
		shipTimer = GetParent().GetNode<Timer>("ShipTimer");
		rope = GetParent().GetNode<MeshInstance3D>("Rope");
		ropeMesh = rope.Mesh as ImmediateMesh;
	}
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;



		Vector2 input = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBack") * Acelleration;

		float inputY = (Input.GetActionStrength("MoveUp") - Input.GetActionStrength("MoveDown")) * VerticalAcelleration;

		Vector3 input3 = (this.GlobalTransform.Basis * new Vector3(input.X, inputY, input.Y));

		input3 = input3.Normalized() * Math.Min(input3.Length(), MaxSpeed);


		Velocity = Velocity += input3 * (float)delta;

		currentSpeed = Velocity.Length();
		MoveAndSlide();

		if (Input.IsActionPressed("Stop"))
		{
			Velocity = Velocity.Lerp(Vector3.Zero, 0.5f * (float)delta);
			//testing
		}

		if (Input.IsActionJustPressed("EnterShip"))
		{
			shipTimer.Start();
		}
		else if (Input.IsActionJustReleased("EnterShip"))
		{
			shipTimer.Stop();
		}

		ropeMesh.ClearSurfaces();
		ropeMesh.SurfaceBegin(Mesh.PrimitiveType.Lines);
		ropeMesh.SurfaceSetNormal(Vector3.Up);
		ropeMesh.SurfaceSetUV(Vector2.Zero);
		ropeMesh.SurfaceAddVertex(new Vector3(0,0,-1.252f));
		// GD.Print("Player Position: " + GetParent<Node3D>().GlobalTransform.Origin);
		ropeMesh.SurfaceSetNormal(Vector3.Up);
		ropeMesh.SurfaceSetUV(Vector2.Zero);
		ropeMesh.SurfaceAddVertex(new Vector3(Position.X, Position.Y-.5f, Position.Z));
		// GD.Print("Rope End Position: " + Position);
		ropeMesh.SurfaceEnd();
		ropeLength = (new Vector3(0, 0, -1.252f) - new Vector3(Position.X, Position.Y - .5f, Position.Z)).Length();
		if (ropeLength > MAXropeLength)
		{
			Vector3 ropeDirection = (new Vector3(0, 0, -1.252f) - new Vector3(Position.X, Position.Y - .5f, Position.Z)).Normalized();
			Vector3 targetPosition = new Vector3(0, 0, -1.252f) - ropeDirection * MAXropeLength;
			Position = targetPosition + new Vector3(0, .5f, 0);
			Velocity = Velocity.Lerp(Vector3.Zero, 0.5f * (float)delta);
		}

	}

	public override void _Input(InputEvent @event)
	{
		// Vector2 inputCam = Input.GetVector("camera_left", "camera_right", "camera_up", "camera_down");
		if (@event is InputEventMouseMotion mouseMotion)
		{


			this.RotateY(-mouseMotion.Relative.X * CameraSensitivity);
			// this.RotateX(-mouseMotion.Relative.Y * CameraSensitivity);
			RotateObjectLocal(Vector3.Right, -mouseMotion.Relative.Y * CameraSensitivity);
			Vector3 camRotation = this.Rotation;

			// camRotation.X = Mathf.Clamp(camRotation.X, Mathf.DegToRad(-80f), Mathf.DegToRad(80f));

			this.Rotation = camRotation;
		}
		else if (@event is InputEventKey keyEvent && keyEvent.IsPressed() && keyEvent.Keycode == Key.Escape)
		{


			Input.MouseMode = Input.MouseModeEnum.Visible;
			// GetParent().GetNode<Control>("Settings").Visible = true;
			GetTree().Paused = true;

		}
	}
}
