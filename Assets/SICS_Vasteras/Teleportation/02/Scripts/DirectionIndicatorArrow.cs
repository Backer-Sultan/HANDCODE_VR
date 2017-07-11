/*********************************************
 * Project: HANDCODE                         *
 * Author:  Backer Sultan                    *
 * Email:   backer.sultan@ri.se              *
 * *******************************************/

using UnityEngine;

namespace HandCode
{
    public class DirectionIndicatorArrow : MonoBehaviour
    {
        /* fields & properties */

        public float speed = 0.2f;

        private Animator animator;



        /* methods & coroutines */

        private void Start()
        {
            // initialization
            animator = GetComponent<Animator>();
            if (animator == null)
                Debug.LogError(string.Format("{0}\nDirectionIndicatorArrow.cs: Component `Animator` is missing!", Machine.GetPath(gameObject)));
        }

        private void Update()
        {
            transform.localPosition += Vector3.up * speed * Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.name == "DestroyTrigger")
            {
                animator.SetTrigger("FadeOut");
                Destroy(gameObject, 1f);
            }
        }
    } 
}
