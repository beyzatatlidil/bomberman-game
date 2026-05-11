using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Theme/ThemeData")]
public class ThemeData : ScriptableObject
{
    public TileBase ground;
    public TileBase wallStatic;
    public TileBase wallBreakable;
    public TileBase wallHard;
}
