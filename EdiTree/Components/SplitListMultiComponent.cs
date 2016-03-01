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
    public class SplitListMultiComponent : GH_Component {
        public SplitListMultiComponent()
            : base("Split List Multi", "Split Multi", Resources.splitlistmulti_description, "EDIT", "EdiTree") {
        }

        protected override string HelpDescription => Resources.splitlistmulti_description_full;
        protected override Bitmap Icon => Resources.splitlistmulti_24x24;
        public override Guid ComponentGuid => new Guid("{b8c107ac-2578-4c2c-b4e4-2ea56912d386}");

        protected override void RegisterInputParams(GH_InputParamManager pManager) {
            pManager.AddGenericParameter("List", "L", "Base List", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Index", "N", "Splitting index", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
            pManager.AddGenericParameter("Chunks", "C", "List Chunks", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA) {
            var tree = new GH_Structure<IGH_Goo>();
            var list = new List<IGH_Goo>();
            var indexList = new List<int>();

            if (!DA.GetDataList(0, list)) { return; }
            if (!DA.GetDataList(1, indexList)) { return; }

            var prev = 0;
            var num = 0;
            foreach (var i in indexList) {
                tree.AppendRange(list.GetRange(prev, i - prev), new GH_Path(num++));
                prev = i;
            }
            tree.AppendRange(list.GetRange(prev, list.Count - prev), new GH_Path(num));

            DA.SetDataTree(0, tree);
        }
    }
}