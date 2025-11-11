public enum GameState
{
    Display,
    Fighting,
    Waiting,
    GameOver
}

public class GameManager
{
    public GameState gameState = GameState.Display;
}