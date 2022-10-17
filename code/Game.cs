using Sandbox;
using TowerOfKoba.UI;

namespace TowerOfKoba;

public partial class TkGame : Game
{	
	public TkGame()
	{
	}

	public override void ClientJoined( Client cl )
	{
		base.ClientJoined(cl);
		
		var ply = new TKPlayer();
		ply.Spawn();

		cl.Pawn = ply;
		
	}

	public override void ClientDisconnect( Client cl, NetworkDisconnectionReason reason )
	{
		base.ClientDisconnect(cl, reason);
	}
}
