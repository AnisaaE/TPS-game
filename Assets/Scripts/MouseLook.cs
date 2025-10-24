using UnityEngine;

[RequireComponent(typeof(Transform))]
public class TPSMouseLook : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Kameranýn transform'u (TPS kamera).")]
    public Transform cameraTransform;

    [Header("Sensitivity & Smoothing")]
    public float mouseSensitivityX = 150f;
    public float mouseSensitivityY = 120f;
    public float smoothTime = 0.05f;

    [Header("Vertical clamp (deg)")]
    public float minPitch = -40f;
    public float maxPitch = 60f;

    [Header("Movement / Activation")]
    [Tooltip("Mouse look yalnýzca yürürken aktif olsun mu? (Horizontal/Vertical input)")]
    public bool onlyWhileMoving = true;
    [Tooltip("Minimum hareket input büyüklüðü (0-1).")]
    public float movementThreshold = 0.1f;
    [Tooltip("Sað fare tuþu basýlýyken de mouse look aktif olsun.")]
    public bool enableOnAim = true;
    [Tooltip("Hareket inputlarýný okuyacak eksen isimleri (Input Manager'a göre).")]
    public string horizontalAxis = "Horizontal";
    public string verticalAxis = "Vertical";

    // Internal
    float yaw;   // oyuncunun Y ekseni dönüþü (character/transform)
    float pitch; // kameranýn X ekseni bakýþý (eklenti)
    Vector2 currentVelocity; // smooth
    Vector2 currentAngles;

    void Start()
    {
        if (cameraTransform == null)
        {
            Debug.LogError("TPSMouseLook: Camera Transform referansý atanmadý.");
            enabled = false;
            return;
        }

        // Baþlangýç açýlarýný set et
        Vector3 camEuler = cameraTransform.localEulerAngles;
        Vector3 selfEuler = transform.eulerAngles;
        yaw = selfEuler.y;
        // localEulerAngles returns 0-360; normalize pitch to -180..180
        pitch = camEuler.x;
        if (pitch > 180f) pitch -= 360f;

        currentAngles = new Vector2(yaw, pitch);
    }

    void Update()
    {
        // 1) Karakterin "yürüdüðünü" tespit et
        bool isMoving = false;
        if (!onlyWhileMoving)
            isMoving = true;
        else
        {
            float h = Input.GetAxis(horizontalAxis);
            float v = Input.GetAxis(verticalAxis);
            isMoving = (Mathf.Abs(h) + Mathf.Abs(v)) >= movementThreshold;
        }

        // 2) Aim tuþu ile override
        if (enableOnAim && (Input.GetMouseButton(1))) // sað fare tuþu
            isMoving = true;

        // 3) Eðer aktifse mouse input kullan, deðilse yumuþak geri dönüþ yapma (veya sýfýr)
        float mouseX = 0f;
        float mouseY = 0f;
        if (isMoving)
        {
            mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX * Time.deltaTime;
            mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;
        }
        else
        {
            // isMoving false iken mouse input yok sayýlýr (mouse hareketi etkisiz)
            mouseX = 0f;
            mouseY = 0f;
        }

        // Hedef açýlarý hesapla
        yaw += mouseX;
        pitch -= mouseY; // mouse Y tersi olduðu için çýkartýyoruz
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Vector2 targetAngles = new Vector2(yaw, pitch);

        // Smooth damp ile yumuþat
        currentAngles.x = Mathf.SmoothDamp(currentAngles.x, targetAngles.x, ref currentVelocity.x, smoothTime);
        currentAngles.y = Mathf.SmoothDamp(currentAngles.y, targetAngles.y, ref currentVelocity.y, smoothTime);

        // Uygula: karakterin yatay dönüþü (yaw) — karakter objesini döndür
        transform.rotation = Quaternion.Euler(0f, currentAngles.x, 0f);

        // Kamera için pitch uygulama (local rotation)
        cameraTransform.localRotation = Quaternion.Euler(currentAngles.y, 0f, 0f);
    }
}

