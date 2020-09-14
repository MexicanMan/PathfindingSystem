using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Who owns item (who can take it)")]
    private Hero _afillation = null;

    public Hero Afillation { get { return _afillation; } }
}
