using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Dialog : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    public string[] sentences;
    private int index;
    public float typingSpeed;

    private bool PressedMouseButton;
    public bool hasPressedMouseButton;

    public GameObject continueButton;

    private void Awake()
    {
        hasPressedMouseButton = false;
        textDisplay = GameObject.FindGameObjectWithTag("MessageField").GetComponent<TextMeshProUGUI>();
    }
    private void Update()
    {
        if (textDisplay.text == sentences[index])
        {
            //On peut enlever le current message quand il a termin? d'?tre ?crit
            continueButton.SetActive(true);

        }

    }

    public IEnumerator Type()
    {
        //animation de l'?criture
        foreach (char letter in sentences[index].ToCharArray())
        {
            textDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void NextSentence()
    {

        continueButton.SetActive(false);

        if (index < sentences.Length - 1)
        {
            index++;
            textDisplay.text = "";

        }
        else
        {
            textDisplay.text = "";
            continueButton.SetActive(false);

        }
    }
}
