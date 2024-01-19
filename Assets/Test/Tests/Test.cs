using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class Test
{
    // A Test behaves as an ordinary method
    [Test]
    public void TestSimplePasses()
    {
        int money = 19;
        int cost_item = 20;


        Assert.AreEqual(money, cost_item, "Если выдаёт ошибку, значит не хватает денег.");
    }

    [Test]
    public void TestCorrect()
    {
        int money = 20;
        int cost_item = 20;

        Assert.AreEqual(money, cost_item, "Если выдаёт ошибку, значит не хватает денег.");
    }
}