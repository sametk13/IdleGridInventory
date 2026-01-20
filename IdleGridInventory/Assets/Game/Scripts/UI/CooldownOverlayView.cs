using UnityEngine;
using UnityEngine.UI;

public sealed class CooldownOverlayView : MonoBehaviour,IBrainModule
{
    [SerializeField] private Image overlayFillImage;

    
    private IBrain brain;
    private void Awake()
    {
        SetCooldownFill01(0);
    }
    public void Inject(IBrain brain)
    {
        this.brain = brain;
    }

    // Requirement: fillAmount goes from 1 -> 0 while cooldown runs
    public void SetCooldownFill01(float value01)
    {
        if (overlayFillImage == null) return;
        overlayFillImage.fillAmount = Mathf.Clamp01(value01);
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }
}
