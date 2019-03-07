using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CheckersMkII
{
    public class Piece : MonoBehaviour
    {
        public bool isWhite, isKing; // Flag to store if white or king
                                     // Stores X and Y cell location in grid
        public Vector2Int cell, oldCell;

        private Animator anim; // Reference to animator

        // Awake runs before Start
        void Awake()
        {
            // Get reference to Animator component
            anim = GetComponent<Animator>();
        }

        public void King()
        {
            // This piece is now king
            isKing = true;
            // Trigger King animation
            anim.SetTrigger("King");
        }
    } 
}
