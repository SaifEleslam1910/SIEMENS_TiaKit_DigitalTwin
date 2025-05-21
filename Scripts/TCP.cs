using UnityEngine;
using System.Net.Sockets;using System.Text;
using System;
using System.Threading;

public class TCPClient : MonoBehaviour
{
    private TcpClient receiveClient;
    private NetworkStream receiveStream;
    private TcpClient sendClient;
    private NetworkStream sendStream;
    private string serverIP = "192.168.0.10"; // Python script host (change to IP if needed)
    private int receivePort = 5000; // Port for OPC-UA_Client_Receive.py
    private int sendPort = 5001; // Port for OPC-UA_Client_Send.py
    private bool isReceiveConnected = false;
    private bool isSendConnected = false;
    private Thread receiveThread;
    public string latestState = ""; // Store latest received JSON state
    public OPCUAState latestParsedState; // Store parsed state

    // Class to match JSON structure from OPC-UA_Client_Receive.py
    [System.Serializable]
    public class OPCUAState
    {
        public bool SENSOR1;
        public bool Barrier;
        public bool MOTOR_LEFT;
        public bool MOTOR_RIGHT;
    }

    void Start()
    {
        ConnectToReceiveServer();
        ConnectToSendServer();
    }

    void ConnectToReceiveServer()
    {
        try
        {
            receiveClient = new TcpClient();
            receiveClient.Connect(serverIP, receivePort);
            receiveStream = receiveClient.GetStream();
            isReceiveConnected = true;
            Debug.Log("Connected to receive server (port 5000)");
            // Start a thread to receive data
            receiveThread = new Thread(ReceiveData);
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.LogError("Receive connection failed: " + e.Message);
            isReceiveConnected = false;
        }
    }

    void ConnectToSendServer()
    {
        try
        {
            sendClient = new TcpClient();
            sendClient.Connect(serverIP, sendPort);
            sendStream = sendClient.GetStream();
            isSendConnected = true;
            Debug.Log("Connected to send server (port 5001)");
        }
        catch (Exception e)
        {
            Debug.LogError("Send connection failed: " + e.Message);
            isSendConnected = false;
        }
    }

    void ReceiveData()
    {
        byte[] buffer = new byte[1024];
        while (isReceiveConnected)
        {
            try
            {
                if (receiveStream.DataAvailable)
                {
                    int bytesRead = receiveStream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string data = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                        latestState = data;
                        try
                        {
                            latestParsedState = JsonUtility.FromJson<OPCUAState>(data);
                            Debug.Log($"Received state: SENSOR1={latestParsedState.SENSOR1}, " +
                                      $"Barrier={latestParsedState.Barrier}, " +
                                      $"MOTOR_LEFT={latestParsedState.MOTOR_LEFT}, " +
                                      $"MOTOR_RIGHT={latestParsedState.MOTOR_RIGHT}");
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning("JSON parse error: " + e.Message);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Receive error: " + e.Message);
                isReceiveConnected = false;
                break;
            }
            Thread.Sleep(10); // Prevent tight loop
        }
    }

    public void SendCommand(bool motorLeft, bool motorRight)
    {
        if (isSendConnected && sendStream != null)
        {
            try
            {
                string command = $"{{\"MOTOR_LEFT\": {motorLeft.ToString().ToLower()}, " +
                               $"\"MOTOR_RIGHT\": {motorRight.ToString().ToLower()}}}";
                byte[] data = Encoding.UTF8.GetBytes(command + "\n");
                sendStream.Write(data, 0, data.Length);
                Debug.Log("Sent command: " + command);
            }
            catch (Exception e)
            {
                Debug.LogError("Send error: " + e.Message);
                isSendConnected = false;
            }
        }
        else
        {
            Debug.LogWarning("Send server not connected");
        }
    }

    void Update()
    {
        // Example: Send commands on key press for testing
        if (Input.GetKeyDown(KeyCode.L))
        {
            SendCommand(true, false); // Turn MOTOR_LEFT on, MOTOR_RIGHT off
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SendCommand(false, true); // Turn MOTOR_LEFT off, MOTOR_RIGHT on
        }
    }

    void OnApplicationQuit()
    {
        if (receiveStream != null)
            receiveStream.Close();
        if (receiveClient != null)
            receiveClient.Close();
        if (sendStream != null)
            sendStream.Close();
        if (sendClient != null)
            sendClient.Close();
        isReceiveConnected = false;
        isSendConnected = false;
        if (receiveThread != null)
            receiveThread.Abort();
        Debug.Log("Disconnected from servers");
    }
}