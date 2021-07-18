﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grid
{
    public class CellEntity : MonoBehaviour, ICellEntity
    {
        [SerializeField] private EntityType cellId;
        [SerializeField] private Vector3 cellSize;

        public Vector3 CellSize => cellSize;
        public EntityType CellID => cellId;

        public string Name
        {
            get => gameObject.name;
            set => gameObject.name = value;
        }

        public ICellEntity Initialize()
        {
            return this;
        }

        public void SetActive(bool state)
        {
            if (gameObject.activeSelf != state) gameObject.SetActive(state);
        }

        public ICellEntity Orient(Vector3 position)
        {
            transform.position = position;
            return this;
        }

        public ICellEntity Orient(Quaternion rotation)
        {
            transform.rotation = rotation;
            return this;
        }

        public ICellEntity Orient(Vector3 position, Quaternion rotation)
        {
            Orient(position);
            Orient(rotation);
            return this;
        }

        public ICellEntity CreateInstance(Transform parent)
        {
            return Instantiate(this, parent);
        }

        public Orientation GetOrientation()
        {
            return new Orientation(transform);
        }
    }
}