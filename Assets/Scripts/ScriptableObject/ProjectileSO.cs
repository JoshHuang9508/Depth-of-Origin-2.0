using Inventory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ProjectileSO : ItemSO, IDestoryable, ISellable, IBuyable, IDroppable
{
    [Header("Attributes")]
    public float speed = 100f;
    public float range = 10f;
}