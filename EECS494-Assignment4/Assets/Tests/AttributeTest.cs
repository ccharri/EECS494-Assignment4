using UnityEngine;
using System.Collections;

public class AttributeTest : MonoBehaviour {

    Attribute arr;
    void Start()
    {
        Attribute arr = new Attribute(10, 1.0, 0);
        print("-= operator test (9)");
        print(arr.get());
        arr -= 1;
        print(arr.get());
        print("+= operator test (10)");
        print(arr.get());
        arr += 1;
        print(arr.get());
        print("*= operator test (12)");
        print(arr.get());
        arr *= 0.20;
        print(arr.get());
        print("/= operator test (10)");
        print(arr.get());
        arr /= 0.20;
        print(arr.get());
    }
}
