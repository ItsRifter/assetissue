using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using TowerOfKoba.Items;

namespace TowerOfKoba.Assets;

[GameResource( "ToK NPC", "npc", "Creates a new ToK NPC" )]
public class NPCAsset : GameResource
{
	//Generic NPC information
	[Category( "Meta Info" )]
	public new string Name { get; set; } = "Default NPC";

	//Maybe we can have a wiki on NPCs?
	[Category( "Meta Info" )]
	public string Description { get; set; } = "Basic Default NPC Description";

	[Category( "Meta Info" ), ResourceType( "png" )]
	public string Icon { get; set; }

	public enum NPCTypeEnum
	{
		None,
		QuestGiver,
		ShopKeeper,
	}

	/// <summary>
	/// ONLY APPLIES TO NON-HOSTILE NPCS, what type of npc is this to the player
	/// </summary>
	[Category( "Meta Info" )]
	public NPCTypeEnum Type { get; set; } = NPCTypeEnum.None;

	public enum RelationEnum
	{
		Neutral,
		Hostile,
		Friendly
	}

	/// <summary>
	/// Relationship to the player
	/// </summary>
	[Category( "Meta Info" )]
	public RelationEnum Relation { get; set; } = RelationEnum.Neutral;

	//Statistics
	[Category( "Statisics" )]
	public int BaseHealth { get; set; } = 1;

	[Category( "Statisics" )]
	public int BaseLevel { get; set; } = 1;

	[Category( "Statisics" ), MinMax( 1, 999 )]
	public float BaseSpeed { get; set; } = 1.0f;

	/// <summary>
	/// The min scaling, affects stats like health, damage etc.
	/// </summary>
	[Category( "Statisics" ), MinMax( 1, 5 )]
	public float LevelScaleMin { get; set; } = 1.0f;

	/// <summary>
	/// The max scaling, affects stats like health, damage etc.
	/// </summary>
	[Category( "Statisics" ), MinMax( 1, 5 )]
	public float LevelScaleMax { get; set; } = 2.0f;

	//Model
	/// <summary>
	/// The actual model
	/// </summary>
	[Category( "Model" ), ResourceType( "vmdl" )]
	public string ModelPath { get; set; }

	[Category( "Model" ), MinMax( 0.1f, 3 )]
	public float ModelScale { get; set; } = 1.0f;

	/// <summary>
	/// Overrides the material, leave empty to use default
	/// </summary>
	[Category( "Model" ), ResourceType( "vmat" )]
	public string MaterialOverride { get; set; }

	/// <summary>
	/// How much damage do they deal
	/// </summary>
	[Category( "Attacking" ), ShowIf( "Relation", RelationEnum.Hostile )]
	public int Damage { get; set; } = 1;

	[Category( "Attacking" ), ResourceType( "sound" ), ShowIf( "Relation", RelationEnum.Hostile )]
	public string HurtSound { get; set; }

	[Category( "Attacking" ), ResourceType( "sound" ), ShowIf( "Relation", RelationEnum.Hostile )]
	public string AlertSound { get; set; }

	[Category( "Attacking" ), ResourceType( "sound" ), ShowIf( "Relation", RelationEnum.Hostile )]
	public string DeathSound { get; set; }

	/// <summary>
	/// How fast can they attack
	/// </summary>
	[Category( "Attacking" ), ShowIf( "Relation", RelationEnum.Hostile )]
	public int AttackSpeed { get; set; } = 1;

	//We could have more methods of attacking for NPCs
	public enum AttackMethodEnum
	{
		Normal,
		Fire,
		Ice,
		Stone,
		Nature
	}

	/// <summary>
	/// How does the NPC attack the player
	/// </summary>
	[Category( "Attacking" ), ShowIf( "Relation", RelationEnum.Hostile )]
	public AttackMethodEnum AttackMethod { get; set; } = AttackMethodEnum.Normal;

	//TODO, get an actual NPC class

	//[Category("Miscellanous"), ShowIf( "Relation", RelationEnum.Hostile )]
	//public NPCAsset[] Allies { get; set; }

	[Category( "Player Rewards" ), Title("XP Reward"), ShowIf( "Relation", RelationEnum.Hostile )]
	public int XPReward { get; set; } = 1;

	[Category( "Player Rewards" ), ShowIf( "Relation", RelationEnum.Hostile )]
	public ItemAsset[] DroppableItems { get; set; }

	//Quest givers
	//TODO, Code quest assets
	//[Category( "Quest Giver" ), ShowIf( "Type", NPCTypeEnum.QuestGiver ), HideIf("Relation", RelationEnum.Hostile)]
	//public QuestAsset Quests { get; set; }

	//Shop keepers
	[Category("Shop"), ShowIf( "Relation", RelationEnum.Friendly )]
	public ItemAsset[] StockItems { get; set; }

	public static IReadOnlyList<NPCAsset> tok_npcs => _allNPCs;
	internal static List<NPCAsset> _allNPCs = new();

	public IReadOnlyList<NPCAsset> ReadAllNPCs()
	{
		return _allNPCs;
	}

	protected override void PostLoad()
	{
		base.PostLoad();

		if ( !_allNPCs.Contains( this ) )
			_allNPCs.Add( this );
	}
}

