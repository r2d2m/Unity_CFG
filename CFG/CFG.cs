﻿/* CONFIG FILE READER
 * READS, PARSES AND STORES
 * DATA FROM CLASSIC .INI FILES
 * V1.1
 * FMLHT, 23.11.2018
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class CFG
{

    static Dictionary<string, bool> cfgBool;
    static Dictionary<string, int> cfgInt;
    static Dictionary<string, float> cfgFloat;
    static Dictionary<string, string> cfgString;
    static List<string> cfgNames;

    public static bool B(string name)
    {
        return cfgBool[name];
    }

    public static int I(string name)
    {
        if (cfgInt.ContainsKey(name))
        {
            return cfgInt[name];
        } else
        {
            return Mathf.RoundToInt(cfgFloat[name]);
        }
    }

    public static float F(string name)
    {
        if (cfgFloat.ContainsKey(name))
        {
            return cfgFloat[name];
        } else
        {
            return (float)cfgInt[name];
        }
    }

    public static string S(string name)
    {
        return cfgString[name];
    }

    public static string[] AS(string name)
    {
        string[] _as = cfgString[name].Split(',');
        for (int i = 0; i < _as.Length; i++)
            _as[i] = _as[i].Trim();
        return _as;
    }

    public static List<string> LS(string name)
    {
        return new List<string>(AS(name));
    }

    public static int[] AI(string name)
    {
        string[] listS = cfgString[name].Split(',');
        int[] listI = new int[listS.Length];
        for (int i = 0; i < listS.Length; i++)
        {
            listI[i] = int.Parse(listS[i]);
        }
        return listI;
    }

    public static List<int> LI(string name)
    {
        return new List<int>(AI(name));
    }

    public static float[] AF(string name)
    {
        string[] listS = cfgString[name].Split(',');
        float[] listI = new float[listS.Length];
        for (int i = 0; i < listS.Length; i++)
        {
            listI[i] = float.Parse(listS[i]);
        }
        return listI;
    }

    public static List<float> LF(string name)
    {
        return new List<float>(AF(name));
    }

    public static Vector2 V2(string name)
    {
        string[] listV = cfgString[name].Split(':');
        return new Vector2(float.Parse(listV[0]), float.Parse(listV[1]));
    }

    public static Vector2Int V2I(string name)
    {
        string[] listV = cfgString[name].Split(':');
        return new Vector2Int(int.Parse(listV[0]), int.Parse(listV[1]));
    }

    public static float V2Rand(string name)
    {
        string[] listV = cfgString[name].Split(':');
        return Random.Range(float.Parse(listV[0]), float.Parse(listV[1]));
    }

    public static int V2RandI(string name)
    {
        string[] listV = cfgString[name].Split(':');
        return Random.Range(int.Parse(listV[0]), int.Parse(listV[1]) + 1);
    }

    public static Vector3 V3(string name)
    {
        float[] lF = AF(name);
        return new Vector3(lF[0], lF[1], lF[2]);
    }

    public static void Clear()
    {
        cfgBool = new Dictionary<string, bool>();
        cfgInt = new Dictionary<string, int>();
        cfgFloat = new Dictionary<string, float>();
        cfgString = new Dictionary<string, string>();
        cfgNames = new List<string>();
    }

    public static void InsertBoolRaw(string name, string value)
    {
        if (value.ToLower() == "true")
        {
            cfgBool[name] = true;
        }
        else
        {
            cfgBool[name] = false;
        }
    }

    public static void InsertIntRaw(string name, string value)
    {
        cfgInt[name] = int.Parse(value);
    }

    public static void InsertFloatRaw(string name, string value)
    {
        cfgFloat[name] = float.Parse(value);
    }

    public static void InsertStringRaw(string name, string value)
    {
        if (value[0] == '\"' || value[0] == '\'')
            value = value.Substring(1, value.Length - 1);
        if (value[value.Length - 1] == '\"' || value[value.Length - 1] == '\'')
            value = value.Substring(0, value.Length - 1);
        cfgString[name] = value;
    }

    public static List<string> FindNameByWord(string word)
    {
        return cfgNames.FindAll((n) => { return n.Contains(word); });
    }

    public static void Load(string file, bool clear = true)
    {
        if (clear)
            Clear();
        FileReader.Read(file, (line) =>
        {
            string key;
            string val;
            string[] lineSrc = line.Split(';');
            lineSrc = lineSrc[0].Split('=');
            if (lineSrc.Length > 1)
            {
                key = lineSrc[0].Trim();
                val = lineSrc[1].Trim();

                string[] val_ = val.Split(new string[] {"//"}, System.StringSplitOptions.None);
                val = val_[0];

                if (clear)
                    cfgNames.Add(key);
                if (val[0] == '\"' || val[0] == '\'')
                {
                    InsertStringRaw(key, val);
                }
                else
                {
                    float _out;
                    if (float.TryParse(val, out _out))
                    {
                        if (val.Contains("."))
                        {
                            InsertFloatRaw(key, val);
                        }
                        else
                        {
                            InsertIntRaw(key, val);
                        }
                    }
                    else
                    {
                        if (val.ToLower() == "true" || val.ToLower() == "false")
                        {
                            InsertBoolRaw(key, val);
                        }
                        else
                        {
                            InsertStringRaw(key, val);
                        }
                    }
                }
            }
        });
    }

    public static void LoadAdditional(string file)
    {
        Load(file, false);
    }

    public static void Write(string path, Dictionary<string, string> data)
    {
        List<string> list = new List<string>();
        foreach (var line in data)
        {
            list.Add(line.Key + " = " + line.Value);
        }
        FileReader.Write(path, list);
    }

}