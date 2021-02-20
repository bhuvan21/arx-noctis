/* Manages the inventory, and also stats for some reason*/
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public GameObject Healthbar;
    public GameObject Manabar;

    public Weapon defaultWeapon;
    public Weapon currentWeapon;
    public GameObject LHWeaponSlot;
    public GameObject RHWeaponSlot;
    GameObject LHWeaponGameobject;
    GameObject RHWeaponGameobject;

    public GameObject inventoryPanel;
    GameObject content;
    GameObject weaponName;
    GameObject weaponCategory;
    GameObject weaponDescription;
    GameObject weaponStats;
    GameObject weaponElements;
    GameObject weaponPreview;

    List<Item> inventory = new List<Item>();

    GameObject selectedButton;
    Item selectedItem;

    public GameObject inventoryButton;
    public GameObject shopButton;
    int tab = 0;
    public GameObject equipButton;
    public GameObject creditButton;

    public List<Armour.Stat> currentStats = new List<Armour.Stat>
    {
        new Armour.Stat("power", 0),
        new Armour.Stat("immunity", 600),
        new Armour.Stat("endurance", 0),
        new Armour.Stat("wisdom", 0),
        new Armour.Stat("luck", 0),
        new Armour.Stat("recovery", 0)
    };

    public List<Armour.Stat> baseStats = new List<Armour.Stat>
    {
        new Armour.Stat("power", 0),
        new Armour.Stat("immunity", 600),
        new Armour.Stat("endurance", 0),
        new Armour.Stat("wisdom", 0),
        new Armour.Stat("luck", 0),
        new Armour.Stat("recovery", 0)
    };


    public int maxHealth = 200;
    public int currentHealth = 200;

    public int maxMana = 200;
    public int currentMana = 200;

    public int level = 1;
    public int xp;
    public int levelXp = 100;
    public int gold=900;
    public int diamond;

    bool shop = false;

    public List<StatusEffect> pEffects = new List<StatusEffect>();
    public List<StatusEffect> eEffects = new List<StatusEffect>();

    // Start is called before the first frame update
    void Start()
    {
        int i = 0;
        foreach (string value in PlayerPrefs.GetString("playerStats").Split('/'))
        {
            baseStats[i] = new Armour.Stat(baseStats[i].name, int.Parse(value));
            i++;
        }
        
        updateStats();

        content = inventoryPanel.transform.Find("Items").transform.Find("Viewport").transform.Find("Content").gameObject;
        weaponName = inventoryPanel.transform.Find("Display").transform.Find("WeaponName").gameObject;
        weaponCategory = inventoryPanel.transform.Find("Display").transform.Find("WeaponCategory").gameObject;
        weaponDescription = inventoryPanel.transform.Find("Display").transform.Find("Description").gameObject;
        weaponStats = inventoryPanel.transform.Find("Display").transform.Find("Stats").gameObject;
        weaponElements = inventoryPanel.transform.Find("Display").transform.Find("Resistances").gameObject;
        weaponPreview = inventoryPanel.transform.Find("Display").transform.Find("Preview").gameObject;

        setDefaultWeapon((Weapon)Resources.Load("PlayerObjects/Weapons/Fists"));
        addItem((Weapon)Resources.Load("PlayerObjects/Weapons/RustyDagger"));

        equipWeapon(defaultWeapon);
        fullHeal();
        closeInventory();
    }

    public void fullHeal()
    {
        currentHealth = maxHealth;
        currentMana = maxMana;
        UpdatePointBars();
    }

    public int getBaseStat(string statString)
    {
        foreach (Armour.Stat stat in baseStats)
        {
            if (stat.name == statString)
            {
                return stat.value;
            }
        }
        return 0;
    }

    public int getStat(string statString)
    {
        foreach (Armour.Stat stat in currentStats)
        {
            if (stat.name == statString)
            {
                return stat.value;
            }
        }
        return 0;
    }

    void UpdatePointBars()
    {
        float perc = (float)currentHealth / (float)maxHealth;
        Healthbar.GetComponent<PointBar>().SetBar(perc);
        Healthbar.GetComponent<PointBar>().SetText(currentHealth.ToString());
        perc = (float)currentMana / (float)maxMana;
        Manabar.GetComponent<PointBar>().SetBar(perc);
        Manabar.GetComponent<PointBar>().SetText(currentMana.ToString());
    }

    public void updateStats()
    {
        maxHealth = 100 + (getStat("endurance") * 10) + ((level - 1) * 5);
        maxMana = 100 + (getStat("wisdom") * 5);
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        if (currentMana > maxMana)
        {
            currentMana = maxMana;
        }
        UpdatePointBars();
    }

    public void setDefaultWeapon(Weapon weapon)
    {
        defaultWeapon = weapon;
    }

    public void addItem(Item item)
    {
        inventory.Add(item);
    }

    void clearItemsPanel()
    {
        foreach(Transform child in content.transform) {
            Destroy(child.gameObject);
        }
    }

    void updateInventory()
    {
        clearItemsPanel();
        clearItemDisplay();
        GameObject myPrefab = Resources.Load("Prefabs/InventorySlot", typeof(GameObject)) as GameObject;
        GameObject button = Instantiate(myPrefab, new Vector3(0, 0, 0), myPrefab.transform.rotation);
        button.transform.parent = content.transform;
        button.transform.localScale = new Vector3(1, 1, 1);
        button.transform.Find("Name").gameObject.GetComponent<Text>().text = defaultWeapon.displayName;
        button.transform.Find("Level").gameObject.GetComponent<Text>().text = "Level " + defaultWeapon.level.ToString();
        button.transform.Find("ID").gameObject.GetComponent<Text>().text = "D";
        button.GetComponent<Button>().onClick.AddListener(() => slotPressed(button, defaultWeapon));
        List<GameObject> buttons = new List<GameObject>();
        int n = 1;
        foreach (Item item in inventory)
        {
            buttons.Add(Instantiate(myPrefab, new Vector3(0, 0, 0), myPrefab.transform.rotation));
            buttons[n-1].transform.parent = content.transform;
            buttons[n - 1].transform.localScale = new Vector3(1, 1, 1);
            buttons[n - 1].transform.Find("Name").gameObject.GetComponent<Text>().text = item.displayName;
            buttons[n - 1].transform.Find("Level").gameObject.GetComponent<Text>().text = "Level " + item.level.ToString();
            buttons[n - 1].transform.Find("ID").gameObject.GetComponent<Text>().text = n.ToString();

            int h = n;
            buttons[n - 1].GetComponent<Button>().onClick.AddListener(() => slotPressed(buttons[h - 1], item));
            n += 1;
        }
    }

    void clearItemDisplay()
    {
        weaponName.GetComponent<Text>().text = "";
        weaponStats.GetComponent<Text>().text = "";
        weaponCategory.GetComponent<Text>().text = "";
        weaponElements.GetComponent<Text>().text = "";
        weaponDescription.GetComponent<Text>().text = "";
        weaponPreview.GetComponent<Image>().sprite = null;
    }

    void slotPressed(GameObject thebutton, Item item)
    {
        selectedButton = thebutton;
        selectedItem = item;
        clearItemDisplay();
        if (item.type == "weapon")
        {
            Weapon weapon = (Weapon)item;
            weaponName.GetComponent<Text>().text = weapon.displayName;
            weaponCategory.GetComponent<Text>().text = "Level " + weapon.level.ToString() + " " + weapon.displayType;
            weaponDescription.GetComponent<Text>().text = weapon.description;
            weaponDescription.GetComponent<Text>().text += "\nDamage: " + weapon.minimum.ToString() + " - " + weapon.maximum.ToString() + " " + char.ToUpper(weapon.element[0]) + weapon.element.Substring(1);
            foreach(Armour.Stat stat in weapon.stats)
            {
                if (stat.value != 0)
                {
                    weaponStats.GetComponent<Text>().text += stat.name + " : " + stat.value.ToString() + "\n";
                }
            }
            foreach (Armour.Resistance resistance in weapon.resistances)
            {
                if (resistance.value != 0)
                {
                    weaponElements.GetComponent<Text>().text += resistance.name + " : " + resistance.value.ToString() + "\n";
                }
            }
            if (weapon.path != "")
            {
                GameObject myPrefab = Resources.Load(weapon.path, typeof(GameObject)) as GameObject;
                weaponPreview.GetComponent<Image>().sprite = myPrefab.GetComponent<SpriteRenderer>().sprite;
            }
        }
        else
        {
        }
    }

    public void closeInventory()
    {
        shopButton.active = false;
        shop = false;
        inventoryPanel.active = false;
    }

    public void openInventory()
    {
        tab = 0;
        equipButton.gameObject.GetComponentInChildren<Text>().text = "Equip";
        equipButton.GetComponent<Button>().onClick.RemoveAllListeners();
        equipButton.GetComponent<Button>().onClick.AddListener(() => equipPressed());
        
        

        inventoryPanel.active = true;
        updateInventory();
        updateEquips();
    }

    public void equipPressed()
    {
        unequipCurrent();
        equipWeapon((Weapon)selectedItem);
        setButtonEquipped(selectedButton);
        updateEquips();
    }

    void setButtonEquipped(GameObject button)
    {
        ColorBlock cb = button.GetComponent<Button>().colors;
        cb.normalColor = Color.cyan;
        button.GetComponent<Button>().colors = cb;
    }

    void setButtonUnequipped(GameObject button)
    {
        ColorBlock cb = button.GetComponent<Button>().colors;
        cb.normalColor = new Color(112.0f/256.0f, 196.0f / 256.0f, 157.0f / 256.0f, 1.0f);
        button.GetComponent<Button>().colors = cb;
    }

    void updateEquips()
    {
        foreach(Transform child in content.transform)
        {
            if (child.Find("Name").gameObject.GetComponent<Text>().text != currentWeapon.displayName)
            {
                setButtonUnequipped(child.gameObject);
            }
            else
            {
                setButtonEquipped(child.gameObject);
            }
        }
    }

    public void unequipCurrent()
    {
        int n = 0;
        foreach (Armour.Stat stat in currentWeapon.stats)
        {
            Armour.Stat statCopy = currentStats[n];
            statCopy.value -= stat.value;
            currentStats[n] = statCopy;
            n++;
        }
    }


    public void equipWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
        foreach (Transform child in RHWeaponSlot.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in LHWeaponSlot.transform)
        {
            Destroy(child.gameObject);
        }

        int n = 0;
        foreach(Armour.Stat stat in weapon.stats)
        {
            Armour.Stat statCopy = baseStats[n];
            statCopy.value += stat.value;
            currentStats[n] = statCopy;
            n++;
        }
        updateStats();
        if (currentHealth == maxHealth)
        {
            updateStats();
        }

        if (weapon.path != "")
        {
            GameObject myPrefab = Resources.Load(weapon.path, typeof(GameObject)) as GameObject;
            RHWeaponGameobject = Instantiate(myPrefab, new Vector3(0, 0, 0), myPrefab.transform.rotation);
            RHWeaponGameobject.transform.parent = RHWeaponSlot.transform;
            RHWeaponGameobject.transform.localPosition = new Vector3(0, 0, 0);
            
            Vector3 newScale = new Vector3(transform.localScale.x, Mathf.Abs(transform.localScale.y), transform.localScale.z);
            RHWeaponGameobject.transform.localScale = Vector3.Scale(Vector3.Scale(RHWeaponGameobject.transform.localScale, newScale), new Vector3(3.3f, 3.3f, 3.3f));

            RHWeaponGameobject.GetComponent<SpriteRenderer>().sortingOrder = 6;
            RHWeaponGameobject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            Debug.Log("WE HERE BRUH");
            Debug.Log(RHWeaponGameobject.transform.rotation.eulerAngles);
            

            if (weapon.dual)
            {
                LHWeaponGameobject = Instantiate(myPrefab, new Vector3(0, 0, 0), myPrefab.transform.rotation);
                LHWeaponGameobject.transform.parent = LHWeaponSlot.transform;
                LHWeaponGameobject.transform.localPosition = new Vector3(0, 0, 0);
                newScale = new Vector3(transform.localScale.x, Mathf.Abs(transform.localScale.y), transform.localScale.z);
                LHWeaponGameobject.transform.localScale = Vector3.Scale(Vector3.Scale(LHWeaponGameobject.transform.localScale, newScale), new Vector3(3.3f, 3.3f, 3.3f));
                if (transform.localScale.x < 0)
                {
                    LHWeaponGameobject.transform.localScale = new Vector3(LHWeaponGameobject.transform.localScale.x, LHWeaponGameobject.transform.localScale.y * -1, LHWeaponGameobject.transform.localScale.z);
                }
                
                LHWeaponGameobject.GetComponent<SpriteRenderer>().sortingOrder = -1;
                LHWeaponGameobject.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            Debug.Log(RHWeaponGameobject.transform.rotation.eulerAngles);
            Debug.Log("is!!");
        }

    }


    public void openShop(string resource)
    {
        tab = 1;
        shop = true;

        equipButton.gameObject.GetComponentInChildren<Text>().text = "Buy";
        equipButton.GetComponent<Button>().onClick.RemoveAllListeners();
        equipButton.GetComponent<Button>().onClick.AddListener(() => buyWeapon());

        shopButton.SetActive(true);
        inventoryButton.SetActive(true);
        inventoryButton.GetComponent<Button>().onClick.AddListener(openInventory);
        shopButton.GetComponent<Button>().onClick.AddListener(() => openShop(resource));

        List<Item> items = ((Shop)Resources.Load("Shops/" + resource)).items;

        clearItemsPanel();
        clearItemDisplay();
        GameObject myPrefab = Resources.Load("Prefabs/InventorySlot", typeof(GameObject)) as GameObject;
        GameObject button = Instantiate(myPrefab, new Vector3(0, 0, 0), myPrefab.transform.rotation);

        List<GameObject> buttons = new List<GameObject>();
        int n = 1;
        foreach (Item item in items)
        {
            buttons.Add(Instantiate(myPrefab, new Vector3(0, 0, 0), myPrefab.transform.rotation));
            buttons[n - 1].transform.parent = content.transform;
            buttons[n - 1].transform.localScale = new Vector3(1, 1, 1);
            buttons[n - 1].transform.Find("Name").gameObject.GetComponent<Text>().text = item.displayName;
            buttons[n - 1].transform.Find("Level").gameObject.GetComponent<Text>().text = "Level " + item.level.ToString();
            buttons[n - 1].transform.Find("ID").gameObject.GetComponent<Text>().text = n.ToString();

            int h = n;
            buttons[n - 1].GetComponent<Button>().onClick.AddListener(() => shopSlotPressed(buttons[h - 1], item));
            n += 1;
        }






        inventoryPanel.active = true;
    }

    void buyWeapon()
    {
        if (gold >= selectedItem.price)
        {
            if (inventory.Contains(selectedItem)) {
                popUp("You already have this!");
            }
            else
            {
                addItem(selectedItem);
                gold = gold - selectedItem.price;
                popUp("Bought " + selectedItem.displayName + " for " + selectedItem.price.ToString() + " gold!");

            }

        }
        else
        {
            popUp("Sorry, you're broke!");
        }
    }

    void shopSlotPressed(GameObject thebutton, Item item)
    {
        selectedButton = thebutton;
        selectedItem = item;
        clearItemDisplay();
        if (item.type == "weapon")
        {
            Weapon weapon = (Weapon)item;
            weaponName.GetComponent<Text>().text = weapon.displayName + " - " + weapon.price.ToString() + " gold";
            weaponCategory.GetComponent<Text>().text = "Level " + weapon.level.ToString() + " " + weapon.displayType;
            weaponDescription.GetComponent<Text>().text = weapon.description;
            weaponDescription.GetComponent<Text>().text += "\nDamage: " + weapon.minimum.ToString() + " - " + weapon.maximum.ToString() + " " + char.ToUpper(weapon.element[0]) + weapon.element.Substring(1);
            foreach (Armour.Stat stat in weapon.stats)
            {
                if (stat.value != 0)
                {
                    weaponStats.GetComponent<Text>().text += stat.name + " : " + stat.value.ToString() + "\n";
                }
            }
            foreach (Armour.Resistance resistance in weapon.resistances)
            {
                if (resistance.value != 0)
                {
                    weaponElements.GetComponent<Text>().text += resistance.name + " : " + resistance.value.ToString() + "\n";
                }
            }
            if (weapon.path != "")
            {
                GameObject myPrefab = Resources.Load(weapon.path, typeof(GameObject)) as GameObject;
                weaponPreview.GetComponent<Image>().sprite = myPrefab.GetComponent<SpriteRenderer>().sprite;
            }
        }
        else
        {
        }
    }

    public void credits()
    {
        popUp("Credits! Music by Eric Matyas, www.soundimage.org");
    }

    void popUp(string message)
    {
        GameObject myPrefab = Resources.Load("Prefabs/Info", typeof(GameObject)) as GameObject;
        GameObject scroll = Instantiate(myPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        foreach(var component in scroll.transform.Find("epico").gameObject.GetComponents<Component>()) {
            print(component);
        }
        scroll.transform.Find("epico").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = message;
        scroll.transform.SetParent(GameObject.Find("Canvas").transform, false);
        scroll.transform.position = Camera.main.WorldToScreenPoint(new Vector3(0, 0, 0));


        myPrefab = Resources.Load("Prefabs/MenuButton", typeof(GameObject)) as GameObject;
        GameObject button = Instantiate(myPrefab, Camera.main.WorldToScreenPoint(new Vector3(0, -2, 0)), Quaternion.identity);
        button.transform.Find("Text").GetComponent<Text>().text = "OK";
        button.transform.parent = GameObject.Find("Canvas").transform;
        button.transform.localScale = new Vector3(.5f, .5f, .5f);
        button.GetComponent<Button>().onClick.AddListener(() => Destroy(scroll));
        button.GetComponent<Button>().onClick.AddListener(() => Destroy(button));
    }

}
