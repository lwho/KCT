﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerbalConstructionTime
{
    public class KCT_Settings
    {
        protected String filePath = KSPUtil.ApplicationRootPath + "GameData/KerbalConstructionTime/KCT_Config.txt";
        //[Persistent] public double BuildTimeModifier;
        [Persistent] public bool SandboxEnabled;
        [Persistent] public int MaxTimeWarp;
        [Persistent] public int SandboxUpgrades;
        [Persistent] public bool ForceStopWarp;
        [Persistent] public bool DisableRecoveryMessages;
        [Persistent] public bool DisableAllMessages;
        [Persistent] public bool CheckForUpdates, VersionSpecific;
        [Persistent] public bool AutoKACAlarams;
        [Persistent] public float RecoveryModifierDefault;
        [Persistent] public bool NoCostSimulationsDefault;
        [Persistent] public bool InstantTechUnlockDefault;
        [Persistent] public bool DisableBuildTimeDefault;
        [Persistent] public bool EnableAllBodiesDefault;
        [Persistent] public bool ReconditioningDefault;
        [Persistent] public bool Debug;
        [Persistent] public bool OverrideLaunchButton;
        [Persistent] public bool PreferBlizzyToolbar;
        [Persistent] public bool AllowParachuteRecovery;

        //Game specific settings
        public bool enabledForSave = true;
        public float RecoveryModifier;
        public bool NoCostSimulations;
        public bool InstantTechUnlock;
        public bool DisableBuildTime;
        public bool EnableAllBodies;
        public bool Reconditioning;
        
        //[Persistent] public bool AutoRevertOnCrash;
        //[Persistent] public bool Use6HourDays;


        public KCT_Settings() 
        {
           // BuildTimeModifier = 1.0;
            SandboxEnabled = true;
            MaxTimeWarp = TimeWarp.fetch.warpRates.Count() - 1;
            
            ForceStopWarp = false;
            SandboxUpgrades = 45;
            DisableRecoveryMessages = false;
            DisableAllMessages = false;
            CheckForUpdates = GameSettings.SEND_PROGRESS_DATA;
            VersionSpecific = false;
            Debug = false;
            OverrideLaunchButton = true;
            RecoveryModifier = 0.75F;
            Reconditioning = true;
            AutoKACAlarams = true;
            PreferBlizzyToolbar = false;
            AllowParachuteRecovery = true;

            RecoveryModifierDefault = 0.75f;
            NoCostSimulationsDefault = false;
            InstantTechUnlockDefault = false;
            DisableBuildTimeDefault = false;
            EnableAllBodiesDefault = false;
            ReconditioningDefault = true;

           // AutoRevertOnCrash = true;
            //Use6HourDays = GameSettings.KERBIN_TIME;
        }

        public void Load()
        {
            if (System.IO.File.Exists(filePath))
            {
                ConfigNode cnToLoad = ConfigNode.Load(filePath);
                ConfigNode.LoadObjectFromConfig(this, cnToLoad);

                if (RecoveryModifierDefault < 0) RecoveryModifierDefault = 0;
                if (RecoveryModifierDefault > 1) RecoveryModifierDefault = 1;

                if (KCT_GameStates.firstStart)
                {
                    RecoveryModifier = RecoveryModifierDefault;
                    NoCostSimulations = NoCostSimulationsDefault;
                    InstantTechUnlock = InstantTechUnlockDefault;
                    DisableBuildTime = DisableBuildTimeDefault;
                    EnableAllBodies = EnableAllBodiesDefault;
                    Reconditioning = ReconditioningDefault;
                }
            }
        }

        public void Save()
        {
            ConfigNode cnTemp = ConfigNode.CreateConfigFromObject(this, new ConfigNode());
            cnTemp.Save(filePath);
        }
    }

    public class KCT_TimeSettings
    {
        protected String filePath = KSPUtil.ApplicationRootPath + "GameData/KerbalConstructionTime/KCT_TimeSettings.txt";
        [Persistent] public double OverallMultiplier, BuildEffect, InventoryEffect, ReconditioningEffect, MaxReconditioning, RolloutReconSplit, NodeModifier;
        
        public KCT_TimeSettings()
        {
            OverallMultiplier = 1.0;
            BuildEffect = 1.0;
            InventoryEffect = 100.0;
            ReconditioningEffect = 1728;
            MaxReconditioning = 345600; // This is 4 days / 16 days at 1 BP/s  (or 200 tons)
            RolloutReconSplit = 0.25;
            NodeModifier = 1.0;
        }

        public void Load()
        {
            if (System.IO.File.Exists(filePath))
            {
                ConfigNode cnToLoad = ConfigNode.Load(filePath);
                ConfigNode.LoadObjectFromConfig(this, cnToLoad);
                if (OverallMultiplier < 0)
                    OverallMultiplier = 0;
                if (BuildEffect < 0)
                    BuildEffect = 0;
                if (InventoryEffect < 0)
                    InventoryEffect = 0;
            }
        }

        public void Save()
        {
            ConfigNode cnTemp = ConfigNode.CreateConfigFromObject(this, new ConfigNode());
            cnTemp.Save(filePath);
        }
    }

    public class KCT_FormulaSettings
    {
        protected String filePath = KSPUtil.ApplicationRootPath + "GameData/KerbalConstructionTime/KCT_Formulas.cfg";
        [Persistent] public string NodeFormula, UpgradeFundsFormula, UpgradeScienceFormula;
        [Persistent] public string NodeMax, UpgradeFundsMax, UpgradeScienceMax;
        public KCT_FormulaSettings()
        {
            NodeFormula = "2^([N]+1) / 86400"; //Rate = 2^(N+1)/86400 BP/s
            NodeMax = "0";
            UpgradeFundsFormula = "2^([N]+4) * 1000";
            UpgradeFundsMax = "1024000";
            UpgradeScienceFormula = "2^([N]+2) * 1.0";
            UpgradeScienceMax = "512";
        }

        public void Load()
        {
            if (System.IO.File.Exists(filePath))
            {
                ConfigNode cnToLoad = ConfigNode.Load(filePath);
                ConfigNode.LoadObjectFromConfig(this, cnToLoad.GetNode("KCT_FormulaSettings"));
            }
        }

        public void Save()
        {
            ConfigNode cnTemp = new ConfigNode("KCT_FormulaSettings");
            ConfigNode.CreateConfigFromObject(this, new ConfigNode()).CopyTo(cnTemp);
            ConfigNode toSave = new ConfigNode();
            toSave.AddNode(cnTemp);
            toSave.Save(filePath);
        }
    }
}
/*
Copyright (C) 2014  Michael Marvin, Zachary Eck

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/