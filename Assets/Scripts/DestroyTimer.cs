﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    public float timeToDestroy = 2f;

    void Start()
    {
        Destroy(gameObject, timeToDestroy);
    }
}
