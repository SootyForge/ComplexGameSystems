using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Checkers
{
    public class CheckersBoard : MonoBehaviour
    {
        [Tooltip("Prefabs for Checker Pieces")]
        public GameObject whitePiecePrefab, blackPiecePrefab;
        [Tooltip("Where to attach the spawned pieces in the Hierarchy")]
        public Transform checkersParent;
        public Vector3 boardOffset = new Vector3(-4, 0, -4);
        public Vector3 pieceOffset = new Vector3(.5f, 0, .5f);
        void Start()
        {
            GenerateBoard();
        }

        /// <summary>
        /// Generates a Checker Piece in specified coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void GeneratePiece(int x, int y, bool isWhite)
        {
            // What prefab are we using (white or black)
            GameObject prefab = isWhite ? whitePiecePrefab : blackPiecePrefab;
            // Generate Instance of prefab
            GameObject clone = Instantiate(prefab);
            // Reposition clone
            clone.transform.localPosition = new Vector3(x, 0, y) + boardOffset + pieceOffset;
        }

        /// <summary>
        /// Clears and re-generates entire board
        /// </summary>
        public void GenerateBoard()
        {
            // Generate White Team
            for (int y = 0; y < 3; y++)
            {
                bool oddRow = y % 2 == 0;
                // Loop through columns
                for (int x = 0; x < 8; x += 2)
                {
                    // Generate Piece
                    GeneratePiece(x, y, true);
                }
            }
            // Generate Black Team
            for (int y = 5; y < 8; y++)
            {
                bool oddRow = y % 2 == 0;
                // Loop through columns
                for (int x = 0; x < 8; x += 2)
                {
                    // Generate Piece
                    GeneratePiece(x, y, false);
                }
            }
        }
    } 
}