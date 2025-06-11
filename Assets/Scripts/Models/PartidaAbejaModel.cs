using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.Models.SettingsModel;

public class PartidaAbejaModel
{
    public class PartidaAbejaModelRoot
    {
        public DateTime fecha { get; set; }
        public DateTime duracionPartida { get; set; }
        public SettingsModelRoot settings { get; set; }
        public int aciertos { get; set; }
        public int falsosAciertos { get; set; }
        public int floresNegras { get; set; }
        public int fallos { get; set; }
    }
}