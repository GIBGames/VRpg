/**
 * VRPGUtils.cs by Toast https://github.com/dorktoast - 11/6/2023
 * VRPG Project Repo: https://github.com/GIBGames/VRPG
 * Join the GIB Games discord at https://discord.gg/gibgames
 * Licensed under MIT: https://opensource.org/license/mit/
 */

using UdonSharp;
using UnityEngine;
using VRC.SDK3.Data;

namespace GIB.VRPG2
{
    /// <summary>
    /// Static utilities for VRPG
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public static class Utils
    {
        #region UI utiliites

        public static string MakeColor(string targetText, Color color)
        {
            return "<color=#" + GetColorHex(color) + ">" + targetText + "</color>";
        }

        public static string GetColorHex(Color color)
        {
            int r = Mathf.RoundToInt(color.r * 255.0f);
            int g = Mathf.RoundToInt(color.g * 255.0f);
            int b = Mathf.RoundToInt(color.b * 255.0f);
            return string.Format("{0:X2}{1:X2}{2:X2}", r, g, b);
        }

        public static string GetDots(string targetString, char dot)
        {
            if (int.TryParse(targetString, out int value))
            {
                if (value < 1)
                {
                    return string.Empty;
                }

                string result = "";

                for (int i = 1; i <= value; i++)
                {
                    result += dot;

                    if (i % 5 == 0 && i != value)
                    {
                        result += " ";
                    }
                }

                return result;
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion

        #region Dictionary Utilities
        public static DataDictionary JsonToDictionary(string input)
        {
            if (VRCJson.TryDeserializeFromJson(input, out DataToken json))
            {
                DataDictionary newDict = json.DataDictionary;
                return newDict;
            }
            else
            {
                Debug.Log("[VRPGUtils] Error Deserializing Character Sheet from json!");
                return new DataDictionary();
            }
        }

        public static string DictionaryToJson(DataDictionary dictionary)
        {
            if (VRCJson.TrySerializeToJson(dictionary, JsonExportType.Beautify, out DataToken json))
            {
                // Successfully serialized! We can immediately get the string out of the token and do something with it.
                return json.String;
            }
            else
            {
                // Failed to serialize for some reason, running ToString on the result should tell us why.
                Debug.Log("[VRPGUtils] Failed to Serialize to Json. Result was:" + json.ToString());
                return string.Empty;
            }
        }

        #region Set Data

        public static void SetString(this DataDictionary data, string key, string value)
        {
            if (data.ContainsKey(key))
            {
                data[key] = value;
            }
            else
            {
                data.Add(key, value);
            }
        }

        public static void SetInt(this DataDictionary data, string key, int value)
        {
            if (data.ContainsKey(key))
            {
                data[key] = value;
            }
            else
            {
                data.Add(key, value);
            }
        }

        public static void SetFloat(this DataDictionary data, string key, float value)
        {
            if (data.ContainsKey(key))
            {
                data[key] = value;
            }
            else
            {
                data.Add(key, value);
            }
        }

        public static void SetUInt(this DataDictionary data, string key, uint value)
        {
            if (data.ContainsKey(key))
            {
                data[key] = value;
            }
            else
            {
                data.Add(key, value);
            }
        }

        public static void SetUShort(this DataDictionary data, string key, ushort value)
        {
            if (data.ContainsKey(key))
            {
                data[key] = value;
            }
            else
            {
                data.Add(key, value);
            }
        }

        public static void SetBool(this DataDictionary data, string key, bool value)
        {
            if (data.ContainsKey(key))
            {
                data[key] = value;
            }
            else
            {
                data.Add(key, value);
            }
        }

        #endregion

        #region Get Data

        // Thanks to Miner28 for the extensions these are based off of.

        public static DataToken GetRandom(this DataList list) => list[Random.Range(0, list.Count)];

        public static DataList GetList(this DataDictionary data, DataToken key)
        {
            if (data.TryGetValue(key, out DataToken value))
            {
                if (data[key].TokenType == TokenType.DataList)
                    return data[key].DataList;
            }
            return null;
        }

        public static DataDictionary GetDictionary(this DataDictionary data, DataToken key)
        {
            if (data.TryGetValue(key, out DataToken value))
            {
                if (data[key].TokenType == TokenType.DataDictionary)
                    return data[key].DataDictionary;
            }
            return null;
        }

        public static string GetString(this DataDictionary data, DataToken key, string defaultValue = "")
        {
            if (data.TryGetValue(key, out DataToken value))
            {
                if (data[key].TokenType == TokenType.String)
                    return data[key].String;
                return data[key].ToString();
            }
            else
            {
                Debug.Log($"[VRPG Utils] DataDictionary tried to get integer with key {key}, no value.");
                return defaultValue;
            }
        }

        public static int GetInt(this DataDictionary data, DataToken key, int defaultValue = 0)
        {
            if (data.TryGetValue(key, out DataToken value))
            {
                if (data[key].TokenType == TokenType.Int)
                    return data[key].Int;
                return System.Convert.ToInt32(data[key].Number);
            }
            else
            {
                Debug.Log($"[VRPG Utils] DataDictionary tried to get integer with key {key}, no value.");
                return defaultValue;
            }
        }

        public static uint GetUInt(this DataDictionary data, DataToken key, uint defaultValue = 0)
        {
            if (data.TryGetValue(key, out DataToken value))
            {
                if (data[key].TokenType == TokenType.UInt)
                    return data[key].UInt;
                return System.Convert.ToUInt32(data[key].Number);
            }
            else
            {
                Debug.Log($"[VRPG Utils] DataDictionary tried to get uint with key {key}, no value.");
                return defaultValue;
            }
        }

        public static ushort GetUShort(this DataDictionary data, DataToken key, ushort defaultValue = 0)
        {
            if (data.TryGetValue(key, out DataToken value))
            {
                if (data[key].TokenType == TokenType.UShort)
                    return data[key].UShort;
                return System.Convert.ToUInt16(data[key].Number);
            }
            else
            {
                Debug.Log($"[VRPG Utils] DataDictionary tried to get uShort with key {key}, no value.");
                return defaultValue;
            }
        }

        public static float GetFloat(this DataDictionary data, DataToken key, float defaultValue)
        {
            if (data.TryGetValue(key, out DataToken value))
            {
                if (data[key].TokenType == TokenType.Float)
                    return data[key].Float;
                return (float)data[key].Number;
            }
            else
            {
                Debug.Log($"[VRPG Utils] DataDictionary tried to get Float with key {key}, no value.");
                return defaultValue;
            }
        }

        public static bool GetBool(this DataDictionary data, DataToken key, bool defaultValue = false)
        {
            if (data.TryGetValue(key, out DataToken value))
            {
                if (data[key].TokenType == TokenType.Boolean)
                    return data[key].Boolean;
            }
            else
            {
                Debug.Log($"[VRPG Utils] DataDictionary tried to get Bool with key {key}, bad value.");
            }
            return defaultValue;
        }

        public static DataDictionary GetDictionary(this DataList data, int index)
        {
            return data[index].DataDictionary;
        }

        public static DataList GetList(this DataList data, int index)
        {
            return data[index].DataList;
        }

        public static string GetString(this DataList data, int index)
        {
            return data[index].String;
        }



        public static uint GetUInt(this DataList data, int index)
        {
            if (data[index].TokenType == TokenType.UInt)
                return data[index].UInt;
            return (uint)data[index].Number;
        }

        public static float GetFloat(this DataList data, int index)
        {
            if (data[index].TokenType == TokenType.Float)
                return data[index].Float;
            return (float)data[index].Number;
        }

        public static bool GetBool(this DataList data, int index)
        {
            return data[index].Boolean;
        }

        public static bool GetBool(this DataToken data)
        {
            return data.Boolean;
        }

        public static byte GetByte(this DataList data, int index)
        {
            if (data[index].TokenType == TokenType.Byte)
                return data[index].Byte;
            return (byte)data[index].Number;
        }

        public static byte GetByte(this DataDictionary data, DataToken key)
        {
            if (data[key].TokenType == TokenType.Byte)
                return data[key].Byte;
            return (byte)data[key].Number;
        }

        public static double GetDouble(this DataList data, int index)
        {
            return data[index].Number;
        }

        public static double GetDouble(this DataDictionary data, DataToken key)
        {
            return data[key].Number;
        }

        public static GameObject GetGameObject(this DataDictionary data, DataToken key)
        {
            return (GameObject)data[key].Reference;
        }

        public static GameObject GetGameObject(this DataList data, int index)
        {
            return (GameObject)data[index].Reference;
        }

        public static Vector3 GetVector3(this DataDictionary data, DataToken key)
        {
            return (Vector3)data[key].Reference;
        }

        public static Vector3 GetVector3(this DataList data, int index)
        {
            return (Vector3)data[index].Reference;
        }

        public static Quaternion GetQuaternion(this DataDictionary data, DataToken key)
        {
            return (Quaternion)data[key].Reference;
        }

        public static Quaternion GetQuaternion(this DataList data, int index)
        {
            return (Quaternion)data[index].Reference;
        }

        #endregion

        #endregion
    }
}