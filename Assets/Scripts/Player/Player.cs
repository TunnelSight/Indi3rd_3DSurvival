using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerController controller;
    public PlayerCondition condition;
    public Equipment equip;

    public ItemData itemData;
    public Action addItem;

    public Transform dropPosition;

    private void Awake()
    {
        CharacterManager.Instance.Player = this;
        controller = GetComponent<PlayerController>(); //같은 컴포넌트 안에서 저걸 찾아서 넣어주는 기능
        condition = GetComponent<PlayerCondition>(); //같은 컴포넌트 안에서 저걸 찾아서 넣어주는 기능
        equip = GetComponent<Equipment>();
    }
}
