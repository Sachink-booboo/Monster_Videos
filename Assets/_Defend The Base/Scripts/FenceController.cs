using System.Collections;
using UnityEngine;
using DG.Tweening;

public class FenceController : MonoBehaviour
{
    MeshRenderer meshRenderer;
    public Material normalMaterial, damagedMaterial;

    bool isEnemyInside;

    [Header("Bounce Settings")]
    public float tiltAngle = 5f;
    public float tiltTime = 0.1f;

    Quaternion originalLocalRotation;
    Sequence bounceSequence;
    Tween bounceTween;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = normalMaterial;
        // ✅ Store LOCAL rotation
        originalLocalRotation = transform.localRotation;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out Enemy enemy))
        {
            if (!isEnemyInside)
            {
                StartCoroutine(DamageLoop());
            }
        }
    }

    IEnumerator DamageLoop()
    {
        isEnemyInside = true;
        // Damaged effect
        meshRenderer.material = damagedMaterial;

        PlayBounce();

        yield return new WaitForSeconds(0.2f);

        // Back to normal
        meshRenderer.material = normalMaterial;
        yield return new WaitForSeconds(0.2f);

        isEnemyInside = false;
    }

    void PlayBounce()
    {
        bounceSequence?.Kill();

        bounceSequence = DOTween.Sequence();

        bounceSequence
            .Append(transform.DOLocalRotate(
                originalLocalRotation.eulerAngles + new Vector3(tiltAngle, 0, 0),
                tiltTime
            ).SetEase(Ease.OutQuad))

            .Append(transform.DOLocalRotate(
                originalLocalRotation.eulerAngles + new Vector3(-tiltAngle, 0, 0),
                tiltTime
            ).SetEase(Ease.InOutQuad))

            .Append(transform.DOLocalRotate(
                originalLocalRotation.eulerAngles,
                tiltTime
            ).SetEase(Ease.OutQuad));
    }

    void OnDisable()
    {
        bounceSequence?.Kill();
        transform.localRotation = originalLocalRotation;
    }
}
