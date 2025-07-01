using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;

public class RoomManager : NetworkBehaviour
{
    [SerializeField] protected List<Room> rooms = new List<Room>();
    private static Dictionary<ulong, Room> playerRoomMaps = new Dictionary<ulong, Room>();
    public string roomNameInput = "Room_1234";

    public float time = 0.0f;
    public float timeUpdate = 5.0f;

    public int maxPlayersInput = 2;

    [SerializeField] protected bool autoUpdateRooms = true;

    [System.Serializable]
    public class Room
    {
        public string RoomID;
        public List<ulong> Players;
        public int maxPlayers;
        public ulong Owner;

        public Room(ulong owner, string id, int _maxPlayer)
        {
            RoomID = id;
            Owner = owner;
            maxPlayers = _maxPlayer;
            Players = new List<ulong>();
        }
    }

    [System.Serializable]
    private class RoomListWrapper
    {
        public List<Room> Rooms;

        public RoomListWrapper(List<Room> rooms)
        {
            Rooms = rooms;
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Debug.Log("Đây là Server");
            RequestRoomDataServerRpc();
            return;
        }
        if (!IsServer)
        {
            Debug.Log("Đây là Client");
            return;
        }
    }

    void Update()
    {
        if (IsServer) return;
        time += Time.deltaTime;
        if (time >= timeUpdate)
        {
            RequestRoomDataServerRpc();
            time = 0;
        }
    }

    [ContextMenu("Create Room")]
    public void CreateRoom()
    {
        Debug.Log("Create Room Click");
        if (playerRoomMaps.ContainsKey(NetworkManager.Singleton.LocalClientId))
        {
            Debug.LogWarning($"[{NetworkManager.Singleton.LocalClientId}] Already in a room.");
            return;
        }
        if (IsServer)
        {
            CreateRoomOnServer(NetworkManager.Singleton.LocalClientId, roomNameInput, maxPlayersInput);
        }
        else
        {
            CreateRoomServerRpc(NetworkManager.Singleton.LocalClientId, roomNameInput, maxPlayersInput);
        }
        Debug.Log($"playerRoomMaps: {playerRoomMaps.Count()}");
    }

    public void CreateRoomOnServer(ulong clientId, string roomName, int maxPlayers)
    {
        if (rooms.Exists(r => r.RoomID == roomName))
        {
            Debug.LogWarning($"[{clientId}] Room '{roomName}' already exists.");
            return;

        }

        Room newRoom = new Room(clientId, roomName, maxPlayers);
        newRoom.Players.Add(clientId);
        rooms.Add(newRoom);
        playerRoomMaps[clientId] = newRoom;

        if (autoUpdateRooms) UpdateClientsRoomList();

        Debug.Log($"[{clientId}] Created room: {roomName}");
    }

    [ServerRpc(RequireOwnership = false)]
    private void CreateRoomServerRpc(ulong clientId, string roomName, int maxPlayers)
    {
        CreateRoomOnServer(clientId, roomName, maxPlayers);
    }

