using UnityEngine;

public class ConveyorBeltController : MonoBehaviour
{

    [Header("Texture Movement")]
    public Renderer beltRenderer;
    float textureSpeed = 0f;

    [Header("Object Movement")]
    float beltSpeed = 0f;
    public Vector3 direction = Vector3.right;

    void Update()
    {
        check_sense_2();
        check_sense();

        // تحريك الخامة (Texture)
        if (beltRenderer != null)
        {
            if (beltSpeed > 0f)
            {
                // جرب _BaseMap أو _MainTex حسب نوع الماتريال
                beltRenderer.material.SetTextureOffset("_BaseMap", new Vector2(Time.time * textureSpeed, 0));
            }
            check_sense_2();

        }
        
    }

    void OnCollisionStay(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;
        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * beltSpeed;
        }
    }

     void check_sense_2()
    {
        if (sense_2.flag_2 == 0)
        {
            // Add your logic here for when the player enters the trigger zone
            // For example, you can stop the conveyor belt or change its speed
            textureSpeed = 0.08f; // Stop the conveyor belt
            beltSpeed = 0.8f; // Stop the object movement
        }
        else
        {
        }
    }
    void check_sense()
    {
        if (sense.flag == 1)
        {
            // Add your logic here for when the player enters the trigger zone
            // For example, you can stop the conveyor belt or change its speed
            textureSpeed = 0f; // Stop the conveyor belt
            beltSpeed = 0f; // Stop the object movement
        }
        else
        {
        }
    }
   
}
