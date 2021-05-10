﻿using Agent.Exceptions;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agent
{
    public class FileHandler
    {
        public string ImportFile(string filepath)
        {
            try
            {
                if (Path.GetExtension(filepath) == ".txt" || Path.GetExtension(filepath) == ".cfg")
                {
                    using (FileStream fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
                    {
                        using (StreamReader reader = new StreamReader(fileStream))
                        {
                            string fileData = reader.ReadToEnd();

                            reader.Close();

                            return fileData;
                        };
                    }
                }
                else 
                {
                    throw new FileException("File given is not of the correct file type");
                }
            }
            catch (FileException e)
            {
                Log.Logger.Information(e.Message);
                //idk functie breekt anders
                return null;
            }
            
        }

        public void ExportFile(string content)
        {
            //TODO change file name to selected by user
            string tmpFileName = "agentFile.cfg";
            string safeFileLocation = String.Format(Path.GetFullPath(Path.Combine
                        (AppDomain.CurrentDomain.BaseDirectory, @"..\\..\\..\\"))) + "resource\\" + tmpFileName;

            try
            {
                CreateDirectory(safeFileLocation);

                using (FileStream fileStream = new FileStream(safeFileLocation, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter streamWriter = new StreamWriter(fileStream))
                    {
                        streamWriter.Write(content);
                    };   
                };
            }
            catch (FileException)
            {
                throw;
            }
        }
        public void CreateDirectory(string filepath)
        {
            string directoryName = Path.GetDirectoryName(filepath);

            if (directoryName.Length > 0)
            {
                Directory.CreateDirectory(directoryName);
            }

            if (!File.Exists(filepath))
            {
                File.Create(filepath);
            }
        }
    }
}


