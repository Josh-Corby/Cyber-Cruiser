using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Utilities : MonoBehaviour
{
    #region Generic
    /// <summary>
    /// Gets a random colour
    /// </summary>
    /// <returns>A random colour</returns>
    public Color GetRandomColour()
    {
        return new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
    }

    /// <summary>
    /// Shortcut to writing gameObject.SetActive(true)
    /// </summary>
    /// <param name="_go">The gameObject to turn on</param>
    public void TurnOnObject(GameObject _go)
    {
        _go.SetActive(true);
    }

    /// <summary>
    /// Shortcut to writing gameObject.SetActive(false)
    /// </summary>
    /// <param name="_go">The gameObject to turn off</param>
    public void TurnOffObject(GameObject _go)
    {
        _go.SetActive(false);
    }

    /// <summary>
    /// Creates an empty GameObject (container) and add this to it.
    /// In other words, creates a new GameObject level between this and the parent.
    /// </summary>
    /// <returns>The container.</returns>
    /// <param name="name">The container GameObject name</param>
    public GameObject MakeContainer(string name = null)
    {
        GameObject obj = new GameObject();
        obj.transform.SetParent(transform, false);
        obj.transform.position = transform.position;
        obj.name = (!String.IsNullOrEmpty(name) ? name : "container");
        return obj;
    }

    #endregion

    #region Number Related
    /// <summary>
    /// Gets a percentage of a value
    /// </summary>
    /// <param name="_initialValue">The value to get a percentage from</param>
    /// <param name="_percentage">The percentage to take off the main value</param>
    /// <returns>A new value with the percentage taken off the initial value</returns>
    public float GetPercentageValue(float _initialValue, float _percentage)
    {
        float temp = _initialValue / 100f;
        float reverse = 100f - _percentage;
        temp *= reverse;
        temp = temp / 6f;
        return temp;
    }

    /// <summary>
    /// Get the percentage change between the first and second number
    /// </summary>
    /// <param name="roundOne">The first round score</param>
    /// <param name="roundTwo">The second round score</param>
    /// <returns>The percentage change</returns>
    public float PercentageChange(float roundOne, float roundTwo)
    {
        float change = roundTwo - roundOne;
        return change / roundOne * 100;
    }

    public float TrimFloat(float value, int decimals = 2)
    {
        float m = Mathf.Pow(10f, decimals);
        int i = (int)(value * m);
        return ((float)i) / m;
    }

    /// <summary>
    /// Gets the remaining values between two floats
    /// </summary>
    /// <param name="final">The value to get the remainder to</param>
    /// <param name="current">The current value to compare</param>
    /// <returns></returns>
    public float Remainder(float final, float current)
    {
        return final - current;
    }

    /// <summary>
    /// Inverts a value to its negative (or positive) equivelant
    /// </summary>
    /// <param name="_value">The value to invert</param>
    /// <returns>An inverted value</returns>
    public float InvertedValue(float _value)
    {
        return -_value;
    }

    /// <summary>
    /// Checks if we are a multiple of a number
    /// </summary>
    /// <param name="_a">the number to check for</param>
    /// <param name="_b">the number to check against</param>
    /// <returns>true if the numbers are multiples of each other</returns>
    public bool IsMultiple(int _a, int _b)
    {
        return (_a % _b) == 0;
    }

    /// <summary>
    /// Maps a value from one range to another
    /// </summary>
    /// <returns>The mapped value</returns>
    /// <param name="value">The input Value.</param>
    /// <param name="inMin">Input min</param>
    /// <param name="inMax">Input max</param>
    /// <param name="outMin">Output min</param>
    /// <param name="outMax">Output max</param>
    /// <param name="clamp">Clamp output value to outMin..outMax</param>
    public float Map(float value, float inMin, float inMax, float outMin, float outMax, bool clamp = true)
    {
        float f = ((value - inMin) / (inMax - inMin));
        float d = (outMin <= outMax ? (outMax - outMin) : -(outMin - outMax));
        float v = (outMin + d * f);
        if (clamp) v = ClampSmart(v, outMin, outMax);
        return v;
    }
    /// <summary>
    /// Clamps a value, swapping min/max if necessary
    /// </summary>
    /// <returns>The smart.</returns>
    /// <param name="value">The value to clamp</param>
    /// <param name="min">Min output value</param>
    /// <param name="max">Max output value</param>
    public float ClampSmart(float value, float min, float max)
    {
        if (min < max)
            return Mathf.Clamp(value, min, max);
        if (max < min)
            return Mathf.Clamp(value, max, min);
        return value;
    }

    #endregion

    #region Coroutine helpers

    /// <summary>
    /// Executes the Action block as a Coroutine on the next frame.
    /// </summary>
    /// <param name="func">The Action block</param>
    protected void ExecuteNextFrame(Action func)
    {
        StartCoroutine(ExecuteAfterFramesCoroutine(1, func));
    }
    /// <summary>
    /// Executes the Action block as a Coroutine after X frames.
    /// </summary>
    /// <param name="func">The Action block</param>
    protected void ExecuteAfterFrames(int frames, Action func)
    {
        StartCoroutine(ExecuteAfterFramesCoroutine(frames, func));
    }
    private IEnumerator ExecuteAfterFramesCoroutine(int frames, Action func)
    {
        for (int f = 0; f < frames; f++)
            yield return new WaitForEndOfFrame();
        func();
    }

    /// <summary>
    /// Executes the Action block as a Coroutine after X seconds
    /// </summary>
    /// <param name="seconds">Seconds.</param>
    protected void ExecuteAfterSeconds(float seconds, Action func)
    {
        if (seconds <= 0f)
            func();
        else
            StartCoroutine(ExecuteAfterSecondsCoroutine(seconds, func));
    }
    private IEnumerator ExecuteAfterSecondsCoroutine(float seconds, Action func)
    {
        yield return new WaitForSeconds(seconds);
        func();
    }

    /// <summary>
    /// Executes the Action block as a Coroutine whern a condition is met
    /// </summary>
    /// <param name="condition">The Condition block</param>
    /// <param name="func">The Action block</param>
    protected void ExecuteWhenTrue(Func<bool> condition, Action func)
    {
        StartCoroutine(ExecuteWhenTrueCoroutine(condition, func));
    }
    private IEnumerator ExecuteWhenTrueCoroutine(Func<bool> condition, Action func)
    {
        while (condition() == false)
            yield return new WaitForEndOfFrame();
        func();
    }

    #endregion

    #region List/Array related

    /// <summary>
    /// Toggles active or inactive all GameObjects in a list
    /// </summary>
    /// <param name="gos">The list of objects to toggle</param>
    /// <param name="state">The state to toggle the objects (true/false)</param>
    public void ToggleObjects(List<GameObject> gos, bool state)
    {
        for (int i = 0; i < gos.Count; i++)
            gos[i].SetActive(state);
    }

    /// <summary>
    /// Toggles active or inactive all GameObjects in an array
    /// </summary>
    /// <param name="gos">The array of objects to toggle</param>
    /// /// <param name="state">The state to toggle the objects (true/false)</param>
    public void ToggleObjects(GameObject[] gos, bool state)
    {
        for (int i = 0; i < gos.Length; i++)
            gos[i].SetActive(state);
    }

    /// <summary>
    /// Finds an element in a string list that matches the _toFind parameter
    /// </summary>
    /// <param name="_list">The list to search through</param>
    /// <param name="_toFind">The string to search for</param>
    /// <returns>Returns an element that contains the string to be found, else return empty string</returns>
    public string FindElementInList(List<string> _list, string _toFind)
    {
        if (_list.Find(x => x.Contains(_toFind)) != null)
            return _list.Find(x => x.Contains(_toFind));
        else
            return "";
    }

    public GameObject FindChildGameObjectByName(string gameObjectName)
    {
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (gameObject.transform.GetChild(i).name == gameObjectName)
            {
                return gameObject.transform.GetChild(i).gameObject;
            }
        }
        return null;

    }

    /// <summary>
    /// Shuffles a list using Unity's Random
    /// </summary>
    /// <typeparam name="T">The data type</typeparam>
    /// <param name="_list">The list to shuffle</param>
    /// <returns></returns>
    public static List<T> ShuffleList<T>(List<T> _list)
    {
        for (int i = 0; i < _list.Count; i++)
        {
            T temp = _list[i];
            int randomIndex = UnityEngine.Random.Range(i, _list.Count);
            _list[i] = _list[randomIndex];
            _list[randomIndex] = temp;
        }
        return _list;
    }

    /// <summary>
    /// Resizes a list to a specific size
    /// </summary>
    /// <typeparam name="T">The list to resize</typeparam>
    /// <param name="list">The list to resize</param>
    /// <param name="size">How big the list should be</param>
    /// <param name="element"></param>
    /// <returns></returns>
    public static List<T> ResizeList<T>(List<T> list, int size, T element = default(T))
    {
        int count = list.Count;

        if (size < count)
        {
            list.RemoveRange(size, count - size);
        }
        else if (size > count)
        {
            if (size > list.Capacity)   // Optimization
                list.Capacity = size;

            list.AddRange(Enumerable.Repeat(element, size - count));
        }
        return list;
    }

    /// <summary>
    /// Clear a list. If it is a list of gameobjects, destroy the objects
    /// </summary>
    /// <typeparam name="T">The type of he list</typeparam>
    /// <param name="list">List to clear</param>
    public void ClearList<T>(List<T> list)
    {
        if (list.Count > 0)
        {
            for (int i = list.Count - 1; i >= 0; i--)
            {
                T objectToRemove = list[i];
                if (objectToRemove != null)
                {
                    if(objectToRemove is GameObject)
                    {
                        GameObject GO = objectToRemove as GameObject;
                        Destroy(GO);
                    }
                }
            }
        }
        list.Clear();
    }
    #endregion

    #region Enums

    /// <summary>
    /// Adds space between words in an enum
    /// </summary>
    /// <param name="_enum">The enum as a string</param>
    /// <returns>Formatted enum with spaces</returns>
    public string EnumNameFormatted(string _enum)
    {
        return string.Concat(_enum.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
    }

    public static string GetEnumNameByValue(Type enumType, int value)
    {
        foreach (var v in Enum.GetValues(enumType))
            if ((int)v == value)
                return v.ToString();
        return null;
    }
    public static int GetEnumValueByName(Type enumType, string name)
    {
        foreach (var v in Enum.GetValues(enumType))
            if (v.ToString().Equals(name))
                return (int)v;
        return -999999;
    }
    public static string GetEnumValueFromString(Type enumType, string name)
    {
        foreach (var v in Enum.GetValues(enumType))
            if (v.ToString().Equals(name))
                return v.ToString();
        return null;
    }

    #endregion

    #region Fading Elements

    float fadeTime = 0.4f;

    /// <summary>
    /// Fades in a Canvas Group
    /// </summary>
    /// <param name="_cvg">The Canvas Group to fade</param>
    public void FadeInCanvas(CanvasGroup _cvg)
    {
        _cvg.DOFade(1, 1f);
        _cvg.interactable = true;
        _cvg.blocksRaycasts = true;
    }

    /// <summary>
    /// Fades out a Canvas Group
    /// </summary>
    /// <param name="_cvg">The Canvas Group to fade</param>
    public void FadeOutCanvas(CanvasGroup _cvg)
    {
        _cvg.DOFade(0, 1f);
        _cvg.interactable = false;
        _cvg.blocksRaycasts = false;
    }

    public void InstantTransparent(CanvasGroup cvs)
    {
        cvs.alpha = 0;
        cvs.interactable = false;
        cvs.blocksRaycasts = false;
    }

    public void InstantOpaque(CanvasGroup cvs)
    {
        cvs.alpha = 1;
        cvs.interactable = true;
        cvs.blocksRaycasts = true;
    }

    public void InstantOpaque(TMP_Text _text)
    {
        _text.alpha = 1;
    }

    public void InstantTransparent(TMP_Text _text)
    {
        _text.alpha = 0;
    }

    public void InstantOpaque(Image _image)
    {
        _image.DOFade(1, 0);
    }

    public void InstantTransparent(Image _image)
    {
        _image.DOFade(0, 0);
    }


    /// <summary>
    /// Fades and element to a specific value
    /// </summary>
    /// <param name="_image">The image to fade</param>
    /// <param name="_fadeValue">The value to fade to</param>
    public void FadeElement(Image _image, float _fadeValue)
    {
        _image.DOFade(_fadeValue, fadeTime);
    }

    /// <summary>
    /// Fades in an image
    /// </summary>
    /// <param name="_image">The image to fade in</param>
    public void FadeInElement(Image _image)
    {
        _image.DOFade(1, fadeTime);
    }

    /// <summary>
    /// Fades out an image
    /// </summary>
    /// <param name="_image">The image to fade out</param>
    public void FadeOutElement(Image _image)
    {
        _image.DOFade(0, fadeTime);
    }

    /// <summary>
    /// Fades in an image
    /// </summary>
    /// <param name="_text">The image to fade in</param>
    public void FadeInElement(TMP_Text _text)
    {
        _text.DOFade(1, fadeTime);
    }

    /// <summary>
    /// Fades out an image
    /// </summary>
    /// <param name="_text">The image to fade out</param>
    public void FadeOutElement(TMP_Text _text)
    {
        _text.DOFade(0, fadeTime);
    }
    #endregion

    #region String Formatters
    /// <summary>
    /// Formats an integer with leading spaces
    /// </summary>
    /// <returns>The formatted string</returns>
    /// <param name="value">Value.</param>
    /// <param name="length">Min length.</param>
    public string FormatIntSpaces(int value, int length)
    {
        return String.Format("{0," + length + "}", value);
    }
    /// <summary>
    /// Formats an integer with leading Zeroes
    /// </summary>
    /// <returns>The formatted string</returns>
    /// <param name="value">Value.</param>
    /// <param name="length">Min length.</param>
    public string FormatIntZeroes(int value, int length)
    {
        return value.ToString("D" + length);
    }
    /// <summary>
    /// Formats a float with a specified number of decimals
    /// </summary>
    /// <returns>The formatted string</returns>
    /// <param name="value">The Value.</param>
    /// <param name="decimals">Max Number of decimals.</param>
    /// <param name="decimals">If flexible, will drop trailing zeroes</param>
    public string FormatFloat(float number, int decimals = 2, bool flex = false)
    {
        if (decimals <= 0) return ((int)number).ToString();
        string fmt = FormatDecimalsFormat(decimals, flex);
        return String.Format(fmt, number);
    }
    /// <summary>
    /// Formats a double with a specified number of decimals
    /// </summary>
    /// <returns>The formatted string</returns>
    /// <param name="value">The Value.</param>
    /// <param name="decimals">Max Number of decimals.</param>
    /// <param name="decimals">If flexible, will drop trailing zeroes</param>
    public string FormatDouble(double number, int decimals, bool flex = false)
    {
        if (decimals <= 0) return ((long)number).ToString();
        string fmt = FormatDecimalsFormat(decimals, flex);
        return String.Format(fmt, number);
    }
    public string FormatDecimalsFormat(int decimals, bool flex = true)
    {
        string fmt = "{0:#,0.";
        for (int i = 0; i < decimals; i++)
            fmt += (flex && i > 0) ? "#" : "0";
        fmt += "}";
        return fmt;
    }
    #endregion

    #region Time/Date
    /// <summary>
    /// Formats a float value a a time string like mm:ss or h:mm:ss
    /// </summary>
    /// <returns>The formatted string</returns>
    /// <param name="value">The time in seconds</param>
    /// <param name="millis">Formats second decimals (h:mm:ss.dd)</param>
    public string FormatTime(float value, bool decimals = false)
    {
        string result = "";
        float v = value;
        if (v < 0f)
        {
            v = -v;
            result = "-";
        }
        v = Mathf.FloorToInt(v);
        int secs = Mathf.FloorToInt(v % 60f);
        int mins = Mathf.FloorToInt((v % 3600f) / 60f);
        int hours = Mathf.FloorToInt(v / 3600f);
        if (hours > 0)
            result += hours.ToString() + ":" + FormatIntZeroes(mins, 2);
        else
            result += mins.ToString();
        result += ":" + FormatIntZeroes(secs, 2);
        if (decimals)
            result += "." + FormatIntZeroes(Mathf.FloorToInt((Mathf.Abs(value) - v) * 100f), 2);
        return result;
    }
    /// <summary>
    /// Un-Format a time string likeh:mm:ss into float
    /// </summary>
    /// <returns>The formatted time string</returns>
    /// <param name="value">The time in seconds, or -1 if invlaid string</param>
    public float UnFormatTime(string value)
    {
        if (string.IsNullOrEmpty(value))
            return 0f;
        bool neg = (value[0] == '-');
        if (neg)
            value = value.Substring(1);
        float result = 0f;
        // Get decimals
        string[] parts = value.Split('.');
        if (parts.Length == 2f)
        {
            value = parts[0];
            if (!float.TryParse("0." + parts[1], out result))
                return -1f;
        }
        else if (parts.Length == 1)
            value = parts[0];
        else
            return -1f;
        // GEt hh:mm:ss
        int hours = 0;
        int minutes = 0;
        int seconds = 0;
        parts = value.Split(':');
        int partCount = parts.Length;
        if (partCount > 0)
            if (!Int32.TryParse(parts[--partCount], out seconds))
                return -1f;
        if (partCount > 0)
            if (!Int32.TryParse(parts[--partCount], out minutes))
                return -1f;
        if (partCount > 0)
            if (!Int32.TryParse(parts[--partCount], out hours))
                return -1f;
        result += (hours * 60f * 60f) + (minutes * 60f) + seconds;
        return neg ? -result : result;
    }


    public string GetTodaysDate()
    {
        return DateTime.Now.ToLongDateString();
    }

    public int GetCurrentYear()
    {
        return DateTime.Now.Year;
    }

    public string GetDayWithSuffix(int day)
    {
        switch (day)
        {
            case 1:
            case 21:
            case 31:
                return day.ToString() + "st";
            case 2:
            case 22:
                return day.ToString() + "nd";
            case 3:
            case 23:
                return day.ToString() + "rd";
            default:
                return day.ToString() + "th";
        }
    }

    public string GetMonth(int month)
    {
        switch (month)
        {
            case 1: return "January";
            case 2: return "February";
            case 3: return "March";
            case 4: return "April";
            case 5: return "May";
            case 6: return "June";
            case 7: return "July";
            case 8: return "August";
            case 9: return "September";
            case 10: return "October";
            case 11: return "November";
            case 12: return "December";
            default: return "January";
        }
    }

    #endregion

    /// <summary>
    /// Check if weights of a spawner exceed 1. If so, correct them
    /// </summary>
    /// <typeparam name="T"></typeparam>`
    /// <param name="structArray"> Array of structs</param>
    /// <param name="structType">The struct type of the array </param>
    /// <param name="fieldName">The name of a float field in the struct type</param>
    /// <param name="totalWeight">The total weight to be outputted for the inspector</param>
    public float ValidateSpawnRates<T>(T[] structArray, Type structType, string fieldName, float totalWeight) where T : struct
    {
        //Reset total weight
        totalWeight = 0;
        //Create a new list to store weights
        var valueList = new List<float>();

        foreach (object _struct in structArray)
        {
            //Check if any structs in the array aren't of the correct type;
            if (_struct.GetType() != structType)
            {
                continue;
            }

            //Use reflection to get the desired float from the struct
            FieldInfo field = structType.GetField(fieldName);
            if (field != null && field.FieldType == typeof(float))
            {
                object boxedValue = field.GetValue(_struct);
                float value = (float)boxedValue;
                //add the float to the list for easier use
                valueList.Add(value);
            }
        }

        //get the total weight
        for (int i = 0; i < valueList.Count; i++)
        {
            totalWeight += valueList[i];
        }

        //if total weight exceeds 1, divide values by 1/total weight
        if (totalWeight > 1)
        {
            float factor = 1 / totalWeight;
            for (int i = 0; i < valueList.Count; i++)
            {
                valueList[i] *= factor;
            }
        }

        //round floats to desired decimal place
        for (int i = 0; i < valueList.Count; i++)
        {
            valueList[i] = (float)Math.Round(valueList[i], 3);
        }

        totalWeight = 0;
        //Check total weight again
        for (int i = 0; i < valueList.Count; i++)
        {
            totalWeight += valueList[i];
        }

        //Assign changed values back to structs
        int x = 0;
        foreach (object _struct in structArray)
        {
            if (_struct.GetType() != structType)
            {
                continue;
            }

            FieldInfo field = structType.GetField(fieldName);
            if (field != null && field.FieldType == typeof(float))
            {
                float value = (float)field.GetValue(_struct);
                value = valueList[x];

                field.SetValue(_struct, value);
                structArray[x] = (T)_struct;
                x++;
            }
        }
        return totalWeight;
    }

    public void EnableSlider(UISlider slider, float maxValue)
    {
        slider.gameObject.SetActive(true);
        slider.SetSliderMaxValue(maxValue);
    }

    public void EnableAndSetSlider(UISlider slider, float currentValue, int maxValue)
    {
        slider.gameObject.SetActive(true);
        slider.SetSliderValues(currentValue, maxValue);
    }

    public void DisableSlider(UISlider slider)
    {
        slider.gameObject.SetActive(false);
    }

    public void ChangeSliderValue(UISlider slider, float value)
    {
        if (slider.gameObject.activeSelf)
        {
            slider.SetSliderValue(value);
        }
    }
}