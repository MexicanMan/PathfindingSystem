using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hero : MonoBehaviour
{
    private const float EpsMovement = 0.5f;

    private readonly UnityEvent _onTargeted = new UnityEvent();
    private readonly UnityEvent<bool> _onReachedGoal = new UnityEvent<bool>();

    [SerializeField]
    private HeroState _state = null;

    [SerializeField]
    [Tooltip("Grid that hero walk on")]
    private Grid _grid = null;

    [SerializeField]
    [Tooltip("Cell coordinates where hero should spawn")]
    private Vector2Int _spawnPosition = Vector2Int.zero;
    [SerializeField]
    [Tooltip("How much items should be picked up to finish level")]
    private int _goalToCollect = 3;

    private bool _isDragging = false;
    private int _currentPathIndex;
    private int _currentPickedItems;

    private Vector2Int _currentPos;
    private List<Cell> _currentPath;
    private Cell _target;

    private Color _heroColor;
    private Collider _collider;
    private Rigidbody _rigidbody;

    public float Speed { get { return _state.Level + 30f; } }
    public bool IsControllable { get; set; } = true;

    protected void Start()
    {
        Cell spawnCell;
        try
        {
            spawnCell = _grid.Cells[_spawnPosition.x, _spawnPosition.y];
        }
        catch (IndexOutOfRangeException)
        {
            Debug.LogError($"Bad spawn position for hero: {gameObject.name}");
            return;
        }

        transform.position = new Vector3(spawnCell.transform.position.x, transform.localScale.y, spawnCell.transform.position.z);
        _currentPos = _spawnPosition;
        _target = null;
        _currentPickedItems = 0;

        _heroColor = GetComponent<MeshRenderer>().material.color;
        _collider = GetComponent<Collider>();
        _rigidbody = GetComponent<Rigidbody>();

        try
        {
            _onTargeted.AddListener(MainBridge.Instance.OnHeroReady);
            _onReachedGoal.AddListener(MainBridge.Instance.OnHeroReachGoal);

            MainBridge.Instance.OnMovementStart += StartMovement;
        }
        catch (NullReferenceException)
        {
            Debug.LogError("No main scene was loaded or/and bridge object was not created!");
            return;
        }
    }

    protected void Update()
    {
        if (!IsControllable)
            HandleMovement();
    }

    protected void OnDestroy()
    {
        MainBridge.Instance.OnMovementStart -= StartMovement;
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(gameObject.tag))
        {
            Physics.IgnoreCollision(collision.collider, _collider);
        }
        else
        {
            Item item = collision.gameObject.GetComponent<Item>();
            if (item != null)
            {
                Physics.IgnoreCollision(collision.collider, _collider);
                if (item.Afillation == this)
                {
                    item.gameObject.SetActive(false);

                    _currentPickedItems++;
                }
            }
        }
    }

    protected void OnMouseDrag()
    {
        if (IsControllable)
            _isDragging = true;
    }

    protected void OnMouseUp()
    {
        if (!_isDragging)
            return;
        _isDragging = false;

        if (_currentPath != null)
            RemoveCurrentPath();
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Cell targetCell = hit.transform.GetComponent<Cell>();
            if (targetCell != null && targetCell.IsCellFree)
            {
                if (_target == null)
                    _onTargeted.Invoke();

                _target = targetCell;
                _target.TargetCell(_heroColor);

                _currentPath = _grid.FindPath(_currentPos, _target.Coords);  
                _currentPathIndex = 0;
            }
        }
    }

    private void StartMovement()
    {
        IsControllable = false;
    }

    private void HandleMovement()
    {
        if (_currentPath == null)
            return;

        Cell currentTargetCell = _currentPath[_currentPathIndex];
        Vector3 currentTargetPos = new Vector3(currentTargetCell.transform.position.x, transform.position.y, currentTargetCell.transform.position.z);
        if (Vector3.Distance(transform.position, currentTargetPos) > EpsMovement)
        {
            Vector3 moveDir = (currentTargetPos - transform.position).normalized;
            transform.position += moveDir * Speed * Time.deltaTime;
        }
        else
        {
            _currentPos = currentTargetCell.Coords;
            if (++_currentPathIndex >= _currentPath.Count)
            {
                CheckGoalReach();

                RemoveCurrentPath();
            }
        }
    }

    private void CheckGoalReach()
    {
        _onReachedGoal.Invoke(_currentPickedItems >= _goalToCollect);
    }

    private void RemoveCurrentPath()
    {
        _target.UntargetCell();
        _currentPath = null;

        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }
}
