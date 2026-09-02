using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    [Header ("Paineis")] 
    public GameObject panelPrincipal;
    public GameObject panelSettings;
    public GameObject panelCreditos;
    
    [Header ("Numero da cena")] 
  
    public int sceneName;
   
    public void Load()
    {
        GameManager.Instance.CarregarCenar(sceneName);
        
    }

    public void EntrarSettings()
    {
        panelPrincipal.SetActive(false);
        panelSettings.SetActive(true);
    }
    
    public void SairSettings()
    {
        panelPrincipal.SetActive(true);
        panelSettings.SetActive(false);
    }
  
    public void EntrarCreditos()
    {
       panelPrincipal.SetActive(false);
       panelCreditos.SetActive(true);
        
    }
   
    public void SairCreditos()
    {
       panelPrincipal.SetActive(true);
       panelCreditos.SetActive(false);
    }
}