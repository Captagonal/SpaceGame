using Godot;
using System;

public partial class Player : CharacterBody3D
{	
	[Export]
	public int Acelleration { get; set; } = 5;
	[Export]
	public int VerticalAcelleration { get; set; } = 3;
	public int MaxSpeed { get; set; } = 10;
	public float CameraSensitivity { get; set; } = .006f;
	public Camera3D camera3D;

	public override void _Ready()
	{
		camera3D = GetNode<Camera3D>("Camera3D");
	}
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

	

		Vector2 input = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBack")* Acelleration;

		float inputY = (Input.GetActionStrength("MoveUp") - Input.GetActionStrength("MoveDown"))* VerticalAcelleration;

		Vector3 input3 = (camera3D.GlobalTransform.Basis * new Vector3(input.X, inputY, input.Y));


		Velocity = Velocity += input3 * (float)delta;
		MoveAndSlide();
	}

	public override void _Input(InputEvent @event)
	{
		// Vector2 inputCam = Input.GetVector("camera_left", "camera_right", "camera_up", "camera_down");
		if (@event is InputEventMouseMotion mouseMotion)
		{


			this.RotateY(-mouseMotion.Relative.X * CameraSensitivity);
			camera3D.RotateX(-mouseMotion.Relative.Y * CameraSensitivity);
			Vector3 camRotation = camera3D.Rotation;

			// camRotation.X = Mathf.Clamp(camRotation.X, Mathf.DegToRad(-80f), Mathf.DegToRad(80f));

			camera3D.Rotation = camRotation;
		}
		else if (@event is InputEventKey keyEvent && keyEvent.IsPressed() && keyEvent.Keycode == Key.Escape)
		{
			
			
			Input.MouseMode = Input.MouseModeEnum.Visible;
			// GetParent().GetNode<Control>("Settings").Visible = true;
			GetTree().Paused = true;
			
		}
	}
}
