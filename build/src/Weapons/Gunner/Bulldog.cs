using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.DeepDuckGalactic
{
	public class Bulldog : Gun
	{
		public StateBinding _angleOffsetBinding = new StateBinding("_angleOffset", -1, false, false);
		public StateBinding _riseBinding = new StateBinding("rise", -1, false, false);
		public float rise;
		public float _angleOffset;

		bool reloading;
		float reload;

		float reloadTime = 2;

		public override float angle
		{
			get
			{
				return base.angle + _angleOffset;
			}
			set
			{
				_angle = value;
			}
		}

		public Bulldog(float xval, float yval) : base(xval, yval)
		{
			ammo = 4;
			_ammoType = new ATMagnum();
			_type = "gun";
			graphic = new Sprite("magnum", 0f, 0f);
			center = new Vec2(16f, 16f);
			collisionOffset = new Vec2(-8f, -6f);
			collisionSize = new Vec2(16f, 10f);
			_barrelOffsetTL = new Vec2(25f, 12f);
			_fireSound = "magnum";
			_kickForce = 4f;
			_fireRumble = RumbleIntensity.Light;
			_holdOffset = new Vec2(1f, 2f);
			handOffset = new Vec2(0f, 1f);
			_bio = "More or less nothing can stand before a direct hit from one of these 26mm monstrosities.";
			_editorName = "Bulldog";
			editorTooltip = "A custom revolver firing rounds so large it only holds 4 shots. Heavy duty pistol that pierces many objects. Cool shades not included.";
		}

		public override void Update()
		{
			base.Update();
			if (owner != null)
			{
				if (offDir < 0)
				{
					_angleOffset = -Maths.DegToRad(-rise * 55f);
				}
				else
				{
					_angleOffset = -Maths.DegToRad(rise * 55f);
				}
			}
			else
			{
				_angleOffset = 0f;
			}
			if (rise > 0f)
			{
				rise -= 0.018f;
			}
			else
			{
				rise = 0f;
			}
			if (_raised)
			{
				_angleOffset = 0f;
			}

            if (reloading)
            {
				reload--;
				if(reload > 100)
                {
					if (rise < 0.8f)
					{
						rise += 0.026f;
					}
                    else
                    {
						rise = 0.8f;
						PopShell();
                    }
                }
				if(reload <= 0)
                {
					reloading = false;
					rise = 0;
                }
            }
		}

		public override void OnPressAction()
		{
			if (!reloading)
			{
				base.OnPressAction();
				if (ammo > 0 && rise < 1f)
				{
					rise += 0.4f;
				}
			}
			if (ammo == 0)
			{
				ammo = 4;
				reloading = true;
				reload = reloadTime * 60;
			}
		}
	}
}