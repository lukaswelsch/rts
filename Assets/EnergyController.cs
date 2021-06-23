using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class EnergyController : MonoBehaviour
{
    // Start is called before the first frame update

    Image energyImage;
    GameObject playerController;

    public float currentEnergy;

    private float oldCurrentEnergy;

    public float energyMax;

    void Start()
    {
        energyImage = GetComponent<Image>();
    }

    public void UpdateUI()
    {
        energyImage.fillAmount = currentEnergy / energyMax;
    }
}
