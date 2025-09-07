using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delegates : MonoBehaviour
{
    public delegate void TestDelegate();
    public delegate bool TestDelegateWithParameter(int x);

    private TestDelegate testDelegateFunction;
    private TestDelegateWithParameter testDelegateWithParameterFunction;

    private Action testAction;
    private Action<int, float> testIntFloatAction;
    

    private void Start()
    {
        testDelegateFunction = MytestDelegateFunction;

        //Anonymous Method
        //testDelegateFunction = delegate () { Debug.Log("Anonymous Method"); };
        //Creating Anonymous method using lambda expression
        testDelegateFunction = () => { Debug.Log("Anonymous Method using lambda Expression"); };

        testDelegateFunction();

        testDelegateWithParameterFunction = MytestDelegateWithParameterFunction;

        testDelegateWithParameterFunction = (int x) => { return x < 5; };

        Debug.Log(testDelegateWithParameterFunction(10));

        testIntFloatAction = (int i, float y) => {Debug.Log("Action with Int and Float as Parameter"); };
    }

    void MytestDelegateFunction()
    {
        Debug.Log("MytestDelegateFunction is executed");
    }

    void MySecondDelegateFunction()
    {
        Debug.Log("MySecondDelegateFunction is executed");
    }

    bool MytestDelegateWithParameterFunction(int x)
    {
        return x < 5;
    }
}
