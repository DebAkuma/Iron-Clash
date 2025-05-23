using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject controlMenu;
    [SerializeField] private GameObject selectionMenu;
    [SerializeField] private GameObject playMenu;
    [SerializeField] private GameObject roomMenu;

    private void Awake()
    {
        DeactivateAllMenus();
        mainMenu.SetActive(true);
    }
    private void DeactivateAllMenus()
    {
        mainMenu.SetActive(false);
        controlMenu.SetActive(false);
        selectionMenu.SetActive(false);
        playMenu.SetActive(false);
        roomMenu.SetActive(false);
    }
    public void PlayButton()
    {
        DeactivateAllMenus();
        playMenu.SetActive(true);
    }
    public void ControlButton()
    {
        DeactivateAllMenus();
        controlMenu.SetActive(true);
    }
    public void QuitButton()
    {
        Debug.Log("Click on quit button");
        Application.Quit();

    }

    public void Controlbackbutton()
    {
        DeactivateAllMenus();
        mainMenu.SetActive(true);
    }
    public void PlayBackButton()
    {
        DeactivateAllMenus();
        mainMenu.SetActive(true);
    }

    public void SinglePlayerButton()
    {
        DeactivateAllMenus();
        selectionMenu.SetActive(true);
        GameManager.isOnline = false;
    }

    public void MultiPlayerButton()
    {
        DeactivateAllMenus();
        roomMenu.SetActive(true);
        GameManager.isOnline = true;
    }

    public void RoomBackButton()
    {
        DeactivateAllMenus();
        GameManager.isOnline = false;
        mainMenu.SetActive(true);
    }

    public void HostButton()
    {

    }

    public void ClientButton()
    {
        
    }
    
}
