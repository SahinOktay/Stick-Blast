using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Garawell
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private Canvas gridBackgroundCanvas;
        [SerializeField] private GameObject node, connectionVertical;
        [SerializeField] private Vector2Int gridSize;

        private GameObject nodeParent, connectionParent;

        private Cell[][] _cells;

        public void GenerateGrid()
        {
            nodeParent = new GameObject("Node parent");
            connectionParent = new GameObject("Connection parent");

            Vector3 gridStartPoint = new Vector3(
                -gridSize.x * .5f * Constants.Numbers.CELL_SIZE,
                -gridSize.y * .5f * Constants.Numbers.CELL_SIZE,
                0
            );

            Vector3 currentNodePose;

            List<GridConnection> currentRowHorCons = new List<GridConnection>();
            List<GridConnection> downRowHorCons = null;
            List<List<Cell>> cellList = new List<List<Cell>>();

            GridConnection currentConnection;
            List<GridConnection> lastTwoVerticalConnections = new List<GridConnection>();

            for ( int y = 0; y <= gridSize.y; y++ )
            {
                if (y > 0) cellList.Add(new List<Cell>());
                currentRowHorCons.Clear();
                lastTwoVerticalConnections.Clear();

                for (int x = 0; x <= gridSize.x; x++)
                {
                    currentNodePose = gridStartPoint + x * Vector3.right + y * Vector3.up;
                    Instantiate(node, currentNodePose, Quaternion.identity, nodeParent.transform);

                    if (x > 0)
                    {
                        currentConnection = Instantiate(
                            connectionVertical, 
                            currentNodePose + Vector3.left * .5f * Constants.Numbers.CELL_SIZE, 
                            Quaternion.Euler(0, 0, 90),
                            connectionParent.transform
                        ).GetComponent<GridConnection>();

                        currentRowHorCons.Add(currentConnection);
                    }

                    if (y > 0)
                    {
                        lastTwoVerticalConnections.Add(
                            Instantiate(
                                connectionVertical,
                                currentNodePose + Vector3.down * .5f * Constants.Numbers.CELL_SIZE,
                                Quaternion.identity,
                                connectionParent.transform
                            ).GetComponent<GridConnection>()
                        );
                    }

                    if (x > 0 && y > 0)
                    {
                        cellList[^1].Add(new Cell());

                        cellList[^1][^1].down = downRowHorCons[x - 1];
                        cellList[^1][^1].up = currentRowHorCons[x - 1];
                        cellList[^1][^1].left = lastTwoVerticalConnections[0];
                        cellList[^1][^1].right = lastTwoVerticalConnections[1];

                        lastTwoVerticalConnections.RemoveAt(0);
                    }
                }

                downRowHorCons = currentRowHorCons.ToList();
            }

        }
    }
}