using UdonSharp;
using UdonToolkit;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using GIB.VRpg;
public class PatronData : UdonSharpBehaviour
{
    [TextArea] public string PatronHash;
    public string[] patronList;

    [Button("Populate")]
    public void CreatePatronList()
    {
        string[] separatorChar = new string[] { "\r\n" };
        patronList = PatronHash.Split(separatorChar,System.StringSplitOptions.None);
    }

    public bool IsPatron(string target)
    {
        foreach(string s in patronList)
        {
            if (s.ToLower() == target.ToLower())
                return true;
        }
        return false;
    }
}
