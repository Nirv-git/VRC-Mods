using System;
using System.Collections;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using VRChatUtilityKit.Ui;

namespace InstanceHistory
{
    class MenuManager
    {
        private static int _instanceIndex = 0;
        public static int InstanceIndex
        {
            get { return _instanceIndex; }
            set
            {
                if (value > InstanceManager.instances.Count - 1 || value < 0)
                    return;

                _instanceIndex = value;

                pageNumLabel.TextComponent.text = $"<size=30>Page: {PageNum} of {LastPageNum}</size>";

                for (int i = 0; i < 12; i++)
                {
                    if (_instanceIndex + i >= InstanceManager.instances.Count)
                    {
                        buttons[i].TextComponent.text = "N/A";
                        buttons[i].Tooltip.field_Public_String_0 = "N/A";
                        buttons[i].ButtonComponent.onClick = new Button.ButtonClickedEvent();
                    }
                    else
                    {
                        InstanceManager.WorldInstance instance = InstanceManager.instances[InstanceIndex + i];
                        buttons[i].TextComponent.text = (instance.worldName + ": " + instance.instanceId.Split('~')[0]).Truncate(60);
                        buttons[i].Tooltip.field_Public_String_0 = instance.worldName + ": " + instance.instanceId.Split('~')[0];
                        buttons[i].ButtonComponent.onClick = new Button.ButtonClickedEvent();
                        buttons[i].ButtonComponent.onClick.AddListener(new Action(() => WorldManager.EnterWorld(instance.worldId + ":" + instance.instanceId)));
                    }
                }
            }
        }
        public static int PageNum
        {
            get { return Mathf.CeilToInt((_instanceIndex + 1) / 12f); }
        }
        public static int LastPageNum
        {
            get { return Mathf.CeilToInt(InstanceManager.instances.Count / 12f); }
        }

        public static Transform openButton;
        private static readonly SingleButton[] buttons = new SingleButton[12];
        private static SingleButton pageUp;
        private static SingleButton pageDown;
        private static Label pageNumLabel;
        private static SubMenu instanceHistorySub;
        private static Sprite worlds;

        public static void UiInit()
        {
            MelonLogger.Msg("Loading UI...");
            //worlds = UiManager.QMStateController.transform.Find("Container/Window/QMParent/Menu_Dashboard/ScrollRect/Viewport/VerticalLayoutGroup/Buttons_QuickLinks/Button_Worlds/Icon").GetComponent<Image>().sprite;

            openButton = UnityEngine.Object.Instantiate(UiManager.QMStateController.transform.Find("Container/Window/QMParent/Menu_Dashboard/Header_H1/RightItemContainer/Button_QM_Expand"));
            var butAction = new System.Action(() => OpenInstanceHistoryMenu());
            openButton.name = "InstanceHistory_UI";
            openButton.SetParent(UiManager.QMStateController.transform.Find("Container/Window/QMParent/"));
            openButton.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            openButton.GetComponent<Button>().onClick.AddListener(butAction);
            openButton.GetComponent<VRC.UI.Elements.Tooltips.UiTooltip>().field_Public_String_0 = "Instance History";
            openButton.GetComponentInChildren<Image>().overrideSprite = LoadAssets.MenuIcon;

            openButton.gameObject.SetActive(!(InstanceHistoryMod.HasUIX && Config.useUIX.Value));
            Config.openButtonX.OnValueChanged += OnPositionChange;
            Config.openButtonY.OnValueChanged += OnPositionChange;

            MelonCoroutines.Start(WaitForQM());

            instanceHistorySub = new SubMenu("InstanceHistory", "InstanceHistorySubMenu", "Instance History");
            ButtonGroup buttgroup = null;
            var allButtons = new System.Collections.Generic.List<IButtonGroupElement>();
            int b = 0;
            for (int i = 1; i < 16; i++)
            {
                //MelonLogger.Msg($"i:{i}, b:{b}");
                switch (i)
                {
                    case 4: pageUp = new SingleButton(new Action(() => InstanceIndex -= 12), LoadAssets.UpArrow, "Go up a page", $"UpPageButton", "Go up a page", (butt) => allButtons.Add(butt)); break;
                    case 8: pageDown = new SingleButton(new Action(() => InstanceIndex += 12), LoadAssets.DownArrow, "Go down a page", $"DownPageButton", "Go down a page", (butt) => allButtons.Add(butt)); break;
                    case 12: pageNumLabel = new Label($"Page: 1 of {LastPageNum}", "", "InstanceHistoryPageLabel", (butt) => allButtons.Add(butt));
                             pageNumLabel.gameObject.transform.Find("Text_H1").localPosition = new Vector3(0f, 40f, 0f); break;
                    case 16: new Label($"Blank", "subtext", "Blank", (butt) => allButtons.Add(butt)); break;
                    default:
                        buttons[b] = new SingleButton(null, LoadAssets.Item, "Placeholder", $"InstanceHistoryButton-{b}", "tooltip", (butt) => allButtons.Add(butt));
                        buttons[b].gameObject.transform.Find("Text_H4").localPosition = new Vector3(0f, -20f, 0f);
                        b++;
                        break;
                }
            }
            instanceHistorySub.AddButtonGroup(new ButtonGroup("InstanceHistoryButtons", "", allButtons, (group) => buttgroup = group));
            buttgroup.RemoveButtonHeader();

            if (InstanceHistoryMod.HasUIX)
                typeof(UIXManager).GetMethod("AddOpenButtonToUIX").Invoke(null, null);

            MelonLogger.Msg("UI Loaded!");
        }

        public static IEnumerator WaitForQM()
        {
            //Wait for the QM to open before cloning the button onto the Dashboard/Launch Pad Menu. This is because VRCUK uses that menu as a basis for it's custom TabMenus
            while (GameObject.Find("/UserInterface/Canvas_QuickMenu(Clone)/Container/Window/MicButton") == null)
                yield return new WaitForSeconds(1f);
            openButton.SetParent(UiManager.QMStateController.transform.Find("Container/Window/QMParent/Menu_Dashboard/Header_H1"));
            openButton.localPosition = new Vector3(Config.openButtonX.Value, Config.openButtonY.Value, 0f); //200f, -60f
            MelonLogger.Msg("Initialized QM Button!");
        }

        public static void OpenInstanceHistoryMenu()
        {
            UiManager.OpenSubMenu(UiManager.QMStateController.field_Private_UIPage_0, instanceHistorySub.uiPage);
            InstanceIndex = 0;
        }

        private static void OnPositionChange(float oldValue, float newValue)
        {
            if (oldValue == newValue) return;
        
            openButton.gameObject.transform.localPosition = new Vector3(Config.openButtonX.Value, Config.openButtonY.Value);
        }
    }
    public static class StringExt
    {
        public static string Truncate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength) + "...";
        }
    }
}
