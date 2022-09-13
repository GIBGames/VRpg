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

    private void Start()
    {
        CreatePatronList();
    }

    [Button("Populate")]
    public void CreatePatronList()
    {
        patronList = PatronHash.Split('\n');
    }

    public bool isPatron(string target)
    {
        foreach(string s in patronList)
        {
            if (s.ToLower() == target.ToLower())
                return true;
        }
        return false;
    }
}
