using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;



[CreateAssetMenu(fileName = "_", menuName = "Infos/ApplicationState")]
public class AppState : ScriptableObject
{
    [SerializeField]
    public ApplicationState appState;
}
