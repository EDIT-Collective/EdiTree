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
    public class DivideListComponent : GH_Component {
        public DivideListComponent()
            : base("Divide List", "Divide List", Resources.dividelist_description, "EDIT", "EdiTree") {
        }

        protected override string HelpDescription => Resources.dividelist_description_full;
        protected override Bitmap Icon => Resources.dividelist_24x24;
        public override Guid ComponentGuid => new Guid("{50920de4-ba65-4b88-a37f-4475ac81c77d}");

        protected override void RegisterInputParams(GH_InputParamManager pManager) {
            pManager.AddGenericParameter("List", "L", "List to Divide", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Number", "N", "Number of Lists", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
            pManager.AddGenericParameter("Chunk", "C", "Partitioned List as Chunks", GH_ParamAccess.tree);
            pManager.AddGenericParameter("Remainder", "R", "Remainder Items in a List", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA) {
            var quotient = new GH_Structure<IGH_Goo>();
            var remainder = new GH_Structure<IGH_Goo>();

            var list = new List<IGH_Goo>();
            var divisor = 0;

            if (!DA.GetDataList(0, list)) { return; }
            if (!DA.GetData(1, ref divisor) || divisor == 0) { return; }

            var size = list.Count / divisor;
            if (size < 1) { return; }

            var remain = list.Count % divisor;

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