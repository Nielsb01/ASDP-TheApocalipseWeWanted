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
        private String[] allowedTypes = new[] {".txt", ".cfg"};

        public virtual string ImportFile(string filepath)
        {
            if (!File.Exists(filepath))
            {
                throw new FileException("File not found!");
            }

            if (!allowedTypes.Contains(Path.GetExtension(filepath)))
            {
                throw new FileException("The provided file is of an incorrect extension");
            }

            using (FileStream fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    string fileData = reader.ReadToEnd();

                    reader.Close();

                    return fileData;
                }
            }
        }

        public virtual void ExportFile(string content, string fileName)
        {
            string safeFileLocation = String.Format(Path.GetFullPath(Path.Combine
                (AppDomain.CurrentDomain.BaseDirectory, @"..\\..\\..\\"))) + "resource\\" + fileName;

            CreateDirectory(safeFileLocation);

            using (FileStream fileStream = File.Open(safeFileLocation, FileMode.OpenOrCreate))
            {
                using (StreamWriter streamWriter = new StreamWriter(fileStream))
                {
                    streamWriter.Write(content);
                }
            }
        }

        public void CreateDirectory(string filepath)
        {
            string directoryName = Path.GetDirectoryName(filepath);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }
        }
    }
}