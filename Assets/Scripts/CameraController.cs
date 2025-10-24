using UnityEngine;

// Bu script CameraPivot objesinin üzerinde olmalýdýr.
public class CameraController : MonoBehaviour
{
    // --- Mouse Kontrol Ayarlarý ---
    [Header("Mouse Look Settings")]
    public float sensitivity = 100f;

    // Dikey bakýþý (yukarý/aþaðý) kontrol etmek için
    private float rotationX = 0f;

    // Yatay bakýþý (saða/sola) kontrol etmek için
    private float rotationY = 0f;
    public float clampAngle = 80f;

    void Start()
    {
        // Farenin görünmez olmasý ve ekranýn merkezinde kilitlenmesi
        Cursor.lockState = CursorLockMode.Locked;

        // Baþlangýç dönüþ deðerlerini pivotun mevcut dönüþünden al
        rotationY = transform.eulerAngles.y;
        rotationX = transform.localEulerAngles.x;
    }

    void LateUpdate()
    {
        // --- DÖNÜÞ HESAPLAMA (Mouse Look) ---

        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        // Yatay dönüþü (saða/sola) rotationY'ye ekle
        rotationY += mouseX;

        // Dikey bakýþý (yukarý/aþaðý) rotationX'ten çýkar (invert)
        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -clampAngle, clampAngle);

        // Hem dikey hem yatay dönüþü Pivot'a uygula
        // Bu script Pivot'un üstünde olduðu için transform.rotation kullanýyoruz.
        Quaternion targetRotation = Quaternion.Euler(rotationX, rotationY, 0f);
        transform.rotation = targetRotation;
    }
}