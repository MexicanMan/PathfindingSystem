using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    private const string ObstacleTag = "Obstacle";

    private MeshRenderer _meshRenderer;

    public Vector2Int Coords { get; set; }
    public bool IsCellFree { get; private set; }

    // Pathfinding part
    public int G { get; set; }
    public int H { get; set; }
    public int F { get { return G + H; } }
    public Cell CameFrom { get; set; }

    protected void Awake()
    {
        IsCellFree = true;

        _meshRenderer = GetComponent<MeshRenderer>();
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(ObstacleTag))
            BlockCell();
    }

    public void TargetCell(Color color)
    {
        _meshRenderer.material.color = color;
    }

    public void UntargetCell()
    {
        _meshRenderer.material.color = Color.white;
    }

    private void BlockCell()
    {
        IsCellFree = false;
        _meshRenderer.material.color = Color.red;
    }
}
