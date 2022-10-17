using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerOfKoba.Items;

[GameResource("ToK Item", "itm", "Creates a new ToK item")]
public class ItemAsset : GameResource
{
	[Category( "Meta Info" )]
	public new string Name { get; set; } = "Standard Item Name";

	[Category( "Meta Info" )]
	public string Description { get; set; } = "Item Description";

	[Category( "Meta Info" ), ResourceType( "png" )]
	public string Icon { get; set; }

	[Category("Model"), ResourceType("vmdl")]
	public string ModelPath { get; set; }

	[Category( "Model" ), MinMax( 0.1f, 2 )]
	public float ModelScale { get; set; } = 1.0f;

	[Category( "Model" ), ResourceType( "vmat" )]
	public string MaterialOverride { get; set; }

	[Category( "Miscellenous" ), MinMax( 1, 1000 )]
	public int SellingPrice { get; set; } = 1;
}

