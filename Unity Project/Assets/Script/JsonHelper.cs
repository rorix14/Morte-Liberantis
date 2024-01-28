using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class JsonHelper
{
    // To serialize and deserialize to and from JSON Arry
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }

    // To serialize and deserialize to and from JSON object form a local json file
    public static T FromJsonObj<T>(string json)
    {
        WrapperObj<T> wrapper = JsonUtility.FromJson<WrapperObj<T>>(json);
        return wrapper.Items;
    }

    public static string ToJsonObj<T>(T classObj, bool prettyPrint)
    {
        WrapperObj<T> wrapper = new WrapperObj<T>();
        wrapper.Items = classObj;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class WrapperObj<T>
    {
        public T Items;
    }
}
