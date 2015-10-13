﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CalculationServices;
using System.Windows.Forms; // key

namespace UnitTest_Parser
{
    [TestClass]
    public class UnitTestAlphabet
    {
        [TestMethod]
        public void TestMethod_isOperator()
        {
            Alphabets a = new Alphabets();
            Assert.AreEqual(true, a.isOperator('+'));
            Assert.AreEqual(true, a.isOperator('-'));
            Assert.AreEqual(true, a.isOperator('*'));
            Assert.AreEqual(true, a.isOperator('/'));
        }
        [TestMethod]
        public void TestMethod_isParenthesis()
        {
            Alphabets a = new Alphabets();
            Assert.AreEqual(true, a.isParenthesis('('));
            Assert.AreEqual(true, a.isParenthesis(')'));
        }
        [TestMethod]
        public void TestMethod_isNumber()
        {
            Alphabets a = new Alphabets();
            Assert.AreEqual(true, a.isNumber('0'));
            Assert.AreEqual(true, a.isNumber('1'));
            Assert.AreEqual(true, a.isNumber('2'));
            Assert.AreEqual(true, a.isNumber('3'));
            Assert.AreEqual(true, a.isNumber('4'));
            Assert.AreEqual(true, a.isNumber('5'));
            Assert.AreEqual(true, a.isNumber('6'));
            Assert.AreEqual(true, a.isNumber('7'));
            Assert.AreEqual(true, a.isNumber('8'));
            Assert.AreEqual(true, a.isNumber('9'));
            Assert.AreEqual(false, a.isNumber('a'));
            Assert.AreEqual(false, a.isNumber('b'));
            Assert.AreEqual(false, a.isNumber('c'));
            Assert.AreEqual(false, a.isNumber('d'));
            Assert.AreEqual(false, a.isNumber('e'));
            Assert.AreEqual(false, a.isNumber('f'));
        }
        [TestMethod]
        public void TestMethod_isMinusSign()
        {
            Alphabets a = new Alphabets();
            Assert.AreEqual(true, a.isMinusSign('-'));
            Assert.AreEqual(false, a.isMinusSign('+'));
        }
        [TestMethod]
        public void TestMethod_isEqual()
        {
            Alphabets a = new Alphabets();
            Assert.AreEqual(true, a.isEqual('='));
        }
        [TestMethod]
        public void TestMethod_isBackSpace()
        {
            Alphabets a = new Alphabets();
            Assert.AreEqual(true, a.isBackSpace((char)Keys.Back));
        }
        [TestMethod]
        public void TestMethod_isAlphabet()
        {
            Alphabets a = new Alphabets();
            char[] alphabet = new char[]{
                '0','1','2','3','4','5','6','7','8','9',
                '+','-','*','/',
                '(',')',
            };
            foreach(var v in alphabet)
            {
                Assert.AreEqual(true,a.isAlphabet(v));
            }
            Assert.AreEqual(false, a.isAlphabet('='));
        }

    }
}
