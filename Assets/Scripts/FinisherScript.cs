using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FinisherScript : MonoBehaviour
{
    public GameObject Plane;
    public GameObject Controller;
    public GameObject Camera2;
    public GameObject[] members;
    public int currentposition;
    public Dictionary<GameObject,float> distlist = new Dictionary<GameObject, float>();
    public TMPro.TextMeshProUGUI position;


    void OnTriggerEnter(Collider col)
    {
       if (col.gameObject.tag == ("Player")){

           Camera2.SetActive(true);
           Debug.Log("Finish Line");
           Plane.SetActive(true);
                
           Controller.SetActive(false);
                
           Destroy(Camera.main);
           Destroy(col.gameObject);
       }
    }

    public void Update()
    {
        positionUI();
    }

    public void positionUI() // Pozisyon sıralamasını tutuyor
    {
        List<Member> mlist = new List<Member>(); // Sıralama için yarışmacıların bilgisini tutan liste tanımlaması
        for (int i = 0; i < 11; i++)
        {
            float distance = Vector3.Distance(gameObject.transform.position, members[i].transform.position); // Bitiş çizgisine olan uzaklıklar hesaplanıyor.
  
            Member m = new Member();
            m.dist = distance;   // Objenin bitişe olan uzaklıgı kaydediliyor
            m.User = members[i]; // Objenin ismi kaydediliyor
            mlist.Add(m); // Oluşturulan objeler listeye kaydediliyor.
        }
       
        mlist = mlist.OrderBy(s => s.dist).ToList(); // Liste sıralamaya sokuluyor.
        Member m1 = mlist.Where(s => s.User == members[0]).FirstOrDefault(); 

        for (int i = 0; i < mlist.Count; i++) // SIralanmış objeden Player'ı bularak sıralamayı arayüze gönderiyoruz.
        {
            if(mlist[i].User == members[0])
            {
                currentposition = i + 1;
                position.text = currentposition + "/11";


            }
        }

    }
}

public class Member // Verilerin tutulması için oluşturulan Class.
{
    public GameObject User { get; set; }
    public float dist { get; set; }
}
