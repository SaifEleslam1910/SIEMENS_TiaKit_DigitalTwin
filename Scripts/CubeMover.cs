using UnityEngine;

public class CubeMover : MonoBehaviour
{
   public float moveSpeed = 5f; // سرعة حركة المكعب

    void Update()
    {
        // الحصول على قيمة المدخل الأفقي (أسهم يمين/يسار أو A/D)
        float horizontalInput = Input.GetAxis("Horizontal");
        // الحصول على قيمة المدخل الرأسي (أسهم فوق/تحت أو W/S)
        float verticalInput = Input.GetAxis("Vertical");

        // إنشاء متجه الحركة بناءً على المدخلات
        // Vector3.right لليمين، Vector3.forward للأمام (في اتجاه Z الموجب)
        // نستخدم فقط محور X (يمين/يسار) ومحور Z (أمام/خلف) للحركة على مستوى أفقي
        Vector3 movement = new Vector3(horizontalInput,  -verticalInput, 0f);

        // تطبيق الحركة على المكعب مع مراعاة السرعة والزمن
        transform.Translate(movement * moveSpeed * Time.deltaTime);
    }
}
