using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour
{
    public Text inputText;
    public Text outputText;

    public ProcessorComponent processorComponent;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            ProcessInput();
        }
    }

    void ProcessInput()
    {
        string input = inputText.text;
        bool validCommand = false;

        if (input.Contains("clear"))
        {
            outputText.text = "";
        }
        else
        {
            string[] words = input.Split(' ');
            
            // comando start 
            if (words.Length > 0 && words[0] == "start")
            {
                int pParamIndex = -1;
                int quantumParamIndex = -1;
                int quantumTimeParamIndex = -1;
                int wordsCount = words.Length;

                for (int i = 0; i < wordsCount; i++)
                {
                    if (pParamIndex == -1 && words[i] == "-p") pParamIndex = i + 1;
                    else if (quantumParamIndex == -1 && words[i] == "-q") quantumParamIndex = i + 1;
                    else if (quantumTimeParamIndex == -1 && words[i] == "-qt") quantumTimeParamIndex = i + 1;
                }

                if (pParamIndex + quantumParamIndex + quantumTimeParamIndex > 3) // los tres parametros estan definidos
                {
                    validCommand = true;
                    string programsText = words[pParamIndex];
                    bool qpR = float.TryParse(words[quantumParamIndex], out float quantum);
                    bool qtpR = float.TryParse(words[quantumParamIndex], out float quantumTime);
                    
                    if (qpR && qtpR) // argumentos validos
                    {
                        Debug.Log("Todo bien");
                    }
                    else
                        outputText.text += "start argumentos invalidos\n";
                }
        

            }


        }
        if (!validCommand)
            outputText.text += "comando invalido\n";

        inputText.text = "";
    }

}
