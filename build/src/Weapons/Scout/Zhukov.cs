using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.DeepDuckGalactic
{
	//[EditorGroup("Faecterr's|Guns")]
	public class Zhukov : Gun
    {
		Sprite _sprite = new Sprite("smg", 0f, 0f);
		public Zhukov(float x, float y) : base(x, y)
        {
			ammo = 25;
			_ammoType = new AT9mm();
			_ammoType.range = 150f;
			_ammoType.accuracy = 0.6f;
			_type = "gun";
			graphic = _sprite;
			center = new Vec2(8f, 4f);
			collisionOffset = new Vec2(-8f, -4f);
			collisionSize = new Vec2(16f, 8f);
			_barrelOffsetTL = new Vec2(17f, 2f);
			_fireSound = "smg";
			_fullAuto = true;
			_fireWait = 0.4f;
			_kickForce = 1f;
			_fireRumble = RumbleIntensity.Kick;
			_holdOffset = new Vec2(-1f, 0f);
			loseAccuracy = 0.05f;
			maxAccuracyLost = 0.5f;
			editorTooltip = "Rapid-fire bullet-spitting machine. Great for making artisanal swiss cheese.";
		}

        public override void Update()
        {
            base.Update();
			if (_barrelOffsetTL.x == 24)
			{
				_barrelOffsetTL = new Vec2(17f, 2f);
			}
			else
			{
				_barrelOffsetTL = new Vec2(24f, 2f);
			}
		}

        public override void Draw()
        {
            base.Draw();
			if(duck != null && duck._spriteArms != null)
            {
				duck.holdAngleOff = duck.hSpeed * 0.2f;
				Graphics.Draw(duck._spriteArms, position.x + 7 * offDir, position.y, depth - 6);
				Graphics.Draw(_sprite, position.x + 7 * offDir, position.y, depth - 5);
            }
        }
    }
}
