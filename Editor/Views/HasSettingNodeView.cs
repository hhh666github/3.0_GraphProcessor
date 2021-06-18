﻿using CZToolKit.Core;
using CZToolKit.Core.Editors;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace CZToolKit.GraphProcessor.Editors
{
    public interface IHasSettingsView
    {
        void CloseSettings();
    }

    public class HasSettingNodeView : BaseNodeView, IHasSettingsView
    {
        bool settingsExpanded = false;
        NodeSettingsView settingsContainer;
        VisualElement settings;
        Button settingButton;

        public HasSettingNodeView()
        {
            styleSheets.Add(GraphProcessorStyles.SettingsNodeViewStyle);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            InitializeSettings();
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            OnGeometryChanged(null);
        }

        void InitializeSettings()
        {
            CreateSettingButton();
            settingsContainer = new NodeSettingsView();
            settingsContainer.visible = false;
            settings = new VisualElement();
            settings.Add(CreateSettingsView());
            settingsContainer.Add(settings);
            Add(settingsContainer);

            foreach (var fieldInfo in NodeDataTypeFieldInfos)
            {
                if (Utility_Attribute.TryGetFieldInfoAttribute(fieldInfo, out ShowInSettingAttribute settingAttribute))
                    AddSettingField(fieldInfo);
            }
        }

        protected void AddSettingField(FieldInfo _fieldInfo)
        {
            var label = Utility_Attribute.TryGetFieldInfoAttribute(_fieldInfo, out InspectorNameAttribute inspectorNameAttribute)
                ? inspectorNameAttribute.displayName : _fieldInfo.Name;
            VisualElement fieldDrawer = CreateControlField(_fieldInfo, label);
            if (fieldDrawer == null) return;
            settingsContainer.Add(fieldDrawer);
        }

        void OnGeometryChanged(GeometryChangedEvent evt)
        {
            if (settingButton != null)
            {
                var settingsButtonLayout = settingButton.ChangeCoordinatesTo(settingsContainer.parent, settingButton.layout);
                settingsContainer.style.top = settingsButtonLayout.yMax - 18f;
                settingsContainer.style.left = settingsButtonLayout.xMin - layout.width + 20f;
            }
        }

        void CreateSettingButton()
        {
            settingButton = new Button(ToggleSettings) { name = "settings-button" };
            settingButton.Add(new Image { name = "icon", scaleMode = ScaleMode.ScaleToFit });

            titleContainer.Add(settingButton);
        }

        void ToggleSettings()
        {
            settingsExpanded = !settingsExpanded;
            if (settingsExpanded)
                OpenSettings();
            else
                CloseSettings();
        }

        public void OpenSettings()
        {
            if (settingsContainer != null)
            {
                Owner.ClearSelection();
                Owner.AddToSelection(this);

                settingButton.AddToClassList("clicked");
                settingsContainer.visible = true;
                settingsExpanded = true;
            }
        }

        public void CloseSettings()
        {
            if (settingsContainer != null)
            {
                settingButton.RemoveFromClassList("clicked");
                settingsContainer.visible = false;
                settingsExpanded = false;
            }
        }

        protected virtual VisualElement CreateSettingsView() { return new Label("Settings") { name = "header" }; }

    }
}