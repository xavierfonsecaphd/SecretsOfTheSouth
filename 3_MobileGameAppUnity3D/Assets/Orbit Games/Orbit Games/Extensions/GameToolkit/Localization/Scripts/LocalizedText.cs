// Copyright (c) H. Ibrahim Penekli. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using UnityEngine;

namespace GameToolkit.Localization
{
    /// <summary>
    /// 
    /// </summary>
    [CreateAssetMenu(fileName = "LocalizedText", menuName = "GameToolkit/Localization/Text")]
    public class LocalizedText : LocalizedAsset<string>
    {
        [Serializable]
        private class TextLocaleItem : LocaleItem<string> { };

        [SerializeField]
        private TextLocaleItem[] m_LocaleItems = new TextLocaleItem[1];

        public override LocaleItemBase[] LocaleItems { get { return m_LocaleItems; } }

        /// <summary>
        /// Hack in the localization plugin to allow dynamic creation of localization assets 
        /// </summary>
        /// <param name="text"></param>
        public void SetText(string text)
        {
            m_LocaleItems[0] = new TextLocaleItem();
            m_LocaleItems[0].Language = SystemLanguage.English;
            m_LocaleItems[0].Value = text;
        }
    }
}
