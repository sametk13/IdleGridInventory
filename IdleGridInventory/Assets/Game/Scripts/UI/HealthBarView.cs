using UnityEngine;
using UnityEngine.UI;

public sealed class HealthBarView : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    public void SetNormalized(float value01)
    {
        value01 = Mathf.Clamp01(value01);
        if (fillImage != null)
            fillImage.fillAmount = value01;
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }
}
