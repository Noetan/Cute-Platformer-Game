// The top level controller that manages everything else
// Put game wide code here

using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    // Only one instance of the game controller at a time
    // Use this to reference the game controller without passing references around
    public static GameController Instance;

    void Awake()
    {
        Instance = this;
        // Prevent the game controller from being destroyed when loading new levels
        DontDestroyOnLoad(this);
    }

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	    switch (GameStates.Current)
        {
            default:
                break;
        }
	}

    // When game is paused, this is called
    void OnApplicationPause(bool paused)
    {
    }

    // Note these are just placeholders copied and pasted from my other game
    // Just leaving these here as templates for when we start to use states
    // To change state, call ChangeState(new state);
    // Add your state to ChangeState if needed
    // Give it a function to call
    #region State changing

    // Handles leaving the current state and changing into the new one
    public void ChangeState(GameStates.States newState)
    {
        // Don't do anything if the newState is a duplicate
        if (GameStates.Current == newState)
        {
            return;
        }

        // Clean up current state if needed
        switch (GameStates.Current)
        {
            case GameStates.States.MENU:
                ChangeMenuState(false);
                break;

            case GameStates.States.GAME_OVER:
                ChangeGameOverState(false);
                break;

            case GameStates.States.PAUSED:
                ChangePausedState(false);
                break;
        }

        // Enter new state
        switch (newState)
        {
            case GameStates.States.MENU:
                ChangeMenuState(true);
                break;

            case GameStates.States.PLAYING:
                ChangePlayingState(true);
                break;

            case GameStates.States.GAME_OVER:
                ChangeGameOverState(true);
                break;

            case GameStates.States.PAUSED:
                ChangePausedState(true);
                break;

            case GameStates.States.GAME_START:
                ChangeGameStartState(true);
                break;
        }
    }

    // States
    void ChangeMenuState(bool entering)
    {
        GameStates.Current = GameStates.States.MENU;

        if (entering)
        {
            // do something
        }
        else
        {
            // clean up something
        }
    }

    public void ChangeGameStartState(bool entering)
    {
        GameStates.Current = GameStates.States.GAME_START;
    }

    public void ChangePlayingState(bool entering)
    {
        GameStates.Current = GameStates.States.PLAYING;
    }

    public void ChangeGameOverState(bool entering)
    {
        GameStates.Current = GameStates.States.PLAYING;
    }

    public void ChangePausedState(bool entering)
    {
        GameStates.Current = GameStates.States.PAUSED;

        if (entering)
        {
            Debug.Log("pausing");
            Time.timeScale = 0.0f;
        }
        else
        {
            Debug.Log("unpausing");
            Time.timeScale = 1.0f;
        }
    }
    #endregion

    IEnumerator Delay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}
