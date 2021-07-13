﻿using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

namespace CZToolKit.GraphProcessor.Editors
{
    public sealed class GroupView : Group, IBindableView<GroupPanel>
    {
        public Label titleLabel { get; private set; }
        public ColorField colorField { get; private set; }
        public BaseGraphView Owner { get; private set; }
        public GroupPanel Model { get; private set; }

        public GroupView() : base()
        {
            styleSheets.Add(GraphProcessorStyles.GroupViewStyle);
            titleLabel = headerContainer.Q("titleLabel") as Label;
            colorField = new ColorField { name = "headerColorPicker" };
            headerContainer.Add(colorField);
        }

        public void SetUp(GroupPanel _group, CommandDispatcher _commandDispatcher, BaseGraphView _graphView)
        {
            Owner = _graphView;

            Model = _group;
            BindingProperties();
            Model.UpdateProperties();

            colorField.RegisterValueChangedCallback(e =>
            {
                Model.Color = e.newValue;
            });

            InitializeInnerNodes();
        }
        #region 数据监听回调
        void OnTitleChanged(string _title)
        {
            title = _title;
            Owner.SetDirty();
        }
        void OnPositionChanged(Rect _position)
        {
            base.SetPosition(_position);
            Owner.SetDirty();
        }
        void OnColorChanged(Color _color)
        {
            headerContainer.style.backgroundColor = _color;
            // 当明度大于0.5f,且透明度大于0.5f，文字颜色为黑色，否则为白色
            titleLabel.style.color = _color.GetLuminance() > 0.5f && _color.a > 0.5f ? Color.black : Color.white * 0.9f;
            colorField.SetValueWithoutNotify(_color);
            Owner.SetDirty();
        }

        void BindingProperties()
        {
            Model.RegisterValueChangedEvent<string>(nameof(Model.Title), OnTitleChanged);
            Model.RegisterValueChangedEvent<Rect>(nameof(Model.Position), OnPositionChanged);
            Model.RegisterValueChangedEvent<Color>(nameof(Model.Color), OnColorChanged);
        }

        public void UnBindingProperties()
        {
            Model.UnregisterValueChangedEvent<string>(nameof(Model.Title), OnTitleChanged);
            Model.UnregisterValueChangedEvent<Rect>(nameof(Model.Position), OnPositionChanged);
            Model.UnregisterValueChangedEvent<Color>(nameof(Model.Color), OnColorChanged);
        }
        #endregion

        public override bool AcceptsElement(GraphElement element, ref string reasonWhyNotAccepted)
        {
            return base.AcceptsElement(element, ref reasonWhyNotAccepted);
        }

        void InitializeInnerNodes()
        {
            foreach (var nodeGUID in Model.InnerNodeGUIDs)
            {
                if (!Owner.Model.Nodes.ContainsKey(nodeGUID)) continue;

                BaseNodeView nodeView = Owner.NodeViews[nodeGUID];
                AddElement(nodeView);
            }

            //foreach (var stackGUID in Model.InnerStackGUIDs)
            //{
            //    if (!Owner.Model.Stacks.ContainsKey(stackGUID)) continue;

            //    var stackView = Owner.StackViews[stackGUID];
            //    AddElement(stackView);
            //}
        }

        protected override void OnGroupRenamed(string _oldName, string _newName)
        {
            if (string.IsNullOrEmpty(_newName) || _oldName.Equals(_newName)) return;
            Model.Title = _newName;
        }

        public override void OnSelected()
        {
            base.OnSelected();
            BringToFront();

            foreach (GraphElement element in containedElements)
            {
                element.BringToFront();
            }
        }

        protected override void OnElementsAdded(IEnumerable<GraphElement> elements)
        {
            foreach (var element in elements)
            {
                BaseNodeView nodeView = element as BaseNodeView;
                if (nodeView != null && !Model.InnerNodeGUIDs.Contains(nodeView.Model.GUID))
                    Model.AddNode(nodeView.Model.GUID);
                //StackView stackNodeView = element as StackView;
                //if (stackNodeView != null && !Model.InnerStackGUIDs.Contains(stackNodeView.Model.GUID))
                //    Model.AddStack(stackNodeView.Model.GUID);
            }
            base.OnElementsAdded(elements);
        }

        protected override void OnElementsRemoved(IEnumerable<GraphElement> elements)
        {
            foreach (var element in elements)
            {
                BaseNodeView nodeView = element as BaseNodeView;
                if (nodeView != null)
                    Model.AddNode(nodeView.Model.GUID);
                StackView stackNodeView = element as StackView;
                if (stackNodeView != null)
                    Model.RemoveNode(stackNodeView.Model.GUID);
            }
            base.OnElementsRemoved(elements);
        }

        public override void SetPosition(Rect _newPos)
        {
            Model.Position = _newPos;
        }
    }
}