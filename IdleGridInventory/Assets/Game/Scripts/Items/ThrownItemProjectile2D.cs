using System;
using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class ThrownItemProjectile2D : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Sequence sequence;
    private float spinDegPerSec;

    private void Reset()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnDisable()
    {
        KillTweens();
    }

    public void Launch(
        Sprite sprite,
        Vector3 startWorld,
        Vector3 targetWorld,
        Vector3 projectileScale,
        float speedUnitsPerSecond,
        float arcHeight,
        float launchAngleDegrees,
        float spinDegreesPerSecond,
        Action onArrived)
    {
        if (spriteRenderer == null)
        {
            Debug.LogError("[ThrownItemProjectile2D] Missing SpriteRenderer reference.", this);
            Destroy(gameObject);
            return;
        }

        spriteRenderer.sprite = sprite;
        transform.position = startWorld;
        transform.localScale = projectileScale;

        Vector3 toTarget = targetWorld - startWorld;
        float directDistance = toTarget.magnitude;

        if (directDistance <= 0.001f)
        {
            onArrived?.Invoke();
            Destroy(gameObject);
            return;
        }

        float speed = Mathf.Max(0.01f, speedUnitsPerSecond);
        spinDegPerSec = Mathf.Max(0f, spinDegreesPerSecond);

        Vector3 dir = toTarget.normalized;
        Vector3 rotatedDir = Quaternion.Euler(0f, 0f, launchAngleDegrees) * dir;

        Vector3 apex = startWorld + rotatedDir * (directDistance * 0.5f);
        apex.y += arcHeight;

        Vector3[] path = { startWorld, apex, targetWorld };

        KillTweens();

        sequence = DOTween.Sequence();
        sequence.SetLink(gameObject);

        // Movement: constant speed + linear feel
        Tween moveTween = transform
            .DOPath(path, speed, PathType.CatmullRom, PathMode.TopDown2D, resolution: 10)
            .SetSpeedBased(true)
            .SetEase(Ease.Linear);

        sequence.Append(moveTween);

        // Rotation: start immediately and keep spinning until arrival (not duration-based)
        sequence.OnUpdate(() =>
        {
            if (spinDegPerSec <= 0f) return;
            transform.Rotate(0f, 0f, spinDegPerSec * Time.deltaTime);
        });

        sequence.OnComplete(() =>
        {
            onArrived?.Invoke();
            Destroy(gameObject);
        });
    }

    private void KillTweens()
    {
        if (sequence != null)
        {
            sequence.Kill();
            sequence = null;
        }
    }
}
