/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2016 EDIT Collective
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using EdiTree.Properties;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

namespace EdiTree.Components {
    public class PartitionListAdvComponent : GH_Component {
        public PartitionListAdvComponent()
            : base("Partition List Adv", "Partition", Resources.partitionlist_description, "EDIT", "EdiTree") {
        }

        protected override Bitmap Icon => Resources.partitionlist_24x24;
        public override Guid ComponentGuid => new Guid("{b2912c22-bad1-4bec-b00e-f11ec13f26a8}");

        protected override void RegisterInputParams(GH_InputParamManager pManager) {
            pManager.AddGenericParameter("List", "L", "List to partition", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Size", "S", "Size of partitions", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
            pManager.AddGenericParameter("Chunk", "C", "List Chunks", GH_ParamAccess.tree);
            pManager.AddGenericParameter("Remainder", "R", "Remaining Items in a List", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA) {
            var quotient = new GH_Structure<IGH_Goo>();
            var remainder = new GH_Structure<IGH_Goo>();

            var list = new List<IGH_Goo>();
            var size = 0;

            if (!DA.GetDataList(0, list)) { return; }
            if (!DA.GetData(1, ref size) || size == 0) { return; }

            var remain = list.Count%size;

            var num = -1;
            for (var i = 0; i < list.Count - remain; i += size) {
                quotient.AppendRange(list.GetRange(i, size), new GH_Path(++num));
            }
            remainder.AppendRange(list.GetRange(list.Count - remain, remain), new GH_Path(++num));

            DA.SetDataTree(0, quotient);
            DA.SetDataTree(1, remainder);
        }
    }
}