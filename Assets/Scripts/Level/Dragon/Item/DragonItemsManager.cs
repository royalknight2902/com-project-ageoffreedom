﻿using UnityEngine;
using System.Collections;

public enum DragonItemType
{
    Head,
    Body,
    Ring,
    Amulet,
    Wing,
    Rune,
}

public class DragonItemsManager : Singleton<DragonItemsManager>
{

    public GameObject tempListDragonItem;
    public LevelSelectDragonAnimation dragonAnimation;
    public SDragonItemsAttribute attribute;

    [HideInInspector]
    public System.Collections.Generic.Dictionary<string, GameObject> listItems = new System.Collections.Generic.Dictionary<string, GameObject>();

    #region STATE MACHINE
    FiniteStateMachine<DragonItemsManager> FSM;

    public DragonItemsStateIdle stateIdle;

    EDragonStateAction stateAction;
    public EDragonStateAction StateAction
    {
        get
        {
            return stateAction;
        }
        set
        {
            if (stateAction != value)
            {
                stateAction = value;
                switch (stateAction)
                {
                    case EDragonStateAction.IDLE:
                        changeState(stateIdle);
                        break;
                }
            }
        }
    }

    void changeState(FSMState<DragonItemsManager> e)
    {
        FSM.Change(e);
        runResources();
    }

    void Update()
    {
        FSM.Update();
    }

    public void runResources()
    {
        dragonAnimation.changeResources(StateAction);
    }
    #endregion

    void Awake()
    {
        stateIdle = new DragonItemsStateIdle();

        FSM = new FiniteStateMachine<DragonItemsManager>();

        StateAction = EDragonStateAction.IDLE;
    }

    void Start()
    {
        initalize();
        runResources();
    }

    void OnEnable()
    {
        FSM.Configure(this, stateIdle);
        updateAttribute(PlayerInfo.Instance.dragonInfo.id);
    }

