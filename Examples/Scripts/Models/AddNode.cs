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
using UnityEngine;

namespace CZToolKit.GraphProcessor
{
    [NodeMenuItem("Math", "Add")]
    public class AddNode : BaseNode
    {
        #region Model
        [Input]
        [SerializeField] float input;

        [Output]
        [SerializeField] float output;
        #endregion

        #region ViewModel
        public override object GetValue(NodePort _port)
        {
            switch (_port.FieldName)
            {
                case nameof(output):
                    float inputSum = 0;
                    foreach (var value in GetConnectValues(nameof(input)))
                    {
                        inputSum += (float)value;
                    }
                    return inputSum;
            }

            return false;
        }
        #endregion
    }
}
