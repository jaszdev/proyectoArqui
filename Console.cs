using UnityEngine;
using UnityEngine.UI;

public class Console : MonoBehaviour
{
    public InputField inputField;
    public Text outputText;

    public ProcessorComponent processorComponent;
    
    void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            ProcessInput();
        }
    }

    void ProcessInput()
    {
        string input = inputField.text;
        string[] words = input.Split(' ');
        //Debug.Log("input: " + input);
        bool validCommand = false;

        // comando clear
        if (input.Contains("clear"))
        {
            validCommand = true;
            outputText.text = "";
        }
        // comando exists: revisa si archivo existe
        else if (words.Length == 2 && (words[0] == "exists" || words[0] == "e"))
        {
            validCommand = true;
            bool exists = System.IO.File.Exists(words[1]);
            outputText.text += exists ? "archivo existe\n" : "archivo no existe\n";
        }
        // comando esc: comando para salir de la aplicacion
        else if (words.Length == 1 && words[0] == "esc")
        {
            validCommand = true;
            Application.Quit();
        }
        // comando mem
        else if (words.Length == 2 && words[0] == "mem")
        {
            validCommand = true;
            int dir = TBL.DataDirToIndex(int.Parse(words[1]));
            outputText.text += processorComponent.processor.Memory.GetData(dir);
        }
        // comando abort 
        else if (words.Length == 1 && words[0] == "abort")
        {
            validCommand = true;
            outputText.text += "proceso terminado\n";
            processorComponent.Abort();
        }
        // comando results: para escribir en un txt los resultados de la ejecucion
        else if (words.Length == 1 && (words[0] == "r" || words[0] == "results"))
        {
            validCommand = true;
            processorComponent.processor.WriteResults("resultados.txt");
        }
        // comando debug
        else if (words.Length == 1 && (words[0] == "d" || words[0] == "debug"))
        {
            processorComponent.debug = !processorComponent.debug;
        }
        else if (words.Length > 0 && (words[0] == "start" || words[0] == "s"))
        {
            validCommand = true;

            if (words.Length != 7)
            {
                outputText.text += "formato de start no soportado\n";
            }
            else
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
                    string programsNameFileName = words[pParamIndex];
                    bool qpR = int.TryParse(words[quantumParamIndex], out int quantum);
                    bool qtpR = float.TryParse(words[quantumTimeParamIndex], out float quantumTime);

                    if (qpR && qtpR) // argumentos validos
                    {
                        if (System.IO.File.Exists(programsNameFileName))
                        {
                            string[] programNames = System.IO.File.ReadAllLines(programsNameFileName);
                            processorComponent.StartProcessor(programNames, quantum, quantumTime);
                            outputText.text += "archivo: " + programsNameFileName +
                                "\nquantum: " + quantum +
                                "\ntiempo quantum: " + quantumTime + "\n";
                        }
                        else
                        {
                            outputText.text += "archivo no existe\n";
                        }


                    }
                    else
                    {
                        outputText.text += "start argumentos invalidos\n";
                    }
                }
            }

        }

        if (!validCommand)
            outputText.text += "comando invalido\n";

        inputField.text = "";
        inputField.Select();
    }

}
