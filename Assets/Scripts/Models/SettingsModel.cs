using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    public class SettingsModel
    {
        public class SettingsModelValuesRoot
        {
            // Mano con la que se va a jugar
            public static Dictionary<string, int> manoValores = new Dictionary<string, int>()
            {
                { "Derecha"  , 0 },
                { "Izquierda", 1 }
            };

            // Tiempo que el paciente debe de de mantener cada posición
            public static Dictionary<string, List<int>> tiempoEnPosicionValores = new Dictionary<string, List<int>>()
            {
                { "1 segundo", new List<int>(){ 1 } },
                { "5 segundos", new List<int>(){ 5 } },
                { "10 segundos", new List<int>(){ 10 } },
                { "Tiempos combinados (1s, 5s y 10s)", new List<int>(){ 1, 5, 10 } }
            };

            // Order en el que se realizaran los movimientos
            public static Dictionary<string, List<int>> ordenAparacionValores = new Dictionary<string, List<int>>()
            {
                { "Uno y uno", new List<int>(){ 1 } },
                { "Dos y dos", new List<int>(){ 2 } },
                { "Aleatorio", new List<int>(){ 1, 2 } }
            };

            // Número de sets
            public static Dictionary<string, int> numeroSetsValores = new Dictionary<string, int>()
            {
                { "1 set (10 ejercicios)"   , 10 },
                { "2 sets (20 ejercicios)"  , 20 },
                { "3 sets (30 ejercicios)"  , 30 },
                { "4 sets (40 ejercicios)"  , 40 },
                { "5 sets (50 ejercicios)"  , 50 },
                { "6 sets (60 ejercicios)"  , 60 },
                { "7 sets (60 ejercicios)"  , 70 },
                { "8 sets (80 ejercicios)"  , 80 },
                { "9 sets (90 ejercicios)"  , 90 },
                { "10 sets (100 ejercicios)", 100 },
            };

            // Tiempo de descanso entre cada set
            public static Dictionary<string, int> tiempoDescansoValores = new Dictionary<string, int>()
            {
                { "0 segundos" , 0 },
                { "10 segundos", 10 },
                { "20 segundos", 20 },
                { "30 segundos", 30 }
            };

            // Tiempo de reacción
            public static Dictionary<string, int> tiempoReaccionValores = new Dictionary<string, int>()
            {
                { "2 segundos (difícil)", 2 },
                { "5 segundos (medio)"  , 5 },
                { "20 segundos (fácil)" , 20 }
            };

            // Habrá distractores
            public static Dictionary<string, bool> conDistractoresValores = new Dictionary<string, bool>()
            {
                { "No", false },
                { "Si", true }
            };

            // Frecuencia de aparición de los distractores en porcentaje
            public static Dictionary<string, int> frecuenciaDistractoresValores = new Dictionary<string, int>()
            {
                { "Baja" , 25 },
                { "Media", 50 },
                { "Alta" , 100 }
            };

            public enum Movimiento
            {
                PS = 0,     // De pronación a supinación
                UFMPN = 1,  // Únicamente flexión de muñeca en posición neutra
                UEMPN = 2,  // Únicamente extensión de muñeca en posición neutra
                UFMP = 3,   // Únicamente flexión de muñeca en pronación
                UEMP = 4,   // Únicamente extensión de muñeca en pronación
                CFEPN = 5,  // Combinación de flexión y extensión en posición neutra
                CFEP = 6,   // Combinación de flexión y extensión en pronación
                CTM = 7     // Combinación de todos los movimientos
            }

            public static Dictionary<string, string> nombreMovimiento = new Dictionary<string, string>()
            {
                //{ "PS",     "De pronación a supinación"},
                //{ "UFMPN",  "Únicamente flexión de muñeca en posición neutra"},
                //{ "UEMPN",  "Únicamente extensión de muñeca en posición neutra"},
                //{ "UFMP",   "Únicamente flexión de muñeca en pronación"},
                //{ "UEMP",   "Únicamente extensión de muñeca en pronación"},
                //{ "CFEPN",  "Combinación de flexión y extensión en posición neutra"},
                //{ "CFEP",   "Combinación de flexión y extensión en pronación"},
                //{ "CTM",    "Combinación de todos los movimientos"}
                { "PS",     "De pronacion a supinacion"},
                { "UFMPN",  "Unicamente flexion de muneca en posicion neutra"},
                { "UEMPN",  "Unicamente extension de muneca en posicion neutra"},
                { "UFMP",   "Unicamente flexion de muneca en pronacion"},
                { "UEMP",   "Unicamente extension de muneca en pronacion"},
                { "CFEPN",  "Combinacion de flexion y extension en posicion neutra"},
                { "CFEP",   "Combinacion de flexion y extension en pronacion"},
                { "CTM",    "Combinacion de todos los movimientos"}
            };

            public enum Orientacion
            {
                NOIMPLEMENTADO = -1,
                ARRIBA = 0,
                DERECHA = 1,
                ABAJO = 2,
                IZQUIERDA = 3,
                NEUTRAL = 4
            }

            public static Dictionary<Movimiento, List<Orientacion>> nombreMovimientoValores = new Dictionary<Movimiento, List<Orientacion>>()
            {
                { Movimiento.PS, new List<Orientacion>(){ Orientacion.NOIMPLEMENTADO }},
                { Movimiento.UFMPN, new List<Orientacion>(){ Orientacion.DERECHA }},
                { Movimiento.UEMPN, new List<Orientacion>(){ Orientacion.IZQUIERDA }},
                { Movimiento.UFMP, new List<Orientacion>(){ Orientacion.ABAJO }},
                { Movimiento.UEMP, new List<Orientacion>(){ Orientacion.ARRIBA }},
                { Movimiento.CFEPN, new List<Orientacion>(){ Orientacion.DERECHA, Orientacion.IZQUIERDA }},
                { Movimiento.CFEP, new List<Orientacion>(){ Orientacion.ARRIBA, Orientacion.ABAJO }},
                { Movimiento.CTM, new List<Orientacion>(){ Orientacion.ARRIBA, Orientacion.DERECHA, Orientacion.ABAJO, Orientacion.IZQUIERDA }}
            };
        }

        public class SettingsModelRoot
        {
            // derecha o izquierda
            public string mano { get; set; }

            // Movement settings
            public SettingsModelValuesRoot.Movimiento nombreMovimiento { get; set; }

            // Time movement setting
            public string tiempoEnPosicion { get; set; }

            // Time movement setting
            public string ordenAparacion { get; set; }

            // Number od sets setting
            public string numeroSets { get; set; }

            // Rest time setting
            public string tiempoDescanso { get; set; }

            // Reaction time setting
            public string tiempoReaccion { get; set; }

            // Distractors setting
            public string conDistractores { get; set; }

            // Frequency spawn distractors setting
            public string frecuenciaDistractores { get; set; }

            public float anguloArriba { get; set; }
            public float anguloAbajo { get; set; }
            public float anguloIzquierda { get; set; }
            public float anguloDerecha { get; set; }
            public string comentario { get; set; }
        }
    }
}
