using UnityEngine;

public class AttachWeapon : MonoBehaviour
{
    public GameObject weaponPrefab;          // Inspector'dan sürükle
    public Vector3 localPositionOffset;      // El pozisyonuna göre ince ayar
    public Vector3 localRotationOffset;      // Döndürme offset (Euler)
    public Vector3 localScale = Vector3.one; // Gerekirse ölçek ayarý

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("Animator yok! AttachWeapon script'i Player1 üzerinde olmalý.");
            return;
        }

        Transform rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);

        if (rightHand == null)
        {
            Debug.LogWarning("RightHand bulunamadý. Model Humanoid deðil veya bone map'lenmemiþ.");
            return;
        }

        if (weaponPrefab == null)
        {
            Debug.LogWarning("weaponPrefab atanmadý!");
            return;
        }

        GameObject weaponInstance = Instantiate(weaponPrefab, rightHand);
        weaponInstance.transform.localPosition = localPositionOffset;
        weaponInstance.transform.localRotation = Quaternion.Euler(localRotationOffset);
        weaponInstance.transform.localScale = localScale;
    }
}
