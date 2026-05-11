using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class ThemeManager : MonoBehaviour
{
    public static ThemeManager Instance;

    [Header("Themes")]
    public List<ThemeData> themes;
    public ThemeData currentTheme;

    [Header("Tilemaps")]
    public Tilemap groundMap;
    public Tilemap wallStaticMap;
    public Tilemap wallBreakableMap;
    public Tilemap wallHardMap;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    void Update()
{
    if (Input.GetKeyDown(KeyCode.Alpha1))
        ApplyTheme(themes[0]); // Desert

    if (Input.GetKeyDown(KeyCode.Alpha2))
        ApplyTheme(themes[1]); // Forest

    if (Input.GetKeyDown(KeyCode.Alpha3))
        ApplyTheme(themes[2]); // City
}


    void Start()
    {
        if (currentTheme != null)
            ApplyTheme(currentTheme);
    }

    public void ApplyTheme(ThemeData theme)
{
    if (theme == null) return;

    currentTheme = theme;

   ReplaceTiles(groundMap, theme.ground);
ReplaceTiles(wallStaticMap, theme.wallStatic);
ReplaceTiles(wallBreakableMap, theme.wallBreakable);
ReplaceTiles(wallHardMap, theme.wallHard);

}


   void ReplaceTiles(Tilemap map, TileBase newTile)
{
    if (map == null)
    {
        Debug.LogWarning("Tilemap null");
        return;
    }

    if (newTile == null)
    {
        Debug.LogWarning("Theme tile is NULL, theme not applied");
        return;
    }

    foreach (var pos in map.cellBounds.allPositionsWithin)
    {
        if (map.HasTile(pos))
            map.SetTile(pos, newTile);
    }
}


}
