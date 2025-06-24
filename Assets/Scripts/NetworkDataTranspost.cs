using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.Netcode;
using System.Collections.Generic;
public class NetworkDataTranspost : NetworkBehaviour
{

    // Chịu trách nhiệm truyền dữ liễu giữa server - host - client
    public override void OnNetworkSpawn()
    {
        if (!IsServer && IsOwner) //Only send an RPC to the server from the client that owns the NetworkObject of this NetworkBehaviour instance
        {
            ServerOnlyRpc(0, NetworkObjectId);
        }
        if (IsServer)
        {
            Debug.Log($"This is server");
        }
        else
        {
            Debug.Log($"This is client");
        }
        Debug.Log($"Debug: {NetworkObjectId} - {IsOwner}");
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ClientAndHostRpc(int value, ulong sourceNetworkObjectId, string serializedMap)
    {
        Debug.Log($"Client Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}");
        var map = DeserializeMap(serializedMap);
        Debug.Log($"{NetworkObjectId}: {map.GetLength(0)}x{map.GetLength(1)}");
        
        // Để GameManager thực hiện việc xây dựng map dựa trên bản vẽ
        GameManagerV3.Instance.ApplyMap(map);
    }

    [Rpc(SendTo.Server)]
    private void ServerOnlyRpc(int value, ulong sourceNetworkObjectId)
    {
        Debug.Log($"Server Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}");
        char[,] mapData = BuildLevel.GenerateRandomMap(13, 31);
        string serialized = SerializeMap(mapData);
        ClientAndHostRpc(value, sourceNetworkObjectId, serialized);
    }

    public static string SerializeMap(char[,] map)
    {
        int rows = map.GetLength(0);
        int cols = map.GetLength(1);
        System.Text.StringBuilder sb = new System.Text.StringBuilder(rows * cols + 5);
        sb.Append(rows).Append(',').Append(cols).Append('|'); // Lưu kích thước

        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                sb.Append(map[r, c]);

        return sb.ToString();
    }

    public static char[,] DeserializeMap(string serialized)
    {
        var parts = serialized.Split('|');
        var size = parts[0].Split(',');
        int rows = int.Parse(size[0]);
        int cols = int.Parse(size[1]);
        string data = parts[1];

        char[,] map = new char[rows, cols];
        for (int i = 0; i < data.Length; i++)
        {
            int r = i / cols;
            int c = i % cols;
            map[r, c] = data[i];
        }
        return map;
    }
}
