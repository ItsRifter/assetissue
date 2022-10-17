using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using TowerOfKoba.Assets;
using TowerOfKoba.Items;
using TowerOfKoba.NPC.Nav;

namespace TowerOfKoba.NPC;

public partial class BaseNPC : AnimatedEntity
{

	[ConVar.Replicated]
	public static bool rpg_nav_drawpath { get; set; }

	TKPlayer targetPlayer;
	TimeSince timeFoundPlayer;
	bool isInPursuit;

	Vector3 InputVelocity;
	Vector3 LookDir;

	[ConCmd.Admin( "tok_npc_clear" )]
	public static void ClearAllNPCs()
	{
		foreach ( var npc in All.OfType<BaseNPC>().ToArray() )
			npc.Delete();
	}

	[ConCmd.Admin( "tok_npc_killall" )]
	public static void KillAllNPCs()
	{
		foreach ( var npc in All.OfType<BaseNPC>().ToArray() )
			npc.TakeDamage( DamageInfo.Generic( 9999 ) );
	}

	public NavSteering Steer;

	NPCAsset npcAsset;

	public void SpawnFromAsset( NPCAsset asset )
	{
		npcAsset = asset;

		Health = npcAsset.BaseHealth;
		SetModel( npcAsset.ModelPath );
		Scale = npcAsset.ModelScale;

		Spawn();
	}

	public override void Spawn()
	{
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );

		EnableHitboxes = true;
		EnableLagCompensation = true;

		Steer = new NavWander();
	}

	[Event.Tick.Server]
	public void Tick()
	{
		InputVelocity = 0;

		Scale = npcAsset.ModelScale;
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );

		if ( Steer != null )
		{
			Steer.Tick( Position );

			if ( !Steer.Output.Finished )
			{
				InputVelocity = Steer.Output.Direction.Normal;
				Velocity = Velocity.AddClamped( InputVelocity * Time.Delta * 500, npcAsset.BaseSpeed );
			}

			if ( rpg_nav_drawpath )
			{
				Steer.DebugDrawPath();
			}
		}

		Move( Time.Delta );

		var walkVelocity = Velocity.WithZ( 0 );
		if ( walkVelocity.Length > 0.5f )
		{
			var turnSpeed = walkVelocity.Length.LerpInverse( 0, 100, true );
			var targetRotation = Rotation.LookAt( walkVelocity.Normal, Vector3.Up );
			Rotation = Rotation.Lerp( Rotation, targetRotation, turnSpeed * Time.Delta * 20.0f );
		}

		var animHelper = new CitizenAnimationHelper( this );

		LookDir = Vector3.Lerp( LookDir, InputVelocity.WithZ( 0 ) * 1000, Time.Delta * 100.0f );
		animHelper.WithLookAt( EyePosition + LookDir );
		animHelper.WithVelocity( Velocity );
		animHelper.WithWishVelocity( InputVelocity );
	}

	public virtual void OnAlert()
	{
		if ( isInPursuit )
			return;

		isInPursuit = true;
		//PlaySound( AlertSound );
		timeFoundPlayer = 0;
	}

	protected virtual void Move( float timeDelta )
	{
		var bbox = BBox.FromHeightAndRadius( 64, 4 );

		MoveHelper move = new( Position, Velocity );
		move.MaxStandableAngle = 50;
		move.Trace = move.Trace.Ignore( this ).Size( bbox );

		if ( !Velocity.IsNearlyZero( 0.001f ) )
		{
			move.TryUnstuck();
			move.TryMoveWithStep( timeDelta, 30 );
		}

		var tr = move.TraceDirection( Vector3.Down * 10.0f );

		if ( move.IsFloor( tr ) )
		{
			GroundEntity = tr.Entity;

			if ( !tr.StartedSolid )
			{
				move.Position = tr.EndPosition;
			}

			if ( InputVelocity.Length > 0 )
			{
				var movement = move.Velocity.Dot( InputVelocity.Normal );
				move.Velocity = move.Velocity - movement * InputVelocity.Normal;
				move.ApplyFriction( tr.Surface.Friction * 10.0f, timeDelta );
				move.Velocity += movement * InputVelocity.Normal;

				NPCDebugDraw.Once.Line( tr.StartPosition, tr.EndPosition );

			}
			else
			{
				move.ApplyFriction( tr.Surface.Friction * 10.0f, timeDelta );
			}


		}
		else
		{
			GroundEntity = null;
			move.Velocity += Vector3.Down * 900 * timeDelta;
			NPCDebugDraw.Once.WithColor( Color.Red ).Circle( Position, Vector3.Up, 10.0f );
		}

		Position = move.Position;
		Velocity = move.Velocity;
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		Rotation = Input.Rotation;
		EyeRotation = Rotation;

		var maxSpeed = 500;

		Velocity += Input.Rotation * new Vector3( Input.Forward, Input.Left, Input.Up ) * maxSpeed * 5 * Time.Delta;
		if ( Velocity.Length > maxSpeed ) Velocity = Velocity.Normal * maxSpeed;

		Velocity = Velocity.Approach( 0, Time.Delta * maxSpeed * 3 );

		Position += Velocity * Time.Delta;

		EyePosition = Position;
	}

	public override void TakeDamage( DamageInfo info )
	{
		Health -= info.Damage;

		if ( Health <= 0 )
		{
			//Give player XP based on reward
			if(info.Attacker is TKPlayer player)
				player.AddXP( npcAsset.XPReward );

			OnKilled();
		} 

	}

	public override void OnKilled()
	{
		base.OnKilled();

		Log.Info( npcAsset.DroppableItems.Length );

		if( npcAsset.DroppableItems.Length > 0)
			foreach ( var itemAsset in npcAsset.DroppableItems )
			{
				ItemBase item = new ItemBase();
				item.SpawnFromAsset( itemAsset );
				item.Position = Position;
			}
		
	}

	public override void FrameSimulate( Client cl )
	{
		base.FrameSimulate( cl );

		Rotation = Input.Rotation;
		EyeRotation = Rotation;
		Position += Velocity * Time.Delta;
	}

	[ConCmd.Admin("tok_npc_spawn")]
	public static void SpawnNPCCMD(string npcName)
	{
		NPCAsset asset = ResourceLibrary.Get<NPCAsset>( $"npcs/{npcName}.npc" );

		if( asset == null )
		{
			Log.Error( "Invalid NPC, check the NPC asset" );
			return;
		}

		BaseNPC npc = new BaseNPC();
		npc.SpawnFromAsset( asset );

		var player = ConsoleSystem.Caller.Pawn;

		if ( player == null )
			return;

		var tr = Trace.Ray( player.EyePosition, player.EyePosition + player.EyeRotation.Forward * 999 )
			.Ignore( player )
			.Run();

		npc.Position = tr.EndPosition;
	}
}

