﻿#region 注 释
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
using System;
using System.Collections.Generic;
using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace CZToolKit.GraphProcessor
{
    public abstract class GraphOwner : MonoBehaviour, IGraphOwner, IGraphAsset, IVariableOwner
    {
        #region �ֶ�
        protected List<SharedVariable> variables = new List<SharedVariable>();
        protected Dictionary<string, int> sharedVariableIndex;
        #endregion

        #region ����
        public abstract BaseGraph Graph { get; }
        public abstract Type GraphType { get; }
        #endregion

        #region Serialize
        public abstract void SaveVariables();

        public abstract void SaveGraph();

        public abstract void CheckGraphSerialization();

        protected abstract void CheckVaraiblesSerialization();
        #endregion

        #region API
        public UnityObject Self()
        {
            return this;
        }

        public string GetOwnerName()
        {
            return gameObject.name;
        }

        public SharedVariable GetVariable(string _guid)
        {
            if (string.IsNullOrEmpty(_guid)) return null;
            CheckVaraiblesSerialization();
            if (variables != null)
            {
                if (sharedVariableIndex == null || sharedVariableIndex.Count != variables.Count)
                    UpdateVariablesIndex();
                int index;
                if (sharedVariableIndex.TryGetValue(_guid, out index))
                    return variables[index];
            }
            return null;
        }

        public List<SharedVariable> GetAllVariables()
        {
            CheckVaraiblesSerialization();
            return variables;
        }

        public void SetVariable(SharedVariable sharedVariable)
        {
            if (sharedVariable == null) return;
            CheckVaraiblesSerialization();

            if (variables == null)
                variables = new List<SharedVariable>();
            else if (sharedVariableIndex == null)
                UpdateVariablesIndex();
            int index;
            if (sharedVariableIndex != null && sharedVariableIndex.TryGetValue(sharedVariable.GUID, out index))
            {
                SharedVariable sharedVariable2 = variables[index];
                if (!sharedVariable2.GetType().Equals(typeof(SharedVariable)) && !sharedVariable2.GetType().Equals(sharedVariable.GetType()))
                    Debug.LogError(string.Format("Error: Unable to set SharedVariable {0} - the variable type {1} does not match the existing type {2}", sharedVariable.GUID, sharedVariable2.GetType(), sharedVariable.GetType()));
                else
                    sharedVariable2.SetValue(sharedVariable.GetValue());
            }
            else
            {
                variables.Add(sharedVariable);
                UpdateVariablesIndex();
            }
        }

        public void SetVariableValue(string _guid, object _value)
        {
            GetVariable(_guid)?.SetValue(_value);
        }

        public IReadOnlyList<SharedVariable> GetVariables()
        {
            CheckVaraiblesSerialization();
            return variables;
        }

        public void SetVariables(List<SharedVariable> _variables)
        {
            variables = _variables;
            UpdateVariablesIndex();
        }
        #endregion

        protected void UpdateVariablesIndex()
        {
            if (variables == null)
            {
                if (sharedVariableIndex != null)
                    sharedVariableIndex = null;
                return;
            }
            if (sharedVariableIndex == null)
                sharedVariableIndex = new Dictionary<string, int>(variables.Count);
            else
                sharedVariableIndex.Clear();
            for (int i = 0; i < variables.Count; i++)
            {
                if (variables[i] != null)
                    sharedVariableIndex.Add(variables[i].GUID, i);
            }
        }
    }

    public abstract class GraphOwner<TGraph> : GraphOwner, ISerializationCallbackReceiver
        where TGraph : BaseGraph, new()
    {
        #region �ֶ�
        [HideInInspector]
        [SerializeField]
        TGraph graph = new TGraph();
        #endregion

        #region ����
        public override BaseGraph Graph
        {
            get { return graph; }
        }

        public TGraph T_Graph
        {
            get { return graph; }
        }

        #endregion

        #region Serialize
        #region Graph
        [NonSerialized]
        bool initializedGraph;
        [HideInInspector]
        [SerializeField]
        string serializedGraph;
        [HideInInspector]
        [SerializeField]
        List<UnityObject> graphUnityReferences;

        public override void SaveGraph()
        {
            serializedGraph = JsonSerializer.SerializeValue(graph, out graphUnityReferences);
        }

        void DeserializeGraph()
        {
            graph = JsonSerializer.DeserializeValue<TGraph>(serializedGraph, graphUnityReferences);
            graph.Enable();
            graph.InitializePropertyMapping(this);
        }

        public override void CheckGraphSerialization()
        {
            if (initializedGraph) return;
            initializedGraph = true;
            DeserializeGraph();
        }
        #endregion

        #region Variables
        [NonSerialized]
        bool initializedVariables;
        [HideInInspector]
        [SerializeField]
        string serializedVariables;
        [HideInInspector]
        [SerializeField]
        List<UnityObject> variablesUnityReferences;

        public override void SaveVariables()
        {
            serializedVariables = JsonSerializer.SerializeValue(variables, out variablesUnityReferences);
        }

        void DeserializeVariables()
        {
            variables = JsonSerializer.DeserializeValue<List<SharedVariable>>(serializedVariables, variablesUnityReferences);
            UpdateVariablesIndex();
        }

        protected override void CheckVaraiblesSerialization()
        {
            if (initializedVariables) return;
            initializedVariables = true;
            DeserializeVariables();
        }
        #endregion

        public void OnBeforeSerialize()
        {
            //SaveGraph();
            //SaveVariables();
        }

        public void OnAfterDeserialize()
        {
            CheckGraphSerialization();
            CheckVaraiblesSerialization();
        }

        #endregion

        public override Type GraphType { get { return typeof(TGraph); } }

        private void Reset()
        {
            graph = new TGraph();
        }
    }
}
