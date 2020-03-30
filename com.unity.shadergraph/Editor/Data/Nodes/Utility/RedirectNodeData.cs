using System.Collections.Generic;
using System.Linq;
using UnityEditor.Graphing;
using UnityEditor.Rendering;
using UnityEngine;

namespace UnityEditor.ShaderGraph
{
    class RedirectNodeData : AbstractMaterialNode
    {
        public const int kInputSlotID = 0;
        public const int kOutputSlotID = 1;
        public RedirectNodeData()
        {
            name = "Redirect Node";
        }

        public void SetPosition(Vector2 pos)
        {
            var temp = drawState;
            Vector2 offset = new Vector2(-30, -12);
            temp.position = new Rect(pos + offset, Vector2.zero);
            drawState = temp;
        }

        public void GetOutputAndInputSlots(out SlotReference outputSlotRef, out List<SlotReference> inputSlotRefs)
        {
            outputSlotRef = new SlotReference();
            inputSlotRefs =  new List<SlotReference>();
            var inputSlot = FindSlot<MaterialSlot>(kInputSlotID);

            IEnumerable<IEdge> inEdges = owner.GetEdges(inputSlot.slotReference);
            if (!inEdges.Any())
            {
                return;
            }

            outputSlotRef = inEdges.ToList()[0].outputSlot;
            var outputSlot = FindSlot<MaterialSlot>(kOutputSlotID);
            // Get the slot where this edge ends.
            IEnumerable<IEdge> outEdges = owner.GetEdges(outputSlot.slotReference);
            if (!outEdges.Any())
            {
                return;
            }
            foreach (var edge in outEdges)
            {
                inputSlotRefs.Add(edge.inputSlot);
            }
        }

        public override void ValidateNode()
        {
            base.ValidateNode();

            bool noInputs = false;
            bool noOutputs = false;
            var slots = new List<ISlot>();

            GetInputSlots(slots);
            foreach (var inSlot in slots)
            {
                var edges = owner.GetEdges(inSlot.slotReference).ToList();
                noInputs = !edges.Any();
            }

            slots.Clear();
            GetOutputSlots(slots);
            foreach (var outSlot in slots)
            {
                var edges = owner.GetEdges(outSlot.slotReference).ToList();
                noOutputs = !edges.Any();
            }

            if(noInputs && !noOutputs)
            {
                owner.AddValidationError(guid, "Node has no inputs and default value will be 0.", ShaderCompilerMessageSeverity.Warning);
            }
        }
    }
}
