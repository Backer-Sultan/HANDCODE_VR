/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using UnityEngine;

namespace HandCode
{
    public class Hologram : MonoBehaviour
    {
        /* fields & properties */

        [Range(0f, 1f)]
        public float speed = 0.5f;
        public Color color1, color2;
        public GameObject obj;

        private Color lerpedColor;
        private Renderer[] rends;



        /* methods & coroutines */

        private void Start()
        {
            if (obj == null)
            {
                Debug.LogError(string.Format("{0}\nHologram: Object reference is missing!", Machine.GetPath(gameObject)));
                return;
            }
            rends = GetComponentsInChildren<Renderer>();
        }

        // general color lerp - make sure to call it from derrived methods!
        internal virtual void Update()
        {
            lerpedColor = Color.Lerp(color1, color2, Mathf.PingPong(Time.time * 2f, 1));
            foreach (Renderer rend in rends)
                rend.material.color = lerpedColor;
        }
    }
}