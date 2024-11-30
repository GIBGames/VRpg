using UdonSharp;
using UdonToolkit;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using GIB.VRPG2;
using UnityEngine.UI;
using VRC.SDK3.StringLoading;
using VRC.Udon.Common.Interfaces;
using VRC.SDK3.Data;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class SheetModuleBase : UdonSharpBehaviour
{
    DataDictionary CurrentDict;

    public GameObject ValueParent;
    [SerializeField] private SheetDataValue[] sheetDatavalues;

    private void Start()
    {
        sheetDatavalues = ValueParent.GetComponentsInChildren<SheetDataValue>();
        ClearSheet();
    }

    public void ClearSheet()
    {
        foreach (SheetDataValue sdv in sheetDatavalues)
        {
            sdv.ClearValue();
        }
    }

    public void ConvertCharacterSheet(DataDictionary charDict)
    {
        ClearSheet();

        CurrentDict = charDict;

        foreach (SheetDataValue sdv in sheetDatavalues)
        {
            sdv.AssignValue();
        }
    }
}
