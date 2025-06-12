using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkButton : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnButtonStartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void OnButtonStartClient()
    {
        NetworkManager.Singleton.StartClient();
    }
    
    public void OnButtonStartServer()
    {
        NetworkManager.Singleton.StartServer();
    }
}
