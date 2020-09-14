using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Bridge between game level scene and HUD main scene for throwing events between them
/// </summary>
public class MainBridge : MonoBehaviour
{
    public UnityAction OnHeroReady;
    public UnityAction<bool> OnHeroReachGoal;

    public UnityAction OnMovementStart;

    public static MainBridge Instance { get; private set; }

    protected void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);
    }
}
