using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HandCode
{
    public class TapePieceHologram : Hologram
    {
        public Transform TapePiece;

        private void OnTriggerEnter(Collider other)
        {
            if(other.name == "SmallTapeRoll_01")
            {
                gameObject.SetActive(false);
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            transform.position = TapePiece.position;
            transform.rotation = TapePiece.rotation;
        }
    }

}