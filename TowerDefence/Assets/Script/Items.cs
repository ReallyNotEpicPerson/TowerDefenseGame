using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu]
public class Items : ScriptableObject
{
    [SerializeField] string id;
    public string ID {get { return id; }}
    public string itemName;
    [Range(1,1000)]
    public int maximumStacks = 1;
    public Sprite icon;

    private void OnValidate()
    {
        string path = AssetDatabase.GetAssetPath(this);
        id = AssetDatabase.AssetPathToGUID(path);
    }
    public virtual Items GetCopy()
    {
        return this;
    }
    public virtual void Destroy(){}
}