    void initalize()
    {
        #region DRAGON ITEM
        AutoDestroy.destroyChildren(tempListDragonItem, null);
        //Debug.Log(ReadDatabase.Instance.DragonItemInfo.Count);
        int i = 0;
        GameObject DragonItemBefore = null;
        float rowItemBefore = 0f;
        #region Code cũ
        foreach (System.Collections.Generic.KeyValuePair<string, DragonItemData> iterator in ReadDatabase.Instance.DragonInfo.Item)
        {
            GameObject dragonItem = Instantiate(Resources.Load<GameObject>("Prefab/Level/Dragon Item")) as GameObject;
            dragonItem.transform.parent = tempListDragonItem.transform;
            dragonItem.transform.localScale = Vector3.one;

            //set ID
            dragonItem.GetComponent<DragonItemsController>().ID = iterator.Key;

            //lay du lieu bonus tu item
            string bonusText = "";
            //int OptionInRow = 0; // 2 se xuong dong
            float row = 0f;

            for (int j = 0; j < iterator.Value.Options.Length; j++)
            {
                if (iterator.Value.Options[j] > 0)
                {

                    bonusText += DragonItemData.nameOptions[j] + "+" + iterator.Value.Options[j].ToString();

                    bonusText += "\n";
                    row++;
                }
            }
            bonusText = bonusText.Substring(0, bonusText.Length - 1); // bo /n cuoi cung, do hon xet if trong vong lap

            //Anchor
            UIAnchor uiAnchor = dragonItem.GetComponent<UIAnchor>();

            //Stretch
            UIStretch uiStretch = dragonItem.GetComponent<UIStretch>();

            if (i == 0) // Phan tu dau neo theo cha
            {
                uiAnchor.container = tempListDragonItem;
                uiStretch.container = tempListDragonItem;
                uiStretch.relativeSize = new Vector2(uiStretch.relativeSize.x, row / 10);
                dragonItem.GetComponent<UIDragScrollView>().scrollView = tempListDragonItem.GetComponent<UIScrollView>();
            }
            else // cac phan tu sau noi duoi voi nhau
            {
                uiAnchor.container = DragonItemBefore;
                uiAnchor.relativeOffset.y = -0.52f;
                uiAnchor.side = UIAnchor.Side.Center;
                //  dragonItem.GetComponent<UIWidget>().pivot = UIWidget.Pivot.Top;


                uiStretch.container = DragonItemBefore;
                uiStretch.relativeSize = new Vector2(uiStretch.relativeSize.x, row / rowItemBefore);
                dragonItem.GetComponent<UIDragScrollView>().scrollView = DragonItemBefore.GetComponent<UIScrollView>();
            }


        #endregion

            #region code thu nghiem
            //foreach (System.Collections.Generic.KeyValuePair<int, DragonItemData> iterator in ReadDatabase.Instance.DragonItemInfo)
            //{

            //    GameObject dragonItem = Instantiate(LevelManager.Instance.Model.DragonItem) as GameObject;
            //    dragonItem.transform.parent = tempListDragonItem.transform;
            //    dragonItem.transform.localScale = Vector3.one;

            //    //lay du lieu bonus tu item
            //    string bonusText = "";
            //    //int OptionInRow = 0; // 2 se xuong dong
            //    float row = 0f;

            //    for (int j = 0; j < iterator.Value.Options.Length; j++)
            //    {
            //        if (iterator.Value.Options[j] > 0)
            //        {
            //            bonusText += DragonItemData.nameOptions[j] + "+" + iterator.Value.Options[j].ToString();

            //            bonusText += "\n";
            //            row++;
            //        }
            //    }
            //    bonusText  = bonusText.Substring(0,bonusText.Length - 2); // bo /n cuoi cung, do hon xet if trong vong lap

            //    //Stretch
            //    UIStretch uiStretch =  dragonItem.GetComponent<UIStretch>();



            //    uiStretch.container = tempListDragonItem;
            //    uiStretch.relativeSize = new Vector2(uiStretch.relativeSize.x, row / 10);
            //    dragonItem.GetComponent<UIDragScrollView>().scrollView = tempListDragonItem.GetComponent<UIScrollView>();
            //    float currentPosY = 0f;
            //    if (i == 0)
            //        currentPosY = dragonItem.GetComponentInParent<UIPanel>().height / 2 - dragonItem.GetComponent<UIWidget>().height / 2;
            //    else
            //        currentPosY = beforePositionY - dragonItem.GetComponent<UIWidget>().height / 2;

            //    dragonItem.transform.localPosition = new Vector3(0, currentPosY, 0);
            //    Debug.Log(dragonItem.transform.localPosition);
            #endregion
            switch (iterator.Value.Name.Substring(iterator.Value.Name.LastIndexOf(' ') + 1))
            {
                case "Head":
                    dragonItem.GetComponent<UIDragonItems>().dragonItemType = DragonItemType.Head;
                    break;
                case "Body":
                    dragonItem.GetComponent<UIDragonItems>().dragonItemType = DragonItemType.Body;
                    break;
                case "Ring":
                    dragonItem.GetComponent<UIDragonItems>().dragonItemType = DragonItemType.Ring;
                    break;
                case "Amulet":
                    dragonItem.GetComponent<UIDragonItems>().dragonItemType = DragonItemType.Amulet;
                    break;
                case "Wing":
                    dragonItem.GetComponent<UIDragonItems>().dragonItemType = DragonItemType.Wing;
                    break;
                case "Rune":
                    dragonItem.GetComponent<UIDragonItems>().dragonItemType = DragonItemType.Rune;
                    break;
            }

            dragonItem.transform.GetChild(0).gameObject.GetComponent<UILabel>().text = iterator.Value.Name;
            dragonItem.transform.GetChild(1).gameObject.GetComponent<UILabel>().text = bonusText;
            dragonItem.transform.GetChild(2).gameObject.GetComponent<UISprite>().spriteName = iterator.Value.Icon;

            dragonItem.SetActive(true);
            DragonItemBefore = dragonItem;
            rowItemBefore = row;

            listItems.Add(iterator.Key, dragonItem);

            i++;
        }
        #endregion

        #region LOAD DATABASE
        string dataHead = PlayerInfo.Instance.dragonInfo.itemHead;
        string dataWing = PlayerInfo.Instance.dragonInfo.itemWing;
        string dataRing = PlayerInfo.Instance.dragonInfo.itemRing;
        string dataAmulet = PlayerInfo.Instance.dragonInfo.itemAmulet;
        string dataBody = PlayerInfo.Instance.dragonInfo.itemBody;
        string dataRune = PlayerInfo.Instance.dragonInfo.itemRune;

        if (!dataHead.Equals(""))
        {
            GameObject item = listItems[dataHead];
            item.GetComponent<UIDragonItems>().EquipItem();
        }
        if (!dataWing.Equals(""))
        {
            GameObject item = listItems[dataWing];
            item.GetComponent<UIDragonItems>().EquipItem();
        }
        if (!dataRing.Equals(""))
        {
            GameObject item = listItems[dataRing];
            item.GetComponent<UIDragonItems>().EquipItem();
        }
        if (!dataAmulet.Equals(""))
        {
            GameObject item = listItems[dataAmulet];
            item.GetComponent<UIDragonItems>().EquipItem();
        }
        if (!dataBody.Equals(""))
        {
            GameObject item = listItems[dataBody];
            item.GetComponent<UIDragonItems>().EquipItem();
        }
        if (!dataRune.Equals(""))
        {
            GameObject item = listItems[dataRune];
            item.GetComponent<UIDragonItems>().EquipItem();
        }
        #endregion
    }

