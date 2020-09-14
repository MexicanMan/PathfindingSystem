using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Grid : MonoBehaviour
{
	private const float PlaneStandartSize = 10f;

    [SerializeField]
    private int _width = 10;
    [SerializeField]
    private int _height = 10;

    [SerializeField]
    private Cell _cellPrefab = null;

	private Cell[,] _cells;
	private Pathfinder _pathfinder;

	public Cell[,] Cells { get { return _cells; } }

	protected void Awake()
	{
		_pathfinder = new Pathfinder(this);

		UpdateGrid();
	}

	#if UNITY_EDITOR

	protected void Update()
	{
		if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
			UpdateGrid();
	}

	#endif

	public List<Cell> FindPath(Vector2Int start, Vector2Int goal)
	{
		Cell startCell = _cells[start.x, start.y];
		Cell goalCell = _cells[goal.x, goal.y];

		return _pathfinder.AStar(startCell, goalCell);
	}

	private void UpdateGrid()
	{
		ClearGrid();

		_cells = new Cell[_width, _height];

		for (int z = 0; z < _height; z++)
		{
			for (int x = 0; x < _width; x++)
			{
				CreateCell(x, z);
			}
		}
	}

	private void ClearGrid()
	{
		Cell[] currentChildrenCells = GetComponentsInChildren<Cell>();
		foreach (var cell in currentChildrenCells)
			DestroyImmediate(cell.gameObject);
	}

	private void CreateCell(int x, int z)
	{
		Vector3 position = new Vector3(x * PlaneStandartSize, 0f, z * PlaneStandartSize);

		_cells[x, z] = Instantiate(_cellPrefab);
		_cells[x, z].Coords = new Vector2Int(x, z);
		_cells[x, z].transform.SetParent(transform, false);
		_cells[x, z].transform.localPosition = position;
	}
}
