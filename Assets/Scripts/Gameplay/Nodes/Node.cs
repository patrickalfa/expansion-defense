﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Node : MonoBehaviour
{   
    [Header("Attributes")]
    public int x, y;
    public bool built;
    public bool occupied;
    public int cost;

    [Header("Control variables")]
    protected bool over;

    [Header("References")]
    public Sprite groundBuilt;
    public GameObject _contentBuilt;

    protected Transform _transform;
    protected SpriteRenderer _sprite;
    protected SpriteRenderer _content;
    protected Transform _outline;
    protected Transform _overlay;

    protected virtual void Start()
    {
        _transform = transform;
        _sprite = GetComponent<SpriteRenderer>();
        _outline = _transform.Find("Outline");
        _overlay = _transform.Find("Overlay");

        if (_transform.Find("Content"))
            _content = _transform.Find("Content").GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!over)
            UpdateSortingOrder(Mathf.RoundToInt(transform.position.y * 100f) * -1);
    }

    private void OnMouseEnter()
    {
        over = true;

        if (GameManager.instance.stage == GAME_STAGE.EXPANSION || occupied)
        {
            UpdateSortingOrder(999);

            if (IsAdjacent() && !built)
                _overlay.gameObject.SetActive(true);

            _transform.DOKill();
            _transform.DOPunchScale(Vector3.one * .1f, .25f).OnComplete(() =>
            {
                _transform.localScale = Vector3.one;
                UpdateSortingOrder(Mathf.RoundToInt(transform.position.y * 100f) * -1);
            });
        }
        if (GameManager.instance.stage == GAME_STAGE.CONSTRUCTION)
        {
            if (!GameManager.instance._construction)
                return;

            GameManager.instance._construction.gameObject.SetActive(built && !occupied);
            GameManager.instance._construction.transform.position = _transform.position;
            GameManager.instance._construction.SetSortingOrder(_sprite.sortingOrder + 102);
        }
    }

    private void OnMouseExit()
    {
        over = false;

        _overlay.gameObject.SetActive(false);

        _transform.DOKill();
        _transform.localScale = Vector3.one;
    }

    private void OnMouseUp()
    {
        if (GameManager.instance.state == GAME_STATE.DRAGGING)
            return;

        if (GameManager.instance.stage == GAME_STAGE.EXPANSION)
        {
            if (!built)
                Build();
        }
        else if (GameManager.instance.stage == GAME_STAGE.CONSTRUCTION)
        {
            if (built && !occupied)
                GameManager.instance.Construct(this);
        }
    }

    protected virtual bool Build()
    {
        if (!CanBuild())
            return false;

        built = true;
        GameManager.instance.wood -= cost;

        _sprite.sprite = groundBuilt;

        if (_content)
            _content.gameObject.SetActive(false);
        if (_contentBuilt)
        {
            _contentBuilt.SetActive(true);
            _content = _contentBuilt.GetComponent<SpriteRenderer>();
        }

        SetColor(Color.white);

        UpdateOutlines();
        UpdateSortingOrder(999);
        _overlay.gameObject.SetActive(false);

        _transform.DOKill();
        _transform.DOPunchScale(Vector3.one * .5f, .25f).OnComplete(() =>
        {
            _transform.localScale = Vector3.one;
            UpdateSortingOrder(Mathf.RoundToInt(transform.position.y * 100f) * -1);
        });

        return true;
    }

    private bool CanBuild()
    {
        return (IsAdjacent() && IsAffordable() && !built);
    }

    private bool IsAffordable()
    {
        return (GameManager.instance.wood >= cost);
    }

    private bool IsAdjacent()
    {
        Node node;

        node = Generator.instance.GetNode(x - 1, y);
        bool l = (node && node.built);
        node = Generator.instance.GetNode(x + 1, y);
        bool r = (node && node.built);
        node = Generator.instance.GetNode(x, y - 1);
        bool u = (node && node.built);
        node = Generator.instance.GetNode(x, y + 1);
        bool d = (node && node.built);

        return (l || r || u || d);
    }
    
    private void UpdateSortingOrder(int sortingOrder)
    {
        _sprite.sortingOrder = sortingOrder;

        if (_content)
            _content.sortingOrder = sortingOrder + 102;

        SpriteRenderer[] outlines = _outline.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer o in outlines)
            o.sortingOrder = sortingOrder + 1;
    }

    private void UpdateOutlines()
    {
        _outline.gameObject.SetActive(true);

        UpdateNodeOutline();

        Node node;
        node = Generator.instance.GetNode(x - 1, y);
        if (node) node.UpdateNodeOutline();
        node = Generator.instance.GetNode(x + 1, y);
        if (node) node.UpdateNodeOutline();
        node = Generator.instance.GetNode(x, y - 1);
        if (node) node.UpdateNodeOutline();
        node = Generator.instance.GetNode(x, y + 1);
        if (node) node.UpdateNodeOutline();
    }

    public void UpdateNodeOutline()
    {
        Node node;
        node = Generator.instance.GetNode(x - 1, y);
        bool l = node && node.built;
        node = Generator.instance.GetNode(x + 1, y);
        bool r = node && node.built;
        node = Generator.instance.GetNode(x, y - 1);
        bool u = node && node.built;
        node = Generator.instance.GetNode(x, y + 1);
        bool d = node && node.built;

        _outline.transform.Find("Left").gameObject.SetActive(!l);
        _outline.transform.Find("Right").gameObject.SetActive(!r);
        _outline.transform.Find("Up").gameObject.SetActive(!u);
        _outline.transform.Find("Down").gameObject.SetActive(!d);
    }

    public void SetColor(Color color)
    {
        _sprite.color = color;
        if (_content)
            _content.color = color;
    }
}
