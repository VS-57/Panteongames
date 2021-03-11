using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintPanel : MonoBehaviour
{
    public GameObject Brush; // Fırça
    public float BrushSize = 0.1f; // Fırça boyutu

    void Update()
    {
        if (Input.GetMouseButton(0)) // Dokunulan noktada fırça objesinin üretilmesini sağlıyor
        {
            var Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(Ray, out hit))
            {
                var go = Instantiate(Brush, hit.point + Vector3.up * 0.1f, Quaternion.identity, transform);
                go.transform.Rotate(0f, 0, 271f);
                go.transform.localScale = Vector3.one * BrushSize;
            }
        }
    }
}
