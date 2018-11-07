﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Construction : MonoBehaviour
{
    public bool built;
    public int cost;

    protected Transform _transform;
    protected SpriteRenderer _sprite;

    protected Node node;

    protected virtual void Start()
    {
        _transform = transform;
        _sprite = GetComponent<SpriteRenderer>();
    }

    public virtual bool Build(Node node)
    {
        if (GameManager.instance.gold < cost)
            return false;

        built = true;
        node.occupied = true;

        this.node = node;

        GameManager.instance.gold -= cost;
        _sprite.color = Color.white;

        SetSortingOrder(node.GetComponent<SpriteRenderer>().sortingOrder + 102);

        return true;
    }

    public virtual void SetSortingOrder(int sortingOrder)
    {
        if (_sprite)
            _sprite.sortingOrder = sortingOrder;
    }
}
