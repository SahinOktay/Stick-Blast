using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

namespace Garawell
{
    public class BlockCreator : EditorWindow
    {
        private char[] _invalidChars;
        private List<SingleConnection> _connections;
        private string _prefabName = "New Block";

        private const string prefabFolderPath = "Assets/02Prefabs/Blocks/";

        [MenuItem("Tools/BlockCreator")]
        public static void OpenWindow()
        {
            BlockCreator blockCreator = GetWindow<BlockCreator>();
            blockCreator.titleContent = new GUIContent("Block Creator");
            blockCreator.Show();
        }

        private bool IsValidFileName(string s) => !s.Any(c => _invalidChars.Contains(c));

        private void CreateGUI()
        {
            _connections = new List<SingleConnection>();
            _invalidChars = System.IO.Path.GetInvalidFileNameChars();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Connections");

            bool hasInvaildConnection = false;

            for (int i = 0; i < _connections.Count; i++)
            {
                EditorGUILayout.LabelField("Connection " + i);
                EditorGUILayout.BeginHorizontal();

                _connections[i] = new SingleConnection()
                {
                    firstPoint = EditorGUILayout.Vector2IntField("firstPoint ", _connections[i].firstPoint),
                    secondPoint = EditorGUILayout.Vector2IntField("secondPoint", _connections[i].secondPoint)
                };

                EditorGUILayout.EndHorizontal();

                if (
                    (_connections[i].firstPoint.x != _connections[i].secondPoint.x
                    &&
                    _connections[i].firstPoint.y != _connections[i].secondPoint.y)
                    ||
                    _connections[i].firstPoint == _connections[i].secondPoint
                )
                {
                    GUI.color = Color.Lerp(Color.yellow, Color.red, .5f);
                    EditorGUILayout.LabelField("Invalid Connection");
                    GUI.color = Color.white;
                    hasInvaildConnection = true;
                }

                EditorGUILayout.Space();
            }

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("-")) _connections.RemoveAt(_connections.Count - 1);
            if (GUILayout.Button("+")) _connections.Add(new SingleConnection());

            EditorGUILayout.EndHorizontal();

            _prefabName = EditorGUILayout.TextField(new GUIContent("Prefab Name"), _prefabName);

            bool isValid = !File.Exists(prefabFolderPath + _prefabName + ".prefab") &&
                IsValidFileName(_prefabName) &&
                _prefabName != "";

            if (!isValid) {
                GUI.color = Color.Lerp(Color.yellow, Color.red, .5f);
                EditorGUILayout.LabelField("File name exists or contains invalid characters");
                GUI.color = Color.white;
                return;
            }

            if (hasInvaildConnection) return;

            if (_connections.Count == 0) return;

            if (GUILayout.Button("CreatePrefab"))
            {
                GameObject iBlockPref = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/02Prefabs/Blocks/IBlock.prefab");
                GameObject newBlock = new GameObject(_prefabName);
                GameObject spawnedBlock;

                IEnumerable<ConnectionPoints> connectionPoints = new List<ConnectionPoints>();

                Vector2 min =
                    new Vector2Int(
                        _connections.Select(item => item.firstPoint.x < item.secondPoint.x ? item.firstPoint.x : item.secondPoint.x).Min(),
                        _connections.Select(item => item.firstPoint.y < item.secondPoint.y ? item.firstPoint.y : item.secondPoint.y).Min()
                    );

                Vector2 max =
                    new Vector2Int(
                        _connections.Select(item => item.firstPoint.x > item.secondPoint.x ? item.firstPoint.x : item.secondPoint.x).Max(),
                        _connections.Select(item => item.firstPoint.y > item.secondPoint.y ? item.firstPoint.y : item.secondPoint.y).Max()
                    );

                Vector3 offset = (min + max) * .5f;

                for (int i = 0; i < _connections.Count; i++)
                {
                    spawnedBlock = (GameObject)PrefabUtility.InstantiatePrefab(iBlockPref, newBlock.transform);

                    PlacableBlock iBlockPlacable = spawnedBlock.GetComponent<PlacableBlock>();
                    connectionPoints = connectionPoints.Concat(iBlockPlacable.nodePoints);
                    DestroyImmediate(iBlockPlacable);

                    if (_connections[i].firstPoint.x == _connections[i].secondPoint.x)
                        spawnedBlock.transform.localPosition = new Vector3(
                            _connections[i].secondPoint.x,
                            (_connections[i].firstPoint.y + _connections[i].secondPoint.y) * .5f,
                            0
                        ) - offset;
                    else
                    {
                        spawnedBlock.transform.localPosition = new Vector3(
                            (_connections[i].firstPoint.x + _connections[i].secondPoint.x) * .5f,
                            _connections[i].firstPoint.y,
                            0
                        ) - offset;

                        spawnedBlock.transform.eulerAngles = new Vector3(0, 0, 90);
                    }
                }

                PlacableBlock placableBlock = newBlock.AddComponent<PlacableBlock>();
                placableBlock.nodePoints = connectionPoints.ToArray();
                placableBlock.connections = _connections.ToArray();

                PrefabUtility.SaveAsPrefabAsset(newBlock, prefabFolderPath + _prefabName + ".prefab");
                DestroyImmediate(newBlock);
            }
        }
    }
}