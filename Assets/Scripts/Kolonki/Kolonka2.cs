using System;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
public class Kolonka2 : MonoBehaviour
{
    
    [Header("TMP")]
    [SerializeField] private TMP_Text display;

    [SerializeField] private Door doorToUnlock;

    [Header("Action Buttons")]
    [SerializeField] private XRSimpleInteractable PlusButton;   // Êíîïêà +
    [SerializeField] private XRSimpleInteractable MinusButton; // -

    [Header("Audio")]
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private AudioSource audioSource;

    private bool IsCorrectV = false;
    private string currentInput = "";

    void Start()
    {
        currentInput=display.text;
        PlusButton.selectEntered.AddListener((args) => AddDigit("+"));
        MinusButton.selectEntered.AddListener((args) => AddDigit("-"));
    }
    void AddDigit(string znak)
    {
        if(Convert.ToInt32(currentInput)<10 && znak == "+") { currentInput=Convert.ToString(Convert.ToInt32(currentInput)+1); display.text = currentInput; }
        else if (Convert.ToInt32(currentInput) >0 && znak == "-") { currentInput = Convert.ToString(Convert.ToInt32(currentInput) - 1); display.text = currentInput; }
        if (currentInput == "5") { audioSource.PlayOneShot(audioClip); IsCorrectV = true; }
        else { IsCorrectV = false; }
    }
    void Update()
    {
        doorToUnlock.Point2 = IsCorrectV;
    }
}
