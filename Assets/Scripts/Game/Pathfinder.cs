using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Pathfinder
{
    private Grid _grid;

    public Pathfinder(Grid grid)
    {
        _grid = grid;
    }

    public List<Cell> AStar(Cell start, Cell goal)
    {
        List<Cell> closedList = new List<Cell>();
        List<Cell> openList = new List<Cell>() { start };
        start.CameFrom = null;
        start.G = 0;
        start.H = HeuristicCostCount(start.Coords, goal.Coords);

        while (openList.Count > 0)
        {
            Cell current = GetLowestFCell(openList);
            if (current == goal)
                return PathReconstruct(goal);

            openList.Remove(current);
            closedList.Add(current);

            List<Cell> neighbours = GetNeighbours(current);
            foreach (Cell neigbour in neighbours)
            {
                if (closedList.Contains(neigbour))
                    continue;

                int tentativeG = current.G + 1;
                bool isTenativeBest = true;
                if (!openList.Contains(neigbour))
                    openList.Add(neigbour);
                else if (tentativeG > neigbour.G)
                    isTenativeBest = false;

                if (isTenativeBest)
                {
                    neigbour.CameFrom = current;
                    neigbour.G = tentativeG;
                    neigbour.H = HeuristicCostCount(neigbour.Coords, goal.Coords);
                }
            }
        }

        return null;
    }

    // Let moving in four directions so use Manhattan Distance
    private int HeuristicCostCount(Vector2Int current, Vector2Int goal)
    {
        return Math.Abs(current.x - goal.x) + Math.Abs(current.y - goal.y);
    }

    private Cell GetLowestFCell(List<Cell> cells)
    {
        Cell lowestFCell = cells[0];
        for (int i = 1; i < cells.Count; i++)
            if (lowestFCell.F > cells[i].F)
                lowestFCell = cells[i];

        return lowestFCell;
    }

    private List<Cell> GetNeighbours(Cell current)
    {
        List<Cell> neighbours = new List<Cell>();

        if (current.Coords.x - 1 >= 0)
        {
            Cell neighbour = _grid.Cells[current.Coords.x - 1, current.Coords.y];
            if (neighbour.IsCellFree) 
                neighbours.Add(neighbour);
        }

        if (current.Coords.x + 1 < _grid.Cells.GetLength(0))
        {
            Cell neighbour = _grid.Cells[current.Coords.x + 1, current.Coords.y];
            if (neighbour.IsCellFree)
                neighbours.Add(neighbour);
        }

        if (current.Coords.y - 1 >= 0)
        {
            Cell neighbour = _grid.Cells[current.Coords.x, current.Coords.y - 1];
            if (neighbour.IsCellFree)
                neighbours.Add(neighbour);
        }

        if (current.Coords.y + 1 < _grid.Cells.GetLength(1))
        {
            Cell neighbour = _grid.Cells[current.Coords.x, current.Coords.y + 1];
            if (neighbour.IsCellFree)
                neighbours.Add(neighbour);
        }

        return neighbours;
    }

    private List<Cell> PathReconstruct(Cell goal)
    {
        List<Cell> resultPath = new List<Cell>();

        Cell current = goal;
        while (current != null)
        {
            resultPath.Add(current);
            current = current.CameFrom;
        }

        resultPath.Reverse();
        return resultPath;
    }
}
