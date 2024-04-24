using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TripoSRStateUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI stateText;

    private void Update()
    {
        stateText.text = TripoSRForUnity.Instance.GetCurrentState().ToString();
    }
}
