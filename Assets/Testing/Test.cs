using Packages.Rider.Editor.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITest1 {
    void Beep();
}

public interface ITest2 {
    void Boop();
}

public class TestParent {

}

public class TestChild1 : TestParent, ITest1 {
    public void Beep() { Debug.Log("beep"); }
}

public class TestChild2 : TestParent, ITest2 {
    public void Boop() { Debug.Log("boop"); }
}

public class TestChild3 : TestParent, ITest1, ITest2 {
    public void Beep() { Debug.Log("beep"); }

    public void Boop() { Debug.Log("boop"); }
}

public class Test : MonoBehaviour {
    public void Start() {
        List<TestParent> tests = new List<TestParent>();
        tests.Add(new TestChild1());
        tests.Add(new TestChild2());
        tests.Add(new TestChild3());
        
        foreach (TestParent test in tests) {
            if (test is ITest1 i) {
                i.Beep();
            }
            if (test is ITest2 j) {
                j.Boop();
            }
        }
    }
}