    [ContextMenu("Join Room")]
    public void JoinRoom()
    {
        if (playerRoomMaps.ContainsKey(NetworkManager.Singleton.LocalClientId))
        {
            Debug.LogWarning($"[{NetworkManager.Singleton.LocalClientId}] Already in a room.");
            return;
        }
        if (IsServer)
        {
            JoinSpecificRoom(NetworkManager.Singleton.LocalClientId, roomNameInput);
        }
        else
        {
            JoinRoomServerRpc(NetworkManager.Singleton.LocalClientId, roomNameInput);
        }

        Debug.Log($"playerRoomMaps: {playerRoomMaps.Count()}");

        foreach (var item in playerRoomMaps)
        {
            Debug.Log($"item: {item.Key}, Value: {playerRoomMaps[item.Key]}");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void JoinRoomServerRpc(ulong clientId, string roomName)
    {
        JoinSpecificRoom(clientId, roomName);
    }

    public void JoinSpecificRoom(ulong clientId, string roomName)
    {
        Debug.Log("Tôi thực hiện việc này");
        Room room = rooms.Find(r => r.RoomID == roomName);
        if (room == null)
        {
            Debug.LogWarning($"[{clientId}] Room '{roomName}' not found.");
            return;
        }

        if (room.Players.Contains(clientId))
        {
            Debug.Log($"UID: {clientId} already in Room {roomName}");
            return;
        }

        if (room.Players.Count >= room.maxPlayers)
        {
            Debug.LogWarning($"[{clientId}] Room '{roomName}' is full.");
            return;
        }
        
        room.Players.Add(clientId);
        playerRoomMaps[clientId] = room;
        if (autoUpdateRooms) UpdateClientsRoomList();
        Debug.Log($"[{clientId}] Joined room: {roomName}");
    }

    [ContextMenu("Leave Room")]
    public void LeaveRoom()
    {
        if (IsServer)
        {
            RemovePlayerFromRoom(NetworkManager.Singleton.LocalClientId);
        }
        else
        {
            LeaveRoomServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void LeaveRoomServerRpc(ulong clientId)
    {
        RemovePlayerFromRoom(clientId);
    }

    private void RemovePlayerFromRoom(ulong clientId)
    {
        // Tìm phòng mà client đang ở (từ playerRoomMaps)
        if (!playerRoomMaps.TryGetValue(clientId, out Room roomRef))
        {
            Debug.LogWarning($"[{clientId}] Not in any room.");
            return;
        }

        // Luôn đảm bảo thao tác trên object gốc trong danh sách `rooms`
        Room room = rooms.Find(r => r.RoomID == roomRef.RoomID);

        if (room == null)
        {
            Debug.LogError($"Room reference mismatch! Room {roomRef.RoomID} không tìm thấy trong danh sách rooms.");
            return;
        }

        // Nếu client là chủ phòng, xóa cả phòng
        if (room.Owner == clientId)
        {
            Debug.Log($"[{clientId}] was the owner. Removing entire room: {room.RoomID}");
            rooms.Remove(room);

            // Xóa tất cả player liên quan khỏi playerRoomMaps
            foreach (var uid in room.Players)
            {
                playerRoomMaps.Remove(uid);
            }

            if (autoUpdateRooms) UpdateClientsRoomList();
            return;
        }

        // Nếu không phải chủ phòng → chỉ xóa player khỏi danh sách
        if (room.Players.Contains(clientId))
        {
            room.Players.Remove(clientId);
            playerRoomMaps.Remove(clientId);
            Debug.Log($"[{clientId}] Left room: {room.RoomID}");
        }
        else
        {
            Debug.LogWarning($"[{clientId}] was not found in room {room.RoomID}'s player list.");
        }

        // Nếu phòng trống sau khi xóa thì loại bỏ phòng
        if (room.Players.Count == 0)
        {
            Debug.Log($"Room {room.RoomID} is now empty and will be removed.");
            rooms.Remove(room);
        }

        if (autoUpdateRooms) UpdateClientsRoomList();
    }


    [ContextMenu("Show Room List")]
    public void ShowRoomList()
    {
        Debug.Log("Current Rooms:");
        foreach (var room in rooms)
        {
            Debug.Log($"Room {room.RoomID} - Players: {string.Join(", ", room.Players)}");
        }
    }
    // --------------------------------------------------------- //

    private void UpdateClientsRoomList()
    {
        string json = JsonUtility.ToJson(new RoomListWrapper(rooms));
        SendRoomDataClientRpc(json);
        // Debug.Log("UpdateClientsRoomList");
        // Debug.Log($"json: {json}");
    }

    [ClientRpc]
    private void SendRoomDataClientRpc(string json)
    {
        // Debug.Log($"Client nhận JSON: {json}");
        RoomListWrapper wrapper = JsonUtility.FromJson<RoomListWrapper>(json);
        rooms = wrapper.Rooms;
        // Debug.Log("Updated room list from server.");

        // Kiểm tra kết quả
        // foreach (var room in rooms)
        // {
        //     Debug.Log($"Client thấy Room {room.RoomID} có players: {string.Join(", ", room.Players)}");
        // }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RequestRoomDataServerRpc(ServerRpcParams rpcParams = default)
    {
        UpdateClientsRoomList();
    }
}
