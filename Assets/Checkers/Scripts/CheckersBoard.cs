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
        public Vector3 boardOffset = new Vector3(-4.0f, 0.0f, -4.0f);
        public Vector3 pieceOffset = new Vector3(.5f, 0, .5f);
        public float rayDistance = 1000f;
        public LayerMask hitLayers;
        
        public Piece[,] pieces = new Piece[8,8];

        /*
         * isHost = Is the player currently the host? (For networking)
         * isWhiteTurn = Is it current player's turn or opponent?
         * hasKilled = Did the player's piece get killed?
         */
        private bool isWhiteTurn = true, hasKilled;
        private Vector2 mouseOver, startDrag, endDrag;
        
        private Piece selectedPiece = null;

        private void Start()
        {
            GenerateBoard();
        }
        private void Update()
        {
            // Update the mouse over information
            MouseOver();
            // Is it currently white's turn?
            if(isWhiteTurn)
            {
                // Get x and y coordinate of selected mouse over
                int x = (int)mouseOver.x;
                int y = (int)mouseOver.y;
                // If the mouse is pressed
                if(Input.GetMouseButtonDown(0))
                {
                    // Try selecting piece
                    selectedPiece = SelectPiece(x, y);
                    startDrag = new Vector2(x, y);
                }
                // If there is a selected piece
                if(selectedPiece)
                {
                    // Move the piece with Mouse
                    DragPiece(selectedPiece);
                }
                // If button is released
                if (Input.GetMouseButtonUp(0))
                {
                    endDrag = new Vector2(x, y); // Record end drag
                    TryMove(startDrag, endDrag); // Try moving the piece
                    selectedPiece = null; // Let go of the piece
                }
            }
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
            GameObject clone = Instantiate(prefab, checkersParent);
            // Get the Piece component
            Piece p = clone.GetComponent<Piece>();
            // Update Piece X & Y with Current Location
            p.x = x;
            p.y = y;
            // Reposition clone
            MovePiece(p, x, y);
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
                    GeneratePiece(oddRow ? x : x + 1, y, true);
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
                    GeneratePiece(oddRow ? x : x + 1, y, false);
                }
            }
        }
        
        /// <summary>
        /// Selects a piece on the 2D grid and returns it
        /// </summary>
        /// <param name="x">Coordinate</param>
        /// <param name="y">Coordinate</param>
        /// <returns></returns>
        private Piece SelectPiece(int x, int y)
        {
            // Check if X and Y is out of bounds
            if(OutOfBounds(x, y))
                // Return result early
                return null;

            // Get the piece at X and Y location
            Piece piece = pieces[x, y];
            
            // Check that it is't null
            if(piece)     
                return piece;

            return null;
        }         
        
        /// <summary>
        /// Moves a Piece to another coordinate on a 2D grid.
        /// </summary>
        /// <param name="p">The Piece to move</param>
        /// <param name="x">X Location</param>
        /// <param name="y">Y Location</param>
        private void MovePiece(Piece p, int x, int y)
        {
            // Update array
            pieces[p.x, p.y] = null;
            pieces[x, y] = p;
            p.x = x;
            p.y = y;
            // Translate the piece to another location
            p.transform.localPosition = new Vector3(x, 0, y) + boardOffset + pieceOffset;
        }

        /// <summary>
        /// Updating when the pieces have been selected
        /// </summary>
        private void MouseOver()
        {
            // Perform Raycast from mouse position
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // If the ray hit the board
            if (Physics.Raycast(camRay, out hit, rayDistance, hitLayers))
            {
                // Convert mouse coordinates to 2D array coordinates
                mouseOver.x = (int)(hit.point.x - boardOffset.x);
                mouseOver.y = (int)(hit.point.z - boardOffset.z);
            }
            else // Otherwise
            {
                // Default to error (-1)
                mouseOver.x = -1;
                mouseOver.y = -1;
            }
        }

        /// <summary>
        /// Drags the selected piece using Raycast location
        /// </summary>
        /// <param name="p"></param>
        private void DragPiece(Piece selected)
        {
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // Detects mouse ray hit point
            if(Physics.Raycast(camRay, out hit, rayDistance, hitLayers))
            {
                // Updates position of selected piece to hit point + offset
                selected.transform.position = hit.point + Vector3.up;
            }
        }

        /// <summary>
        /// Tries moving a piece from Current (x1 + y1) to Desired (x2 + y2) coordinates
        /// </summary>
        /// <param name="x1">Current X</param>
        /// <param name="y1">Current Y</param>
        /// <param name="x2">Desired X</param>
        /// <param name="y2">Desired Y</param>
        private void TryMove(Vector2 start, Vector2 end)
        {
            int x1 = (int)start.x;
            int y1 = (int)start.y;
            int x2 = (int)end.x;
            int y2 = (int)end.y;

            // Record start Drag & end Drag
            startDrag = new Vector2(x1, y1);
            endDrag = new Vector2(x2, y2);
            
            // If there is a selected piece
            if (selectedPiece)
            {
                // Check if desired location is Out of Bounds
                if (OutOfBounds(x2, y2))
                {
                    // Move it back to original (start)
                    MovePiece(selectedPiece, x1, y1);
                    return; // Exit function!
                }

                // Check if it is a Valid Move
                if (ValidMove(start, end))
                {
                    //  Replace end coordinates with out selected piece
                    MovePiece(selectedPiece, x2, y2);
                }
                else
                {
                    // Move t back to original (start)
                    MovePiece(selectedPiece, x1, y1);
                }
            }
        }

        private bool OutOfBounds(int x, int y)
        {
            return x < 0 || x >= 8 || y < 0 || y >= 8;
        }

        private bool ValidMove(Vector2 start, Vector2 end)
        {
            int x1 = (int)start.x;
            int y1 = (int)start.y;
            int x2 = (int)end.x;
            int y2 = (int)end.y;

            // Is the start the same as the end?
            if(start == end)
            {
                // You can move back where you were
                return true;
            }

            // If you are moving on top of another piece
            if(pieces[x2, y2])
            {
                // YA CAN'T DO DAT!
                return false;
            }

            // Yeah... Alright, you can do dat.
            return true;
        }
    }
}