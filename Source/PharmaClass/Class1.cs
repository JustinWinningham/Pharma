using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using Verse;
using RimWorld;

namespace Pharma
{
    public class Building_DistillationTank : Building
    {
        private int pasteCount;

        private float progressInt;

        private Material barFilledCachedMat;

        public const int MaxCapacity = 45;

        private const int BaseDistillationDuration = 600000;

        public const float MinIdealTemperature = 7f;

        private static readonly Vector2 BarSize = new Vector2(0.55f, 0.1f);

        private static readonly Color BarZeroProgressColor = new Color(0.4f, 0.27f, 0.22f);

        private static readonly Color BarFermentedColor = new Color(0.9f, 0.85f, 0.2f);

        private static readonly Material BarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.3f, 0.3f, 0.3f), false);

        public float Progress
        {
            get
            {
                return this.progressInt;
            }
            set
            {
                if (value == this.progressInt)
                {
                    return;
                }
                this.progressInt = value;
                this.barFilledCachedMat = null;
            }
        }

        private Material BarFilledMat
        {
            get
            {
                if(this.barFilledCachedMat == null)
                {
                    this.barFilledCachedMat = SolidColorMaterials.SimpleSolidColorMaterial(Color.Lerp(Building_DistillationTank.BarZeroProgressColor, Building_DistillationTank.BarFermentedColor, this.Progress), false);
                }
                return this.barFilledCachedMat;
            }
        }

        private bool Empty
        {
            get
            {
                return this.pasteCount <= 0;
            }
        }

        public bool Distilled
        {
            get
            {
                return !this.Empty && this.Progress >= 1f;
            }
        }

        public int SpaceLeftForPase
        {
            get
            {
                if(this.Distilled)
                {
                    return 0;
                }
                return MaxCapacity - this.pasteCount;
            }
        }

        private float CurrentTempProgressSpeedFactor
        {
            get
            {
                CompProperties_TemperatureRuinable compProperties = this.def.GetCompProperties<CompProperties_TemperatureRuinable>();
                float ambientTemp = base.AmbientTemperature;
                if (ambientTemp < compProperties.minSafeTemperature)
                {
                    return 0.1f;
                }
                if (ambientTemp < 7f)
                {
                    return GenMath.LerpDouble(compProperties.minSafeTemperature, 7f, 0.1f, 1f, ambientTemp);
                }
                return 1f;
            }
        }

        private float ProgressPerTickAtCurrentTemp
        {
            get
            {
                return 1.66666666E-06f * this.CurrentTempProgressSpeedFactor;
            }
        }

        private int EstimatedTicksLeft
        {
            get
            {
                return Mathf.Max(Mathf.RoundToInt((1f - this.Progress) / this.ProgressPerTickAtCurrentTemp), 0);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.pasteCount, "pasteCount", 0, false);
            Scribe_Values.Look<float>(ref this.progressInt, "progress", 0f, false);
        }

        // ******************************************************************************************

        public override void TickRare()
        {
            base.TickRare();
            if (!this.Empty)
            {
                this.Progress = Mathf.Min(this.Progress + 250f * this.ProgressPerTickAtCurrentTemp, 1f);
            }
        }

        public void AddPaste(int count)
        {
            base.GetComp<CompTemperatureRuinable>().Reset();
            if(this.Distilled)
            {
                Log.Warning("Tried to add paste to a full distillation tank. Colonists should take the product first.");
                return;
            }
            int num = Mathf.Min(count, MaxCapacity - this.pasteCount);
            if(num <= 0)
            {
                return;
            }
            this.Progress = GenMath.WeightedAverage(0f, (float)num, this.Progress, (float)this.pasteCount);
            this.pasteCount += num;
        }

        protected override void ReceiveCompSignal(string signal)
        {
            if (signal == "RuinedByTemperature") ;
            {
                this.Reset();
            }
        }

        private void Reset()
        {
            this.pasteCount = 0;
            this.Progress = 0f;
        }

        public void AddPaste(Thing paste)
        {
            int num = Mathf.Min(paste.stackCount, MaxCapacity - this.pasteCount);
            if(num > 0)
            {
                this.AddPaste(num);
                paste.SplitOff(num).Destroy(DestroyMode.Vanish);
            }
        }

        public override string GetInspectString()
        {
            return base.GetInspectString();
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(base.GetInspectString());
                if(stringBuilder.Length != 0)
                {
                    stringBuilder.AppendLine();
                }
                CompTemperatureRuinable comp = base.GetComp<CompTemperatureRuinable>();
                if(!this.Empty && !comp.Ruined)
                {
                    if (this.Distilled)
                    {
                        stringBuilder.AppendLine("ContainsDrugs".Translate(new object[]
                        {
                            this.pasteCount,
                            MaxCapacity
                        }));
                    }
                    else
                    {
                        stringBuilder.AppendLine("ContainsPaste".Translate(new object[]
                        {
                            this.pasteCount,
                            MaxCapacity
                        }));
                    }
                }
                if(!this.Empty)
                {

                }
            }
        }

    }
}
