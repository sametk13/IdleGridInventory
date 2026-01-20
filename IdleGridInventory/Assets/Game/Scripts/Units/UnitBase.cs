using System;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    public event Action<UnitBase> Death;

    public abstract IHealth Health { get; }

    protected virtual void OnEnable()
    {
        if (Health != null)
            Health.Died += HandleDied;
    }

    protected virtual void OnDisable()
    {
        if (Health != null)
            Health.Died -= HandleDied;
    }

    protected virtual void HandleDied()
    {
        Death?.Invoke(this);
    }
}
