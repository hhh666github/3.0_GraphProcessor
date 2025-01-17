#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 半只龙虾人
 *  Github: https://github.com/HalfLobsterMan
 *  Blog: https://www.crosshair.top/
 *
 */
#endregion
using System;
using System.Collections.Generic;

using UnityObject = UnityEngine.Object;

namespace CZToolKit.GraphProcessor
{
    [Serializable]
    public class ClipBoard
    {
        public static List<UnityObject> objectReferences;

        public List<BaseNode> copiedNodes = new List<BaseNode>();
        public List<BaseEdge> copiedEdges = new List<BaseEdge>();
        //public List<StackPanel> copiedStacks = new List<StackPanel>();
        public List<GroupPanel> copiedGroups = new List<GroupPanel>();
    }
}