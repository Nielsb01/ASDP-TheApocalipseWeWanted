﻿using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserInterface;

namespace UserInterface.Tests
{
    public class EditorScreenTest
    {

        EditorScreen sut;
        Mock<ConsoleHelper> mockedConsole = new();
        Mock<ScreenHandler> mockedScreen = new();

        [SetUp]
        public void setup()
        {

            mockedScreen.Object.ConsoleHelper = mockedConsole.Object;
            sut = new EditorScreen();
            sut.SetScreen(mockedScreen.Object);
        }

        [Test]
        public void Test_UpdateLastQuestion()
        {
            //Arrange


            //Act
            sut.UpdateLastQuestion(It.IsAny<string>());

            //Assert
            mockedConsole.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void Test_PrintWarning()
        {
            //Arrange


            //Act
            sut.PrintWarning(It.IsAny<string>());

            //Assert
            mockedConsole.Verify(x => x.WriteLine(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void Test_ClearScreen()
        {
            //Arrange


            //Act
            sut.ClearScreen();

            //Assert
            mockedConsole.Verify(x => x.ClearConsole(), Times.Once);
        }

    }
}
