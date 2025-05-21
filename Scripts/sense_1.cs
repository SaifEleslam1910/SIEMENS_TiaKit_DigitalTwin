using UnityEngine;

public class sense : MonoBehaviour
{
    public static int flag = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("correct"))
        {
            flag = 1;
            // Add your logic here for when the player enters the trigger zone
        }
    }
}
