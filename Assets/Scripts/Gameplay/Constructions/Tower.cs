﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Tower : Construction
{
    [Header("Attributes")]
    public int damage;
    public float range;
    public float reloadTime;

    [Header("References")]
    public Transform pfProjectile;

    private bool canFire = true;
    private Transform _turret;
    private GameObject _rangeIndicator;

    protected override void Start()
    {
        base.Start();

        _turret = _transform.Find("Turret");
        _rangeIndicator = _transform.Find("Range").gameObject;
        _rangeIndicator.transform.localScale = Vector3.one * range * 2f;
    }

    private void FixedUpdate()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(_transform.position, range, LayerMask.GetMask("Enemy"));

        bool shot = false;
        foreach (Collider2D col in cols)
        {
            if (col && canFire && !col.GetComponent<Enemy>().targeted)
            {
                Shoot(col.transform);
                shot = true;
                break;
            }
        }

        if (!shot && cols.Length > 0 && canFire)
            Shoot(cols[0].transform);
    }

    public override bool Build(Node node)
    {
        if (!base.Build(node))
            return false;

        node.GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = true;
        _rangeIndicator.SetActive(false);

        return true;
    }

    private void Shoot(Transform target)
    {
        target.GetComponent<Enemy>().targeted = true;

        Projectile p = Instantiate(pfProjectile, _turret.position,
                        _turret.rotation, _transform).GetComponent<Projectile>();

        p.damage = damage;
        p.target = target;

        _turret.up = (target.position - _transform.position).normalized;
        _turret.GetComponent<Animator>().SetTrigger("Fire");

        canFire = false;
        Invoke("Reload", reloadTime);
    }

    private void Reload()
    {
        canFire = true;
    }

    private void OnMouseEnter()
    {
        if (built)
            _rangeIndicator.SetActive(true);
    }

    private void OnMouseExit()
    {
        if (built)
            _rangeIndicator.SetActive(false);
    }
}