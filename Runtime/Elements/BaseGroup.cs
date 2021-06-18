﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace CZToolKit.GraphProcessor
{
    [Serializable]
    public class BaseGroup : IGraphElement
    {
        public string title;
        public Color color = new Color(0, 0, 0, 0.7f);
        public Rect position;

        public List<string> innerNodeGUIDs = new List<string>();
        public List<string> innerStackGUIDs = new List<string>();

        public BaseGroup(string _title, Vector2 _position)
        {
            title = _title;
            position.position = _position;
            position.size = Vector2.one * 300;
        }

        public void OnCreated() { }
    }
}