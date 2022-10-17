using Sandbox;

namespace TowerOfKoba;

public partial class TKPlayer : Player
{
	public override void Spawn()
	{
		base.Respawn();
		
		SetModel( "models/citizen/citizen.vmdl" );

		Controller = new WalkController();
		Animator = new StandardPlayerAnimator();
		CameraMode = new ShoulderCamera();
		
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
		EnableAllCollisions = true;
	}

	public override void Respawn()
	{
		base.Respawn();

		CameraMode = new ShoulderCamera();

		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
		EnableAllCollisions = true;
	}

	public override void OnKilled()
	{
		base.OnKilled();

		CreateCorpse();
		CameraMode = new SpectateRagdollCamera();

		EnableAllCollisions = false;
		EnableDrawing = false;
	}

	//Creates a player ragdoll and set to delete later on
	public void CreateCorpse()
	{
		var corpse = new ModelEntity( GetModelName() );
		corpse.UsePhysicsCollision = true;
		corpse.PhysicsEnabled = true;

		corpse.Position = Position;
		corpse.Rotation = Rotation;
		corpse.Velocity = Velocity;

		corpse.SetupPhysicsFromModel( PhysicsMotionType.Dynamic );

		corpse.CopyBonesFrom( this );
		corpse.TakeDecalsFrom( this );
		corpse.SetRagdollVelocityFrom( this );
		corpse.DeleteAsync( 10.0f );

		Corpse = corpse;
	}
}
