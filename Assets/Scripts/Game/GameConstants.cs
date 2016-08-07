// File to hold constants used throughout the game
// Useful for avoiding the use of strings so less typo related bugs
// Plus the benefit of IDE auto-complete

// List of messages used with MessageKit
// Add your own here. MessageKit uses ints, not strings to avoid typos.
public static class MessageTypes
{
    public const int PLAYER_JUMP = 0;
    public const int PLAYER_LAND = 1;
    public const int DOUBLETAP_CROUCH = 2;
}

public static class Layers
{
    public const int DEFAULT = 0;
    public const int PLAYER = 8;
    public const int LEDGE_GRAB_SURFACE = 9;
}

public static class Tags
{
    public const string Untagged = "Untagged";
    public const string Player = "Player";
    public const string Pickup = "Pickup";
    public const string Pushable = "Pushable";
    public const string Water = "Water";
    public const string Enemy = "Enemy";
    public const string Coin = "Coin";
    public const string MovingPlatform = "MovingPlatform";
    public const string Checkpoint = "Checkpoint";
    public const string Waypoint = "Waypoint";
    public const string OneWayPlatform = "OneWayPlatform";
    public const string ClimbableRope = "ClimbableRope";
    public const string Ladder = "Ladder";
    public const string ClimbableWall = "ClimbableWall";
    public const string BalancableSurface = "BalancableSurface";
    public const string BalancableRope = "BalancableRope";
    public const string FxTemporaire = "FxTemporaire";
    public const string Spring = "Spring";
    public const string Talkable = "Talkable";
    public const string Triggerable = "Triggerable";
    public const string AttractedItem = "AttractedItem";
    public const string MainCamera = "MainCamera";
}

public static class Buttons
{
    public const string Crouch = "Crouch";
    public const string Jump = "Jump";
    public const string Melee = "Melee";
    public const string Horizontal = "Horizontal";
    public const string Vertical = "Vertical";
}


