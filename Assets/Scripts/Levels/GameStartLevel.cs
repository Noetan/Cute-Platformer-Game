// Script is run on game launch and after the GameController has been init
// Loads the game and opens up the first scene

using UnityEngine;
using System.Collections;

public class GameStartLevel : MonoBehaviour
{
    [Tooltip("The scene to open once the game has loaded\nMake sure the scene has been added to Unity Build Settings")]
    [SerializeField]
    string DefaultLevel;

    // Using Start instead of Awake to make sure the GameController has been created
    void Start()
    {
        // Load the game here

        // Open the first scene (main menu once we're further in or splash screen)
        Application.LoadLevel(DefaultLevel);
        GameController.Instance.ChangeState(GameStates.States.PLAYING);
    }
}
