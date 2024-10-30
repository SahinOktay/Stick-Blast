using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Garawell
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private Canvas gridBackgroundCanvas;
        [SerializeField] private GameObject cellCenter, node, connectionVertical;
        [SerializeField] private Vector2Int gridSize;

        private GameObject nodeParent, connectionParent;
        private List<Vector2Int> _blastCoordinates = new();

        private Cell[][] _cells;
        private Dictionary<SingleConnection, GridConnection> _connections;
        private Vector3 gridStartPoint;

        public void GenerateGrid()
        {
            _connections = new Dictionary<SingleConnection, GridConnection>();

            nodeParent = new GameObject("Node parent");
            connectionParent = new GameObject("Connection parent");

            gridStartPoint = new Vector3(
                -gridSize.x * .5f * Constants.Numbers.CELL_SIZE,
                -gridSize.y * .5f * Constants.Numbers.CELL_SIZE,
                0
            );

            Vector3 currentNodePose, currentCellCenter;

            List<GridConnection> currentRowHorCons = new List<GridConnection>();
            List<GridConnection> downRowHorCons = null;
            List<List<Cell>> cellList = new List<List<Cell>>();

            GridConnection currentConnection;
            List<GridConnection> lastTwoVerticalConnections = new List<GridConnection>();

            Cell newCell;
            SingleConnection connectionInDict;
            for ( int y = 0; y <= gridSize.y; y++ )
            {
                if (y > 0) cellList.Add(new List<Cell>());
                currentRowHorCons.Clear();
                lastTwoVerticalConnections.Clear();

                for (int x = 0; x <= gridSize.x; x++)
                {
                    currentNodePose = gridStartPoint + x * Vector3.right + y * Vector3.up;
                    currentCellCenter = currentNodePose + (Vector3.left + Vector3.down) * .5f * Constants.Numbers.CELL_SIZE;
                    Instantiate(node, currentNodePose, Quaternion.identity, nodeParent.transform);

                    if (x > 0)
                    {
                        currentConnection = Instantiate(
                            connectionVertical, 
                            currentNodePose + Vector3.left * .5f * Constants.Numbers.CELL_SIZE, 
                            Quaternion.Euler(0, 0, 90),
                            connectionParent.transform
                        ).GetComponent<GridConnection>();

                        connectionInDict = new SingleConnection()
                        {
                            firstPoint = new Vector2Int(x - 1, y),
                            secondPoint = new Vector2Int(x, y)
                        };
                        _connections.Add(connectionInDict,currentConnection);

                        currentRowHorCons.Add(currentConnection);
                    }

                    if (y > 0)
                    {
                        currentConnection = Instantiate(
                                connectionVertical,
                                currentNodePose + Vector3.down * .5f * Constants.Numbers.CELL_SIZE,
                                Quaternion.identity,
                                connectionParent.transform
                            ).GetComponent<GridConnection>();

                        lastTwoVerticalConnections.Add(currentConnection);

                        connectionInDict = new SingleConnection()
                        {
                            firstPoint = new Vector2Int(x, y - 1),
                            secondPoint = new Vector2Int(x, y)
                        };
                        _connections.Add(
                            connectionInDict,
                            currentConnection
                        );
                    }

                    if (x > 0 && y > 0)
                    {
                        newCell = new Cell(
                            new List<GridConnection>()
                            {
                                downRowHorCons[x - 1],
                                currentRowHorCons[x - 1],
                                lastTwoVerticalConnections[0],
                                lastTwoVerticalConnections[1]
                            },
                            Instantiate(cellCenter, currentCellCenter, Quaternion.identity)
                        );

                        cellList[^1].Add(newCell);

                        lastTwoVerticalConnections.RemoveAt(0);
                    }
                }

                downRowHorCons = currentRowHorCons.ToList();
            }

            _cells = cellList.Select(item => item.ToArray()).ToArray();
        }

        private void CheckBlast(Vector2Int pos)
        {
            bool horizontalBlast = true, verticalBlast = true;

            // Horizontal Check
            for (int x = 0; x < _cells[pos.y].Length; x++)
                horizontalBlast &= _cells[pos.y][x].IsFilled;

            // Vertical Check
            for (int y = 0; y < _cells.Length; y++)
                verticalBlast &= _cells[y][pos.x].IsFilled;

            if (horizontalBlast)
                for (int x = 0; x < _cells[pos.y].Length; x++)
                    _cells[pos.y][x].ClearCell();

            if (verticalBlast)
                for (int y = 0; y < _cells.Length; y++)
                    _cells[y][pos.x].ClearCell();

            if (horizontalBlast)
            {
                for (int x = 0; x < _cells[pos.y].Length; x++)
                {
                    if (pos.y > 0)
                        _cells[pos.y - 1][x].CheckFillStatus();

                    if (pos.y < _cells.Length - 1)
                        _cells[pos.y + 1][x].CheckFillStatus();
                }
            }

            if (verticalBlast)
            {
                for (int y = 0; y < _cells.Length; y++)
                {
                    if (pos.x > 0)
                    _cells[y][pos.x - 1].CheckFillStatus();

                    if (pos.x < _cells[y].Length - 1)
                        _cells[y][pos.x + 1].CheckFillStatus();
                }
            }
        }

        private void OnPlacementComplete(ButtonEventTrigger button, PlacableBlock block)
        {
            for (int i = 0; i < _blastCoordinates.Count; i++)
                CheckBlast(_blastCoordinates[i]);
        }

        public bool HasAnyAvailableSpot(PlacableBlock block)
        {
            SingleConnection currentConToCheck;
            bool hasAvailableSpot = false;
            for (int y = 0; y <= gridSize.y; y++)
            {
                for (int x = 0; x <= gridSize.x; x++)
                {
                    hasAvailableSpot = false;
                    for (int i = 0; i < block.connections.Length; i++)
                    {
                        currentConToCheck = new SingleConnection()
                        {
                            firstPoint = new Vector2Int(x, y) + block.connections[i].firstPoint,
                            secondPoint = new Vector2Int(x, y) + block.connections[i].secondPoint
                        };

                        if (!_connections.ContainsKey(currentConToCheck))
                        {
                            hasAvailableSpot = false;
                            break;
                        }

                        if (_connections[currentConToCheck].IsConnected)
                        {
                            hasAvailableSpot = false;
                            break;
                        }

                        hasAvailableSpot = true;
                    }

                    if (hasAvailableSpot) return true;
                }
            }

            return false;
        }

        public void DisablePreviews()
        {
            foreach (GridConnection con in _connections.Values)
                con.SetPreview(false);
        }

        public bool PlaceBlock(PlacableBlock block, out int filledBlocks)
        {
            filledBlocks = 0;
            DisablePreviews();
            HashSet<GridConnection> placementConnections = CheckPlacement(block, false, out Vector3 offset);
            if (placementConnections.Count == 0) return false;

            foreach (GridConnection con in placementConnections)
                con.Connect();

            block.Placed += OnPlacementComplete;
            block.Place(placementConnections, -offset);

            _blastCoordinates.Clear();
            for (int y = 0; y < _cells.Length; y++)
                for (int x = 0; x < _cells[y].Length; x++)
                {
                    if (_cells[y][x].CheckFillStatus())
                    {
                        filledBlocks++;
                        _blastCoordinates.Add(new Vector2Int(x, y));
                    }
                }

            return true;
        }

        public HashSet<GridConnection> CheckPlacement(PlacableBlock block, bool showPreview, out Vector3 offsetToPlacement)
        {
            HashSet<GridConnection> previewConnections = new();

            offsetToPlacement = Vector3.zero;
            Vector3 point1Offset, point2Offset;
            Vector2Int point1OffsetRounded, point2OffsetRounded;

            bool point1Smaller;
            SingleConnection connectionInDict;
            for (int i = 0; i < block.nodePoints.Length; i++)
            {
                point1Offset = (block.nodePoints[i].point1.position - gridStartPoint) / Constants.Numbers.CELL_SIZE;
                point1OffsetRounded = new Vector2Int(
                    Mathf.RoundToInt(point1Offset.x),
                    Mathf.RoundToInt(point1Offset.y)
                );

                offsetToPlacement = gridStartPoint + new Vector3(point1OffsetRounded.x, point1OffsetRounded.y) * Constants.Numbers.CELL_SIZE -
                        block.nodePoints[i].point1.position;
                if (
                    Vector3.Magnitude(offsetToPlacement) > Constants.Numbers.PLACEMENT_THRESHOLD
                )
                {
                    DisablePreviews();
                    previewConnections.Clear();
                    return previewConnections;
                }

                point2Offset = (block.nodePoints[i].point2.position - gridStartPoint) / Constants.Numbers.CELL_SIZE;
                point2OffsetRounded = new Vector2Int(
                    Mathf.RoundToInt(point2Offset.x),
                    Mathf.RoundToInt(point2Offset.y)
                );

                if (point1OffsetRounded.x == point2OffsetRounded.x)
                    point1Smaller = point1OffsetRounded.y < point2OffsetRounded.y; 
                else
                    point1Smaller = point1OffsetRounded.x < point2OffsetRounded.x;

                connectionInDict = new SingleConnection()
                {
                    firstPoint = point1Smaller ? point1OffsetRounded : point2OffsetRounded,
                    secondPoint = point1Smaller ? point2OffsetRounded : point1OffsetRounded
                };

                if (!_connections.ContainsKey(connectionInDict))
                {
                    DisablePreviews();
                    previewConnections.Clear();
                    return previewConnections;
                }

                if (_connections[connectionInDict].IsConnected)
                {
                    DisablePreviews();
                    previewConnections.Clear();
                    return previewConnections;
                }

                previewConnections.Add(_connections[connectionInDict]);
            }

            if (showPreview)
                foreach (GridConnection con in _connections.Values)
                    con.SetPreview(previewConnections.Contains(con));

            return previewConnections;
        }
    }
}

[Serializable]
public struct SingleConnection
{
    public Vector2Int firstPoint;
    public Vector2Int secondPoint;
}