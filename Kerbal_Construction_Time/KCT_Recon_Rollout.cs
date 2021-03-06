﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerbalConstructionTime
{
    public class KCT_Recon_Rollout : IKCTBuildItem
    {
        [Persistent] private string name = "";
        [Persistent] public double BP = 0, progress = 0;
        [Persistent] public string associatedID = "";
        [Persistent] public string launchPadID = "LaunchPad";
        public enum RolloutReconType { Reconditioning, Rollout, Rollback, Recovery, None };
        private RolloutReconType RRTypeInternal = RolloutReconType.None;
        public RolloutReconType RRType
        {
            get
            {
                if (RRTypeInternal != RolloutReconType.None)
                    return RRTypeInternal;
                else
                {
                    if (name == "LaunchPad Reconditioning")
                        RRTypeInternal = RolloutReconType.Reconditioning;
                    else if (name == "Vessel Rollout")
                        RRTypeInternal = RolloutReconType.Rollout;
                    else if (name == "Vessel Rollback")
                        RRTypeInternal = RolloutReconType.Rollback;
                    else if (name == "Vessel Recovery")
                        RRTypeInternal = RolloutReconType.Recovery;
                    return RRTypeInternal;
                }
            }
            set
            {
                RRTypeInternal = value;
            }
        }

        public KCT_KSC KSC { get { return KCT_GameStates.KSCs.Count > 0 ? KCT_GameStates.KSCs.FirstOrDefault(k => k.Recon_Rollout.Exists(r=> r.associatedID == this.associatedID)) : null;} }

        public KCT_Recon_Rollout()
        {
            name = "LaunchPad Reconditioning";
            progress = 0;
            BP = 0;
            RRType = RolloutReconType.None;
            associatedID = "";
            launchPadID = "LaunchPad";
        }

        public KCT_Recon_Rollout(Vessel vessel, RolloutReconType type, string id)
        {
            RRType = type;
            associatedID = id;
            BP = vessel.GetTotalMass() * KCT_GameStates.timeSettings.ReconditioningEffect * KCT_GameStates.timeSettings.OverallMultiplier; //1 day per 50 tons (default) * overall multiplier
            if (BP > KCT_GameStates.timeSettings.MaxReconditioning) BP = KCT_GameStates.timeSettings.MaxReconditioning;
            progress = 0;
            if (type == RolloutReconType.Reconditioning) 
            {
                BP *= (1 - KCT_GameStates.timeSettings.RolloutReconSplit);
                name = "LaunchPad Reconditioning";
            }
            else if (type == RolloutReconType.Rollout)
            {
                BP *= KCT_GameStates.timeSettings.RolloutReconSplit;
                name = "Vessel Rollout";
            }
            else if (type == RolloutReconType.Rollback)
            {
                BP *= KCT_GameStates.timeSettings.RolloutReconSplit;
                name = "Vessel Rollback";
                progress = BP;
            }
            else if (type == RolloutReconType.Recovery)
            {
                BP *= KCT_GameStates.timeSettings.RolloutReconSplit;
                name = "Vessel Recovery";
                double KSCDistance = (float)SpaceCenter.Instance.GreatCircleDistance(SpaceCenter.Instance.cb.GetRelSurfaceNVector(vessel.latitude, vessel.longitude));
                double maxDist = SpaceCenter.Instance.cb.Radius * Math.PI;
                BP += BP * (KSCDistance / maxDist);
            }
        }

        public KCT_Recon_Rollout(KCT_BuildListVessel vessel, RolloutReconType type, string id)
        {
            RRType = type;
            associatedID = id;
            BP = vessel.GetTotalMass() * KCT_GameStates.timeSettings.ReconditioningEffect * KCT_GameStates.timeSettings.OverallMultiplier; //1 day per 50 tons (default) * overall multiplier
            if (BP > KCT_GameStates.timeSettings.MaxReconditioning) BP = KCT_GameStates.timeSettings.MaxReconditioning;
            progress = 0;
            if (type == RolloutReconType.Reconditioning)
            {
                BP *= (1 - KCT_GameStates.timeSettings.RolloutReconSplit);
                name = "LaunchPad Reconditioning";
            }
            else if (type == RolloutReconType.Rollout)
            {
                BP *= KCT_GameStates.timeSettings.RolloutReconSplit;
                name = "Vessel Rollout";
            }
            else if (type == RolloutReconType.Rollback)
            {
                BP *= KCT_GameStates.timeSettings.RolloutReconSplit;
                progress = BP;
                name = "Vessel Rollback";
            }
            else if (type == RolloutReconType.Recovery)
            {
                BP *= KCT_GameStates.timeSettings.RolloutReconSplit;
                name = "Vessel Recovery";
                double maxDist = SpaceCenter.Instance.cb.Radius * Math.PI;
                BP += BP * (vessel.DistanceFromKSC / maxDist);
            }
        }

        public void SwapRolloutType()
        {
            if (RRType == RolloutReconType.Rollout)
            {
                RRType = RolloutReconType.Rollback;
                name = "Vessel Rollback";
            }
            else if (RRType == RolloutReconType.Rollback)
            {
                RRType = RolloutReconType.Rollout;
                name = "Vessel Rollout";
            }
        }

        public double ProgressPercent()
        {
            return Math.Round(100 * (progress / BP), 2);
        }

        string IKCTBuildItem.GetItemName()
        {
            return name;
        }

        double IKCTBuildItem.GetBuildRate()
        {
            List<double> rates = KCT_Utilities.BuildRatesVAB(KSC);
            double buildRate = 0;
            foreach (double rate in rates)
                buildRate += rate;
            if (RRType == RolloutReconType.Rollback)
                buildRate *= -1;
            return buildRate;
        }

        double IKCTBuildItem.GetTimeLeft()
        {
            double timeLeft = (BP - progress) / ((IKCTBuildItem)this).GetBuildRate();
            if (RRType == RolloutReconType.Rollback)
                timeLeft = (-progress) / ((IKCTBuildItem)this).GetBuildRate();
            return timeLeft;
        }

        KCT_BuildListVessel.ListType IKCTBuildItem.GetListType()
        {
            return KCT_BuildListVessel.ListType.Reconditioning;
        }

        bool IKCTBuildItem.IsComplete()
        {
            bool complete = progress >= BP;
            if (RRType == RolloutReconType.Rollback)
                complete = progress <= 0;
            return complete;
        }

        public IKCTBuildItem AsBuildItem()
        {
            return (IKCTBuildItem)this;
        }
    }
}
