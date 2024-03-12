using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.DeepDuckGalactic
{
    public class DeepcoreGK2 : Gun
    {
        public DeepcoreGK2(float xval, float yval) : base(xval, yval)
        {
            ammo = 30; 
            _ammoType = new ATHighCalMachinegun(); 
            _type = "gun"; 
			graphic = new Sprite("ak47", 0f, 0f);
			center = new Vec2(16f, 15f);
			collisionOffset = new Vec2(-8f, -3f);
			collisionSize = new Vec2(18f, 10f);
			_barrelOffsetTL = new Vec2(32f, 14f);
			_fireSound = "deepMachineGun2";
			_fullAuto = true;
			_fireWait = 1.125f;
			_kickForce = 0.5f;
			_fireRumble = RumbleIntensity.Kick;
			loseAccuracy = 0.1f;
			maxAccuracyLost = 0.5f;
			editorTooltip = "Go-to weapon of all your favorite Duck Action Heroes.";
		}
    }
}
