using UnityEngine;

public class InputController : MonoBehaviour
{
    // public PlayerController player;
    public bool useControllerMoblieOnly = false; // If you want to force joystick only (mobile-only)
    
    protected virtual void Start()
    {
        // if (GameManager.Instance != null)
        // {
        //     UpdateUIState();
        // }
        // else
        // {
        //     Debug.Log("GameManager is not initialized. Please bind the player before using input controls.");
        // }
    }

    public virtual void BindPlayer(GameObject player)
    {
        // this.player = player.GetComponent<PlayerController>();
        // if (this.player == null)
        // {
        //     Debug.LogError("PlayerController component not found on the player GameObject.");
        // }
    }

    public virtual void UnbindPlayer()
    {
        // player = null;
    }

    public virtual void UpdateUIState()
    {
        // This method can be overridden in derived classes to handle input updates
        // For example, MoveInputController or PlaceBombInputController can implement their own logic here     
    }
}

