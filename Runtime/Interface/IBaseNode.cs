#region 注 释
/***
 *
 *  Title:
 *  
 *  Description:
 *  
 *  Date:
 *  Version:
 *  Writer: 
 *
 */
#endregion
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CZToolKit.GraphProcessor
{
    public interface IBaseNode
    {
        IBaseGraph Owner { get; }

        string GUID { get; }

        void Initialize(IBaseGraph baseGraph);
    }
}