    public void updateAttribute(string branch)
    {
        DragonPlayerData data = ReadDatabase.Instance.DragonInfo.Player[branch];
        attribute.Name.text = branch;
        attribute.HP.text = data.HP.ToString();
        attribute.MP.text = data.MP.ToString();
        attribute.ATK.text = data.ATK.Min + " - " + data.ATK.Max;
        attribute.DEF.text = data.DEF.ToString();
        attribute.ATKSpeed.text = data.ATKSpeed.ToString();
        attribute.MoveSpeed.text = data.MoveSpeed.ToString();
    }

    public void updateListItem(GameObject destroyObject)
    {
        if (destroyObject.GetComponent<UIAnchor>().side == UIAnchor.Side.TopLeft) // item dau
        {
            GameObject deleteItem = tempListDragonItem.transform.GetChild(0).gameObject;
            UIAnchor uiAnchorDeleteItem = deleteItem.GetComponent<UIAnchor>();
            UIStretch uiStretchDeleteItem = deleteItem.GetComponent<UIStretch>();
            if (tempListDragonItem.transform.childCount > 1)
            {
                GameObject secondItem = tempListDragonItem.transform.GetChild(1).gameObject;
                UIAnchor uiAnchorSecondItem = secondItem.GetComponent<UIAnchor>();
                UIStretch uiStretchSecondItem = secondItem.GetComponent<UIStretch>();
                uiAnchorSecondItem.container = uiAnchorDeleteItem.container;
                uiAnchorSecondItem.side = uiAnchorDeleteItem.side;
                uiAnchorSecondItem.relativeOffset = uiAnchorDeleteItem.relativeOffset;

                uiStretchSecondItem.container = uiStretchDeleteItem.container;
                secondItem.transform.localPosition = deleteItem.transform.localPosition;
                secondItem.GetComponent<UIWidget>().pivot = UIWidget.Pivot.TopLeft;
            }
            AutoDestroy.Destroy(destroyObject);
        }
        else
        {

        }
    }

    public void equipItemForDragon(string itemName)
    {
        DragonItemData itemData = loadInfoItem(itemName);
    }

    DragonItemData loadInfoItem(string itemName)
    {
        foreach (System.Collections.Generic.KeyValuePair<string, DragonItemData> iterator in ReadDatabase.Instance.DragonInfo.Item)
        {
            if (iterator.Value.Name == itemName)
                return iterator.Value;
        }
        return null;
    }
}
