using UnityEngine;
using TMPro;
using System;
using System.Linq;
using System.Collections.Generic;

namespace ZeroGame.RHP
{
    public class RuntimeConsole : MonoBehaviour
    {
        internal List<RuntimeConsoleCommand> commandLines = new();
        [SerializeField] private TMP_InputField input;
        [SerializeField] private TMP_Text output;
        //[SerializeField] private Button sendButton;

        private int count = 0;



        private void Start()
        {
            input.onEndEdit.AddListener(OnSent);
            //sendButton.onClick.AddListener(() => OnSent(input.text));

            // Find all commands
            var derivedTypes = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                derivedTypes.AddRange(types.Where(type => type.IsClass
                && !type.IsAbstract
                && type.IsSubclassOf(typeof(RuntimeConsoleCommand))));
            }

            foreach (var type in derivedTypes)
                commandLines.Add((RuntimeConsoleCommand)Activator.CreateInstance(type));
        }


        private void OnSent(string inputText)
        {
            string[] splitText = inputText.Trim().Split(' ');
            string command = splitText[0].ToLower();
            string[] parameters = (splitText.Length > 1) ? splitText.Skip(1).ToArray() : new string[0];
            string outText = "Command couldn't be found!";

            foreach (var commandLine in commandLines)
            {
                if(commandLine.Command.ToLower() == command)
                {
                    outText = commandLine.Execute(parameters); 
                    break;
                }
            }

            output.text = $"[{count++}]: {outText}";
            input.text = "";
        }
    }
}

