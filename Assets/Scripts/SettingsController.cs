using UnityEngine;
using System.Collections;

public class SettingsController : MonoBehaviour 
{

    public Texture settingsTexture;
    public Texture backTexture;
    private Texture currentTexture;

    private GameController gameController;

	// Use this for initialization
	void Start () 
    {
        gameController = FindObjectOfType(typeof(GameController)) as GameController;
        currentTexture = settingsTexture;
	}   

    void OnGUI()
    {
        if (gameController.GetCurrentState() == GameStates.SignIn)
        {
            if (GUI.Button(new Rect(Screen.width / 50, Screen.height / /*1.12f*/50, Screen.width / 12, Screen.height / 10), settingsTexture, GUIStyle.none))
            {
                if (!Application.isEditor && Application.platform == RuntimePlatform.WindowsPlayer)
                {                    
                    //VirtualKeyboard.HideTouchKeyboard();
                }

                gameController.SetGameState(GameStates.Settings);
            }
        }

        if (gameController.GetCurrentState() == GameStates.Settings)
        {
            if (GUI.Button(new Rect(Screen.width / 50, Screen.height / /*1.12f*/50, Screen.width / 12, Screen.height / 10), backTexture, GUIStyle.none))
            {
                if (!Application.isEditor && Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    //VirtualKeyboard.ShowTouchKeyboard();
                }

                gameController.SetGameState(GameStates.SignIn);                
            }
        } 
    }
}
