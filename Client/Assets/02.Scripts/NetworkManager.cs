using System.Net.Sockets;
using System;
using UnityEngine;
using Google.FlatBuffers;
using System.Runtime.InteropServices;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance { get; private set; }
    private TcpClient client;
    private NetworkStream stream;
    private byte[] receiveBuffer = new byte[1024];

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            PacketHandler.Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public string serverIP = "127.0.0.1";  // 서버 IP
    public int serverPort = 7777;          // 서버 포트

    private void Start()
    {
        ConnectToServer();
    }

    public void ConnectToServer()
    {
        try
        {
            client = new TcpClient();
            client.Connect(serverIP, serverPort);
            stream = client.GetStream();
            stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, new AsyncCallback(OnReceiveData), null);
            Debug.Log("서버에 연결됨!");
        }
        catch (SocketException ex)
        {
            Debug.LogError($"서버 연결 실패: {ex.Message}");
        }
    }

    //public NetworkStream GetNetworkStream()
    //{
    //    return stream;
    //}

    private void OnReceiveData(IAsyncResult ar)
    {
        int bytesRead = stream.EndRead(ar);
        if (bytesRead <= 0) return;

        byte[] receivedData = new byte[bytesRead];
        Buffer.BlockCopy(receiveBuffer, 0, receivedData, 0, bytesRead);

        PacketHandler.HandlePacket(receivedData);
        stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, new AsyncCallback(OnReceiveData), null);
    }

    public void SendPacket(FlatBufferBuilder builder, short pktId)
    {
        if (stream == null)
        {
            Debug.LogError("네트워크 연결이 끊어졌습니다.");
            return;
        }

        byte[] packetData = builder.SizedByteArray();
        // 패킷 헤더 생성
        PacketHeader header;
        header.size = (short)(Marshal.SizeOf(typeof(PacketHeader)) + packetData.Length);
        header.id = pktId;

        // 패킷 직렬화
        byte[] headerBytes = Serialize(header);
        byte[] finalPacket = new byte[headerBytes.Length + packetData.Length];

        Buffer.BlockCopy(headerBytes, 0, finalPacket, 0, headerBytes.Length);
        Buffer.BlockCopy(packetData, 0, finalPacket, headerBytes.Length, packetData.Length);

        try
        {
            stream.Write(finalPacket, 0, finalPacket.Length);
        }
        catch (Exception ex)
        {
            Debug.LogError($"패킷 전송 실패: {ex.Message}");
        }
    }

    // PacketHeader를 -> 바이트화
    public static byte[] Serialize<T>(T header) where T : struct
    {
        int size = Marshal.SizeOf(header);
        byte[] arr = new byte[size];
        IntPtr ptr = Marshal.AllocHGlobal(size);

        Marshal.StructureToPtr(header, ptr, true);
        Marshal.Copy(ptr, arr, 0, size);
        Marshal.FreeHGlobal(ptr);

        return arr;
    }
}
