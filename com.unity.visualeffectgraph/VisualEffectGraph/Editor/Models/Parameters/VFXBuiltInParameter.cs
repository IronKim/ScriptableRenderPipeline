using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;

namespace UnityEditor.VFX
{
    class BuiltInVariant : IVariantProvider
    {
        public Dictionary<string, object[]> variants
        {
            get
            {
                return new Dictionary<string, object[]>
                {
                    { "m_expressionOp", VFXBuiltInExpression.All.Cast<object>().ToArray() }
                };
            }
        }
    }

    [VFXInfo(category = "BuiltIn", variantProvider = typeof(BuiltInVariant))]
    class VFXBuiltInParameter : VFXOperator
    {
        [SerializeField, VFXSetting(VFXSettingAttribute.VisibleFlags.None)]
        protected VFXExpressionOperation m_expressionOp;

        override public string name { get { return m_expressionOp.ToString(); } }

        public override void Sanitize()
        {
            if (VFXBuiltInExpression.Find(m_expressionOp) == null)
            {
                switch (m_expressionOp)
                {
                    // Do to reorganization, some indices have changed
                    case VFXExpressionOperation.Tan:         m_expressionOp = VFXExpressionOperation.DeltaTime; break;
                    case VFXExpressionOperation.ASin:        m_expressionOp = VFXExpressionOperation.TotalTime; break;
                    case VFXExpressionOperation.ACos:        m_expressionOp = VFXExpressionOperation.SystemSeed; break;
                    case VFXExpressionOperation.RGBtoHSV:    m_expressionOp = VFXExpressionOperation.LocalToWorld; break;
                    case VFXExpressionOperation.HSVtoRGB:    m_expressionOp = VFXExpressionOperation.WorldToLocal; break;

                    default:
                        Debug.LogWarning(string.Format("Expression operator for the BuiltInParameter is invalid ({0}). Reset to none", m_expressionOp));
                        m_expressionOp = VFXExpressionOperation.None;
                        break;
                }
            }
            base.Sanitize(); // Will call ResyncSlots
        }

        protected override IEnumerable<VFXPropertyWithValue> outputProperties
        {
            get
            {
                var expression = VFXBuiltInExpression.Find(m_expressionOp);
                if (expression != null)
                    yield return new VFXPropertyWithValue(new VFXProperty(VFXExpression.TypeToType(expression.valueType), m_expressionOp.ToString()));
            }
        }

        protected override VFXExpression[] BuildExpression(VFXExpression[] inputExpression)
        {
            var expression = VFXBuiltInExpression.Find(m_expressionOp);
            if (expression == null)
                return new VFXExpression[] {};
            return new VFXExpression[] { expression };
        }
    }
}
