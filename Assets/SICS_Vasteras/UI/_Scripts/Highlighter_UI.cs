using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HandCode
{
    public class Highlighter_UI : Highlighter
    {
        internal Image[] images;
        internal Color[] initialColors;

        internal override void Start()
        {
            base.Start();

            images = GetComponentsInChildren<Image>();
            GetInitialColors();
        }

        internal override void OnEnable()
        {
            Highlight();
        }

        internal override void OnDisable()
        {
            Unhighlight();
        }

        private void GetInitialColors()
        {
            if (images != null && images.Length > 0)
            {
                initialColors = new Color[images.Length];

                for (int i = 0; i < images.Length; i++)
                {
                    initialColors[i] = images[i].color;
                }
            }
        }

        private void Highlight()
        {
            Animator animator = GetComponent<Animator>();
            if (animator != null)
                animator.enabled = false;

            if (images != null)
            {
                for (int i = 0; i < images.Length; i++)
                {
                    images[i].color = highlightColor;
                }
            }
        }

        private void Unhighlight()
        {
            Animator animator = GetComponent<Animator>();
            if (animator != null)
                animator.enabled = true;

            if (images != null)
            {
                for (int i = 0; i < images.Length; i++)
                {
                    images[i].color = initialColors[i];
                }
            }
        }
    }
}