using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class NodeTile : ScriptableObject
{
    public TileBase[] tiles;
    public AudioClip whenPicked;
    public void MakeSound()
    {
        Debug.Log("PutSHit");
    }
}