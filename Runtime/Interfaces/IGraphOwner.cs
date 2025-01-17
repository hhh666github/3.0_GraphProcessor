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

using CZToolKit.Core.SharedVariable;

namespace CZToolKit.GraphProcessor
{
    public interface IGraphOwner : IVariableOwner
    {
        BaseGraph Graph { get; }

        void SaveVariables();
    }
}