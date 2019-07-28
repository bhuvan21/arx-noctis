/* Manages the stat display HUD */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StatDisplayManager : MonoBehaviour
{
    public GameObject openButton;

    public GameObject display;

    public GameObject baseStats;
    public GameObject addStats;
    public GameObject HP;
    public GameObject MP;
    public GameObject XP;
    public GameObject ID;
    public GameObject DMG;
    public GameObject ELM;
    public GameObject CRIT;
    public GameObject GOLD;
    public GameObject DMD;
    public GameObject RESIST;

    private void Start()
    {
        baseStats = display.transform.Find("BASE").gameObject;
        addStats = display.transform.Find("ADD").gameObject;
        HP = display.transform.Find("HP").gameObject;
        MP = display.transform.Find("MP").gameObject;
        XP = display.transform.Find("XP").gameObject;
        ID = display.transform.Find("ID").gameObject;
        DMG = display.transform.Find("DMG").gameObject;
        ELM = display.transform.Find("ELM").gameObject;
        CRIT = display.transform.Find("CRIT").gameObject;
        GOLD = display.transform.Find("GOLD").gameObject;
        DMD = display.transform.Find("DMD").gameObject;
        RESIST = display.transform.Find("RESISTANCES").gameObject;
        startUpdate();
    }

    // Start is called before the first frame update
    public void startUpdate()
    {
        openButton.transform.Find("Text").gameObject.GetComponent<Text>().text = PlayerPrefs.GetString("playerName") + " L" + gameObject.GetComponent<InventoryManager>().level.ToString();
        closeDisplay();
    }

    public void openDisplay()
    {
        display.SetActive(true);
        updateData();
    }

    public void closeDisplay()
    {
        display.SetActive(false);
    }

    void updateData()
    {
        baseStats.GetComponent<Text>().text = "";
        baseStats.GetComponent<Text>().text += this.gameObject.GetComponent<InventoryManager>().getBaseStat("power").ToString() + "\n";
        baseStats.GetComponent<Text>().text += this.gameObject.GetComponent<InventoryManager>().getBaseStat("endurance").ToString() + "\n";
        baseStats.GetComponent<Text>().text += this.gameObject.GetComponent<InventoryManager>().getBaseStat("wisdom").ToString() + "\n";
        baseStats.GetComponent<Text>().text += this.gameObject.GetComponent<InventoryManager>().getBaseStat("recovery").ToString() + "\n";
        baseStats.GetComponent<Text>().text += this.gameObject.GetComponent<InventoryManager>().getBaseStat("luck").ToString() + "\n";
        baseStats.GetComponent<Text>().text += this.gameObject.GetComponent<InventoryManager>().getBaseStat("immunity").ToString() + "\n";

        addStats.GetComponent<Text>().text = "";
        List<int> addStatList = new List<int>();

        addStatList.Add(this.gameObject.GetComponent<InventoryManager>().getStat("power") - this.gameObject.GetComponent<InventoryManager>().getBaseStat("power"));
        addStatList.Add(this.gameObject.GetComponent<InventoryManager>().getStat("endurance") - this.gameObject.GetComponent<InventoryManager>().getBaseStat("endurance"));
        addStatList.Add(this.gameObject.GetComponent<InventoryManager>().getStat("wisdom") - this.gameObject.GetComponent<InventoryManager>().getBaseStat("wisdom"));
        addStatList.Add(this.gameObject.GetComponent<InventoryManager>().getStat("recovery") - this.gameObject.GetComponent<InventoryManager>().getBaseStat("recovery"));
        addStatList.Add(this.gameObject.GetComponent<InventoryManager>().getStat("luck") - this.gameObject.GetComponent<InventoryManager>().getBaseStat("luck"));
        addStatList.Add(this.gameObject.GetComponent<InventoryManager>().getStat("immunity") - this.gameObject.GetComponent<InventoryManager>().getBaseStat("immunity"));

        foreach(int stat in addStatList)
        {
            string formatted = "";
            if (stat < 0)
            {
                formatted += "-";
            }
            else
            {
                formatted += "+";
            }
            formatted += stat.ToString();
            addStats.GetComponent<Text>().text += formatted + "\n";
        }

        HP.GetComponent<Text>().text = "HP : " + this.gameObject.GetComponent<InventoryManager>().currentHealth.ToString() + "/" + this.gameObject.GetComponent<InventoryManager>().maxHealth.ToString();
        MP.GetComponent<Text>().text = "MP : " + this.gameObject.GetComponent<InventoryManager>().currentMana.ToString() + "/" + this.gameObject.GetComponent<InventoryManager>().maxMana.ToString();
        XP.GetComponent<Text>().text = "XP : " + this.gameObject.GetComponent<InventoryManager>().xp.ToString() + "/" + this.gameObject.GetComponent<InventoryManager>().levelXp.ToString();

        GOLD.GetComponent<Text>().text = "GOLD : " + this.gameObject.GetComponent<InventoryManager>().gold.ToString();
        DMD.GetComponent<Text>().text = "GOLD : " + this.gameObject.GetComponent<InventoryManager>().diamond.ToString();

        ID.GetComponent<Text>().text = "ID : " + PlayerPrefs.GetString("playerID");
        DMG.GetComponent<Text>().text = "DMG\n" + this.gameObject.GetComponent<InventoryManager>().currentWeapon.minimum.ToString() + " - " + this.gameObject.GetComponent<InventoryManager>().currentWeapon.maximum.ToString();
        ELM.GetComponent<Text>().text = "ELEMENT:\n" + this.gameObject.GetComponent<InventoryManager>().currentWeapon.element;
        CRIT.GetComponent<Text>().text = "CRIT\n" + (this.gameObject.GetComponent<InventoryManager>().getStat("luck") / 200.0f * 40.0f).ToString() + "%";

        RESIST.GetComponent<Text>().text = "";
        foreach(Armour.Resistance res in this.gameObject.GetComponent<CoreCharacterController>().resistances)
        {
            RESIST.GetComponent<Text>().text += res.name + " " + res.value.ToString() + "\n";
        }

    }

    public void logout()
    {
        SceneManager.LoadScene("Login");
    }
}
