using Godot;
using System;
using System.Threading.Tasks;

public partial class Player : CharacterBody3D
{
	// Settings
	[Export]
	public float Speed = 0.06f;
	[Export]
	public float MaxTrotSpeed = 1f;
	[Export]
	public float MaxGallopSpeed = 2f;
	[Export]
	public float MaxDashSpeed = 3f;
	[Export]
	public float HighJumpForce = 0.17f;
	[Export]
	public float JumpForce = 0.12f;
	[Export]
	public float Gravity = 0.35f;
	[Export]
	public float GravityTerminalSpeed = 1.4f;
	[Export]
	public float TurningSpeed = 5f;

	// Physical State
	private float P_YVelo = 0.0f;
	private float P_XVelo = 0.0f;
	private bool P_FacingRight = true;
	private float P_Facing = -90f;
	private bool P_Jumping = false;
	private bool P_HighJumping = false;
	private Vector3 P_PreviousPosition;
	private Vector3 P_SpeedTendancy;
	private Vector3 P_SpeedActual;
	private float P_GroundAngle = 0;

	// Component References
	private Node3D PlayerModel;
	private AnimationPlayer anim;
	private AnimationTree AnimTree;

	public struct InuAnim {
		public const string IDLE = "Armature|Idle";
		public const string TROT = "Armature|Trot";
		public const string GALLOP = "Armature|Gallop";
		public const string JUMP = "Armature|Jump";
		public const string HIGHJUMP = "Armature|HighJump";
	}

	public override void _Ready()
	{
		anim = GetNode<AnimationPlayer>("InuModel/AnimationPlayer");
		AnimTree = GetNode<AnimationTree>("InuModel/AnimationTree");
		AnimTree.Active = true;
		PlayerModel = GetNode<Node3D>("InuModel");

		P_PreviousPosition = GlobalPosition;
	}

	public override void _PhysicsProcess(double delta)
	{
		var move_dir = 0.0f;
		var player_movement = Input.GetAxis("player_left", "player_right");
		move_dir += player_movement;

		var maxSpeed = Input.IsActionPressed("player_dash") ? MaxGallopSpeed : MaxTrotSpeed;

		if (Math.Abs(player_movement) > 0) {


			if (P_XVelo > maxSpeed + 0.01f) {
				P_XVelo -= 1.5f * (float)delta;
			} else if (P_XVelo < maxSpeed) {
				P_XVelo += 2f * (float)delta;
			} else {
				P_XVelo = maxSpeed;
			}
		} else {
			P_XVelo -= 3f * (float)delta;

			if (P_XVelo < 0) {
				P_XVelo = 0;
			}
		}

		// Air Dash
		// if (P_Jumping && Input.IsActionPressed("player_dash"))
		// {
		// 	P_XVelo += 0.5f;

		// 	if (P_XVelo > MaxDashSpeed)
		// 	{
		// 		P_XVelo = MaxDashSpeed;
		// 	}
		// }
		// Normal Long Jump
		if (P_Jumping && !P_HighJumping && Input.IsActionPressed("player_dash"))
		{
			P_XVelo += 0.5f;

			if (P_XVelo > MaxDashSpeed)
			{
				P_XVelo = MaxDashSpeed;
			}
		}

		if (move_dir == 0) {
			P_SpeedTendancy = Speed * Transform.Basis.X * P_XVelo * (P_FacingRight ? 1 : -1);
		} else {
			P_SpeedTendancy = move_dir * Speed * Transform.Basis.X * P_XVelo;
		}

		P_YVelo -= Gravity * (float)delta;

		if (P_YVelo < -GravityTerminalSpeed) {
			P_YVelo = -GravityTerminalSpeed;
		}

		P_SpeedActual = GlobalPosition - P_PreviousPosition;
		P_PreviousPosition = Position;

		if (IsOnFloor()) {
			P_YVelo = 0.0f;
			if (Input.IsActionPressed("player_jump")) {
				if (Input.IsActionPressed("player_dash") ) {
					if (Math.Abs(GetRealVelocity().X) > 0) {
						P_YVelo = HighJumpForce;
						P_HighJumping = true;
					} else {
						P_HighJumping = false;
						P_YVelo = JumpForce;
					}

				} else {
					P_HighJumping = true;
					P_YVelo = HighJumpForce;
				}

				P_Jumping = true;
				var _ = WaitForJumpAnimationToFinish();
			}
		}

		if (move_dir < 0 && P_FacingRight)
		{
			P_FacingRight = !P_FacingRight;
		}

		if (move_dir > 0 && !P_FacingRight)
		{
			P_FacingRight = !P_FacingRight;
		}

		if (P_FacingRight)
		{
			P_Facing = Mathf.Lerp(P_Facing, -90f, (float)delta * TurningSpeed);
			if (IsOnFloor()) {
				P_GroundAngle = Mathf.Lerp(P_GroundAngle, -GetFloorNormal().X, (float)delta * TurningSpeed);
			}
		}
		else
		{
			P_Facing = Mathf.Lerp(P_Facing, -270f, (float)delta * TurningSpeed);
			if (IsOnFloor()) {
				P_GroundAngle = Mathf.Lerp(P_GroundAngle, GetFloorNormal().X, (float)delta * TurningSpeed);
			}
		}

		this.Position += P_SpeedTendancy;
		this.Position += new Vector3(0, P_YVelo, 0);

		PlayerModel.RotationDegrees = new Vector3(P_GroundAngle * 57.295f, P_Facing, 0);

		MoveAndSlide();
	}

	public override void _Process(double delta)
	{
		ProcessAnimation();
	}

	private async Task WaitForJumpAnimationToFinish() {
		await Task.Delay(P_HighJumping ? 900 : 600);
		P_Jumping = false;
	}

	private float MapRange(float value, float inputMin, float inputMax, float outputMin, float outputMax)
	{
		return outputMin + (value-inputMin)*(outputMax-outputMin)/(inputMax-inputMin);
	}

	private void ProcessAnimation() {
		var lateralVelocity = Mathf.Abs(P_SpeedActual.X);
		if (lateralVelocity > 0.1f) {
			lateralVelocity = 0.1f;
		}
		if (lateralVelocity == 0) {
			AnimTree.Set("parameters/conditions/idle", true);
			AnimTree.Set("parameters/conditions/is_moving", false);
		} else {
			AnimTree.Set("parameters/conditions/idle", false);
			AnimTree.Set("parameters/conditions/is_moving", true);
			AnimTree.Set("parameters/Moving/blend_position", new Vector2(lateralVelocity, 0));
		}

		AnimTree.Set("parameters/conditions/jump", P_Jumping);

		if (P_Jumping) {
			AnimTree.Set("parameters/Jump/blend_position", new Vector2(0, lateralVelocity));
		}
	}

	private Transform3D AlignWithGround(Transform3D transform, Vector3 ground_y) {
		transform.Basis.Y = ground_y;
		transform.Basis = transform.Basis.Orthonormalized();
		return transform;
	}
}
