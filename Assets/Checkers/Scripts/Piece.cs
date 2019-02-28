using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Checkers
{
    public class Piece : MonoBehaviour
    {
        public bool isWhite, isKing;
        public int x, y;

        private Animator anim;
        
        private void Start()
        {
            anim = GetComponent<Animator>();
        }

        public void King()
        {
            isKing = true;
            anim.SetTrigger("King");
        }
    } 
}
