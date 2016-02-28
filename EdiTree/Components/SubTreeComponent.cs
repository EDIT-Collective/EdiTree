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
using System.Linq;
using EdiTree.Properties;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;

namespace EdiTree.Components {
    public class SubTreeComponent : GH_Component, IGH_VariableParameterComponent {
        public SubTreeComponent()
            : base("SubTree", "SubTree", Resources.subtree_description, "EDIT", "EdiTree") {
            Params.ParameterSourcesChanged += (s, e) => {
                if (e.ParameterSide != GH_ParameterSide.Input || e.ParameterIndex != Params.Input.Count - 1) { return; }
                Params.RegisterInputParam(CreateParameter(GH_ParameterSide.Input, Params.Input.Count));
                VariableParameterMaintenance();
                Params.OnParametersChanged();
            };
        }

        protected override string HelpDescription => Resources.subtree_description_full;
        protected override Bitmap Icon => Resources.subtree_24x24;
        public override Guid ComponentGuid => new Guid("{b0f8796c-24bb-4dbf-8966-91737e9b93b4}"); 

        protected override void RegisterInputParams(GH_InputParamManager pManager) {
            pManager.AddGenericParameter("Tree", "T", "Data Tree", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("A", "A", "Path Index at the 1st Placeholder under Lexical Operations",
                GH_ParamAccess.item);
            pManager[0].MutableNickName = false;
            pManager[1].MutableNickName = false;
            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager) {
            pManager.AddTextParameter("Path", "P", "Branch Path", GH_ParamAccess.item);
            pManager.AddGenericParameter("SubTree", "S", "SubTree at the Given Path", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA) {
            GH_Structure<IGH_Goo> tree;
            var pList = new List<int?>();
            int? t = 0;

            if (!DA.GetDataTree(0, out tree)) return;
            for (var i = 1; i < Params.Input.Count; i++) { pList.Add(DA.GetData(i, ref t) ? t : null); }

            var subtree = new GH_Structure<IGH_Goo>();
            var descendant = new List<GH_Path>();

            foreach (var p in tree.Paths) {
                if (pList.Count > p.Length) { continue; }
                var flag = true;
                for (var i = 0; i < pList.Count; i++) {
                    flag = flag && (pList[i] == null || pList[i] == p[i]);
                }
                if (flag) { descendant.Add(p); }
            }

            int i1 = 0, i2 = 0;
            foreach (var p in descendant) {
                tree.PathIndex(p, ref i1, ref i2);
                subtree.AppendRange(tree.Branches[i1], p);
            }

            DA.SetData(0, "{" + string.Join(";", pList.Select(i => i != null ? i.ToString() : "?")) + "}");
            DA.SetDataTree(1, subtree);
        }

        #region IGH_VariableParameterComponent Impl

        private const string Digits = "0ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index) {
            return index == Params.Input.Count && side != GH_ParameterSide.Output;
        }

        bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index) {
            return index == Params.Input.Count - 1 && side != GH_ParameterSide.Output && Params.Input.Count - 1 > 1;
        }

        public IGH_Param CreateParameter(GH_ParameterSide side, int index) {
            return new Param_Integer {
                NickName = Digits[index].ToString(),
                Name = NickName,
                Description = NickName,
                Optional = true,
                MutableNickName = false
            };
        }

        bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index) {
            return true;
        }

        public void VariableParameterMaintenance() {
            for (var i = 1; i < Params.Input.Count; i++) {
                var param = Params.Input[i];
                param.NickName = Digits[i].ToString();
                param.Name = param.NickName;
                param.Description = $"Path Index at the {Util.Ordinal(i)} Placeholder under Lexical Operations";
                param.Optional = true;
                param.MutableNickName = false;
            }
        }

        #endregion
    }
}