﻿using Agent.Exceptions;
using NUnit.Framework;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Agent.Tests
{
    [ExcludeFromCodeCoverage]
    public class FileHandlerTests
    {
        private FileHandler _sut;

        [SetUp]
        public void Setup()
        {
            _sut = new FileHandler();

        }

        [Test]
        public void Test_Import_CorrectFile()
        {
            var expected = "combat when player nearby player then attack";
            var fileLocation = String.Format(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\\..\\..\\"))) + "resource\\import_test_file_1.txt";
            
            var result = _sut.ImportFile(fileLocation);

            Assert.AreEqual(expected, result);
        }
        [Test]
        public void Test_Import_WrongFile()
        { 
            //Method to 
            var fileLocation = String.Format(Path.GetFullPath(Path.Combine
                        (AppDomain.CurrentDomain.BaseDirectory, @"..\\..\\..\\"))) + "resource\\import_test_file_1.php";

            var exception = Assert.Throws<FileException>(() =>
                _sut.ImportFile(fileLocation));

            Assert.AreEqual("File given is not of the correct file type", exception.Message);
        }

        [Test]
        public void Test_ExportFile()
        {
            var expected = "combat=defensive\nexplore=random";
            var fileLocation = String.Format(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\\..\\..\\"))) + "resource\\";
            var fileName = "agentFile.cfg";
            _sut.ExportFile(expected, fileName);

            Assert.AreEqual(expected, _sut.ImportFile(fileLocation + fileName));
        }
    }
}
