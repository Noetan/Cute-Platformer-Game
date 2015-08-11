public static class GameStates
{
    // Curent game state
    public static States Current = States.LAUNCH;

    // These are just generic, we may or may not need to add/remove states
    // Game states
    public enum States
    {
        LAUNCH,
        MENU,
        GAME_START,
        PLAYING,
        GAME_OVER,
        PAUSED
    }
}