using System.Net.Sockets;
using UnityEngine;
using UnityEngine.InputSystem;

public class sense_2 : MonoBehaviour
{
    public TCPClient.OPCUAState getvalue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static int flag_2 =0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("correct"))
        {
            flag_2 = 1;
            // Add your logic here for when the player enters the trigger zone
        }
    }

}
