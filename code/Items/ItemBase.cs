using Sandbox;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TowerOfKoba.Assets;
using TowerOfKoba.NPC;

namespace TowerOfKoba.Items;

public class ItemBase : ModelEntity
{
	ItemAsset itemAsset;

	public void SpawnFromAsset(ItemAsset asset)
	{
		itemAsset = asset;
		SetModel( itemAsset.ModelPath );
		Scale = itemAsset.ModelScale;

		Spawn();
	}

	public override void Spawn()
	{
		base.Spawn();
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
	}

	[ConCmd.Admin("tok_item_spawn")]
	public static void SpawnItemCMD( string itemName )
	{
		ItemAsset asset = ResourceLibrary.Get<ItemAsset>( $"items/{itemName}.itm" );

		if ( asset == null )
		{
			Log.Error( "Invalid Item, check the item asset" );
			return;
		}

		ItemBase item = new ItemBase();
		item.SpawnFromAsset( asset );

		var player = ConsoleSystem.Caller.Pawn;

		if ( player == null )
			return;

		var tr = Trace.Ray( player.EyePosition, player.EyePosition + player.EyeRotation.Forward * 999 )
			.Ignore( player )
			.Run();

		item.Position = tr.EndPosition;
	}
}

