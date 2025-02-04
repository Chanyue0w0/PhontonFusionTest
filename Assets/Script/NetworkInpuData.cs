using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;


public enum InputButtons
{
    JUMP,
    FIRE
}

public struct NetworkInputData : INetworkInput
{
    public Vector3 movementInput;
    public NetworkButtons buttons;
}