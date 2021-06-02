﻿using Agent.Exceptions;
using System;
using System.IO;
using System.Linq;

namespace Agent
{
    public class FileHandler
    {
        private string[] _allowedTypes = new[] { ".txt", ".cfg" };

        public virtual string ImportFile(string filepath)
        {
            if (!File.Exists(filepath))
            {
                throw new FileException("File not found!");
            }

            if (!_allowedTypes.Contains(Path.GetExtension(filepath)))
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
            string safeFileLocation = GetBaseDirectory() + "Resource/" + fileName;

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

        public string GetBaseDirectory()
        {
            return string.Format(Path.GetFullPath(Path.Combine(GoBackToRoot(AppDomain.CurrentDomain.BaseDirectory))));
        }

        private string GoBackToRoot(String path)
        {
            return Directory.GetParent
                (Directory.GetParent
                    (Directory.GetParent
                        (Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).ToString()).ToString()).ToString()).ToString();

        }
    }
}