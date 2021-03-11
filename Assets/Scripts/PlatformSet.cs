using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSet : MonoBehaviour
{
    public bool rotateX;
    public bool rotateY;
    public bool rotateZ;
    public bool fixedmovement;

    public int rotationSpeed = 200;

    void FixedUpdate()
    {
        SpinnerX(); SpinnerY(); SpinnerZ();
    }

    public void SpinnerX() // X eksenin döndürmek için
    {
        if (rotateX == true) transform.Rotate(rotationSpeed * Time.deltaTime, 0, 0);
    }

    public void SpinnerY() // Y eksenin döndürmek için
    {
        if (rotateY == true) transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }

    public void SpinnerZ() // Z eksenin döndürmek için
    {
        if (rotateZ == true) transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
    
}
  