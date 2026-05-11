using System.Collections.Generic;

public enum GameTheme
{
    Desert,
    Forest,
    City
}

public class GameState
{
    public GameTheme Theme { get; private set; } = GameTheme.Desert;

    public readonly List<PlayerModel> Players = new();
    public readonly List<BombModel> ActiveBombs = new();

    public void SetTheme(GameTheme theme)
    {
        Theme = theme;
    }
}
