using Godot;
using System;

public partial class MainCamera : Camera3D
{
	private Node3D target;
	private float smooth_speed = 3f;
	private Vector3 offset;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		offset = new Vector3(0, 2, 5);
		target = GetParent().GetNode<Node3D>("Player");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (target != null) {
			Position = Lerp(Position, target.Position + offset, (float)delta * smooth_speed);
		}
	}

	private Vector3 Lerp(Vector3 firstFloat, Vector3 secondFloat, float by)
	{
		return firstFloat * (1 - by) + secondFloat * by;
	}
}
