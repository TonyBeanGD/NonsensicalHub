using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NonsensicalKit
{
    [RequireComponent(typeof(Image))]
    public class RandomColor : MonoBehaviour
    {
        private Image img;

        private Vector3 color;

        private void Start()
        {
            img = GetComponent<Image>();
            color = new Vector3(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        }

        private void Update()
        {
            color.x += Random.Range(0.003f, 0.006f);
            color.y += Random.Range(0.003f, 0.006f);
            color.z += Random.Range(0.003f, 0.006f);

            float r = color.x % 2 > 1 ? 1 - color.x % 1 : color.x % 1;
            float g = color.y % 2 > 1 ? 1 - color.y % 1 : color.y % 1;
            float b = color.z % 2 > 1 ? 1 - color.z % 1 : color.z % 1;

            img.color = new Color(r, g, b);
        }
    }

}
