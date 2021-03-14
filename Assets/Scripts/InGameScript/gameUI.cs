using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


public class gameUI : MonoBehaviour
{
    public GameObject StartUI;

    void Start()
    {
        Time.timeScale = 0; // Oyunun açılışında durdurulmasını sağlıyor.

    }

    public void StartButton()// Oyunu başlatıyor
    {
        Time.timeScale = 1; 
        StartUI.SetActive(false);
    }

    public void RestartLevel() // Bölümü tekrar yüklüyor
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
