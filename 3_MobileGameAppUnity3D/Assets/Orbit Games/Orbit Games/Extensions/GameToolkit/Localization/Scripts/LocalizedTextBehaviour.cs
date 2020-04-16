// Copyright (c) H. Ibrahim Penekli. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameToolkit.Localization
{
    /// <summary>
    /// 
    /// </summary>
    public class LocalizedTextBehaviour : LocalizedGenericAssetBehaviour<LocalizedText, string>
    {
        [SerializeField]
        private string[] m_FormatArgs = new string[0];

        public string[] FormatArgs
        {
            get
            {
                return m_FormatArgs;
            }
            set
            {
                if (value == null)
                {
                    m_FormatArgs = new string[0];
                }
                else
                {
                    m_FormatArgs = value;
                }
                UpdateComponentValue();
            }
        }

        public OGFormattedLocalizedText FormattedAsset
        {
            get
            {
                return new OGFormattedLocalizedText
                {
                    LocalizedAsset = LocalizedAsset,
                    FormatArgs = FormatArgs
                };
            }
            set
            {
                LocalizedAsset = value == null ? null : value.LocalizedAsset;
                FormatArgs = value == null ? null : value.FormatArgs;
            }
        }

        protected override object GetLocalizedValue()
        {
            var value = (string)base.GetLocalizedValue();
            if (FormatArgs.Length > 0 && !string.IsNullOrEmpty(value))
            {
                try
                {
                    return string.Format(value, FormatArgs);
                }
                catch
                {
                    return "";
                }
            }
            return value;
        }

        private void Reset()
        {
            m_Component = GetComponent<Text>();
            if (m_Component)
            {
                m_Property = "text";
            }
        }
    }
}