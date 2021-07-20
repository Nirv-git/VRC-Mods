﻿using System;
using UIExpansionKit.API;
using UnityEngine;

namespace UserHistory
{
    public class UIXManager
    {

        private static GameObject openButton;

        public static void AddOpenButtonToUIX()
        {
            ExpansionKitApi.GetExpandedMenu(ExpandedMenu.QuickMenu).AddSimpleButton("Open User History", new Action(MenuManager.OpenUserHistoryMenu), new Action<GameObject>((gameObject) => { openButton = gameObject; gameObject.SetActive(Config.useUIX.Value); }));
            Config.useUIX.OnValueChanged += OnUseUIXChange;
        }
        public static void OnUseUIXChange(bool oldValue, bool newValue)
        {
            if (oldValue == newValue) return;

            openButton?.SetActive(newValue);
            MenuManager.openButton.gameObject.SetActive(!newValue);
        }
    }
}
