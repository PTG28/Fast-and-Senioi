using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CarSelection : MonoBehaviour
{
    
    public GameObject[] cars;
    public Button next;
    public Button prev;
    int index;
    void Start()
    {
        index = PlayerPrefs.GetInt("carIndex");

        for(int i = 0; i < cars.Length; i++)
        {
            cars[i].SetActive(false);
            cars[index].SetActive(true);
        }
    }

   
    void Update()
    {
        if(index >= 2)
        {
            next.interactable = false;
        }
        else
        {
             next.interactable = true;
        }

        if(index <= 0)
        {
            prev.interactable = false;
        }
        else
        {
             prev.interactable = true;
        }
    }

    public void Next()
    {
        index++;

        for(int i = 0; i < cars.Length; i++)
        {
            cars[i].SetActive(false);
            cars[index].SetActive(true);
        }

        PlayerPrefs.SetInt("carIndex", index);
        PlayerPrefs.Save();
    }

    public void Prev()
    {
        index--;

        for(int i = 0; i < cars.Length; i++)
        {
            cars[i].SetActive(false);
            cars[index].SetActive(true);
        }

        PlayerPrefs.SetInt("carIndex", index);
        PlayerPrefs.Save();
    }

   public void Race()
   {
        PlayerPrefs.SetInt("carIndex", index);
        PlayerPrefs.Save();
        StartCoroutine(LoadGame());
    }


    IEnumerator LoadGame()
    {
        yield return SceneManager.LoadSceneAsync("Senios", LoadSceneMode.Additive);
        

        SceneManager.UnloadSceneAsync("SelectCarScene");
    }

}

//public void Race()
//{
// SceneManager.LoadSceneAsync("Senios", LoadSceneMode.Additive);
// SceneManager.LoadSceneAsync("RaceScene", LoadSceneMode.Additive);

// SceneManager.UnloadSceneAsync("SelectCarScene");
// }