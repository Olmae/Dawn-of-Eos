﻿

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeMonkey {


    public class Assets : MonoBehaviour {

        private static Assets _i; 

        // Instance reference
        public static Assets i {
            get {
                if (_i == null) _i = Instantiate(Resources.Load<Assets>("CodeMonkeyAssets")); 
                return _i; 
            }
        }


        // All references
        
        public Sprite s_White;
        public Sprite s_Circle;

        public Material m_White;

    }

}
