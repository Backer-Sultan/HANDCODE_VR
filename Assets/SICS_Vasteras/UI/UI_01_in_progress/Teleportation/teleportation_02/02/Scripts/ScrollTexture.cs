/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using UnityEngine;

namespace HandCode
{
    public class ScrollTexture : MonoBehaviour
    {
        /* fields & properties */
        public float speed = 1f;
        public GameObject[] parts;
        
        private Renderer[] rends;



        /* methods & coroutines */

        private void Start()
        {
            rends = GetComponentsInChildren<Renderer>();
        }

        public void RunPaper()
        {
            foreach (Renderer r in rends)
            {
                r.material.mainTextureOffset = new Vector2(r.material.mainTextureOffset.y - Time.deltaTime * speed, r.material.mainTextureOffset.x);
            }
        }

        private void FixedUpdate()

        {
            RunPaper();
        }
    } 
}