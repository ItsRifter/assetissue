using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerOfKoba;

public class ShoulderCamera : CameraMode
{
	private Angles orbitAngles;

	//How far/close can the camera zoom in Y
	float minYZoom = -90;
	float maxYZoom = 25;

	//Similar to above but for the X
	float minXZoom = -3;
	float maxXZoom = 3;

	//Default zoom
	float x_zoom = 2.5f;
	float y_zoom = -55.0f;

	public override void Update()
	{
		var pawn = Local.Pawn as AnimatedEntity;

		if ( pawn == null )
			return;

		Position = pawn.Position;
		Vector3 targetPos;

		var center = pawn.Position + Vector3.Up * 64;

		Position = center;
		Rotation = Rotation.FromAxis( Vector3.Up, 4 ) * Input.Rotation;

		float distance = 130.0f + y_zoom * pawn.Scale;

		targetPos = Position + x_zoom * Input.Rotation.Right * (pawn.CollisionBounds.Maxs.x * pawn.Scale);
		targetPos += Input.Rotation.Forward * -distance;
		
		//Collision check
		var tr = Trace.Ray( Position, targetPos )
			.Ignore( pawn )
			.Radius( 8 )
			.Run();

		Position = tr.EndPosition;
		
		FieldOfView = 70;

		Viewer = null;
	}

	public override void BuildInput( InputBuilder input )
	{
		//If we're holding down walk, allow X adjustments
		if ( input.Down( InputButton.Walk ) )
		{
			x_zoom += input.MouseWheel * 0.5f;
			x_zoom = x_zoom.Clamp( minXZoom, maxXZoom );
		}
		//Otherwise allow Y adjustments
		else
		{
			y_zoom -= input.MouseWheel * 5;
			y_zoom = y_zoom.Clamp( minYZoom, maxYZoom );
		}

		//Do the adjustments on the camera
		orbitAngles.yaw += input.AnalogLook.yaw;
		orbitAngles.pitch += input.AnalogLook.pitch;
		orbitAngles = orbitAngles.Normal;
		orbitAngles.pitch = orbitAngles.pitch.Clamp( -89, 89 );

		base.BuildInput( input );
	}
}
