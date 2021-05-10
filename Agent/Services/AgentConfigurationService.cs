﻿using System;
using Agent.exceptions;
using Agent.Exceptions;
using Serilog;

namespace Agent.Services
{
    public class AgentConfigurationService
    {
        private Pipeline _pipeline;
        private FileHandler _fileHandler;
        private const string CancelCommand = "cancel"; 

        public AgentConfigurationService()
        {
            _pipeline = new Pipeline();
            _fileHandler = new FileHandler();
        }
        
        public void StartConfiguration()
        {
            Console.WriteLine("Please provide a path to your code file");
            var input = Console.ReadLine();

            if (input.Equals(CancelCommand))
            {
                return;
            }

            var content = String.Empty;;
            try
            {
                content = _fileHandler.ImportFile(input);
            }
            catch (FileException e)
            {
                Log.Logger.Information(e.Message);    
                StartConfiguration();
            }
            

            try
            {
                _pipeline.ParseString(content);
                _pipeline.CheckAst();
                var output = _pipeline.GenerateAst();
                _fileHandler.ExportFile(output);
            }
            catch (SyntaxErrorException e)
            {
                Log.Logger.Information("Syntax error: " + e.Message);
                StartConfiguration();
            } 
            catch (SemanticErrorException e)
            {
                Log.Logger.Information("Semantic error: " + e.Message);
                StartConfiguration();
            }
        }
    }
}