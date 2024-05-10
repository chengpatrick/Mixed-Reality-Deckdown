// Copyright (c) Meta Platforms, Inc. and affiliates.

using System;
using TMPro;
using UnityEngine;

namespace Discover.UI.Modal
{
    public class Re_NetworkSelectionMenu : MonoBehaviour
    {
        [SerializeField] private string m_roomName = "cmuetc";

        private Action<string> m_hostAction; // roomName
        private Action<string, bool> m_joinAction; // roomName, isRemote
        private Action m_singlePlayerAction;
        private Action m_settingAction;

        public void Initialize(
            Action<string> hostAction, // roomName
            Action<string, bool> joinAction, // roomName, isRemote
            Action singlePlayerAction,
            Action settingAction,
            string defaultRoomName = null)
        {
            if (!string.IsNullOrWhiteSpace(defaultRoomName))
            {
                m_roomName = defaultRoomName;
            }

            m_hostAction = hostAction;
            m_joinAction = joinAction;
            m_singlePlayerAction = singlePlayerAction;
            m_settingAction = settingAction;
        }

        public void OnHostClicked()
        {
            m_hostAction?.Invoke(m_roomName);
        }

        public void OnJoinClicked(bool remote)
        {
            m_joinAction?.Invoke(m_roomName, remote);
        }

        public void OnSinglePlayerClicked()
        {
            m_singlePlayerAction?.Invoke();
        }

        public void OnSettingsClicked()
        {
            m_settingAction?.Invoke();
        }
    }
}
