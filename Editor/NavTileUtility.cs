using Bloops.GridFramework.Navigation;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor.Bloops.TilemapNavigationSystem
{
	public class NavTileUtility 
	{
		[MenuItem("Assets/Create/NavTile", priority = 357)]
		internal static void CreateNewTile()
		{
			string message = string.Format("Save nav tile'{0}':", "tile");
			string newAssetPath = EditorUtility.SaveFilePanelInProject("Save tile", "NavTile", "asset", message, "Assets/");

			// If user canceled or save path is invalid, we can't create the tile
			if (string.IsNullOrEmpty(newAssetPath))
				return;

			AssetDatabase.CreateAsset(CreateDefaultNavTile(), newAssetPath);
		}

		/// <summary>Creates a NavTile with defaults based on the NavTile preset</summary>
		/// <returns>A NavTile with defaults based on the NavTile preset</returns>
		public static NavTile CreateDefaultNavTile()
		{
			return ObjectFactory.CreateInstance<NavTile>();
		}

		/// <summary>Creates a Tile with defaults based on the Tile preset and a Sprite set</summary>
		/// <param name="sprite">A Sprite to set the Tile with</param>
		/// <returns>A NavTile with defaults based on the Tile preset and a Sprite set</returns>
		[CreateTileFromPalette]
		public static TileBase DefaultTile(Sprite sprite)
		{
			NavTile tile = CreateDefaultNavTile();
			tile.name = sprite.name;
			tile.sprite = sprite;
			tile.color = Color.white;
			return tile;
		}
	}
}