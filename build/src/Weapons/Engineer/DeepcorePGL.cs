﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.DeepDuckGalactic
{
	//[EditorGroup("Faecterr's|Guns")]
	public class DeepcorePGL : Gun
    {
		public override float angle
		{
			get
			{
				return base.angle + this._aimAngle;
			}
			set
			{
				this._angle = value;
			}
		}

		public DeepcorePGL(float xval, float yval) : base(xval, yval)
		{
			this.wideBarrel = true;
			this.ammo = 9;
			this._type = "gun";
			this.graphic = new Sprite("grenadeLauncher", 0f, 0f);
			this.center = new Vec2(16f, 16f);
			this.collisionOffset = new Vec2(-6f, -4f);
			this.collisionSize = new Vec2(16f, 7f);
			this._barrelOffsetTL = new Vec2(28f, 14f);
			this._fireSound = "pistol";
			this._kickForce = 3f;
			this._fireRumble = RumbleIntensity.Light;
			this._holdOffset = new Vec2(4f, 0f);
			this._ammoType = new ATGrenade();
			this._fireSound = "deepMachineGun";
			this._bulletColor = Color.White;
			this.editorTooltip = "Delivers a fun & exciting present to a long distance friend. Hold fire to adjust arc.";
		}

		public override void Update()
		{
			base.Update();
			if (this._aiming && this._aimWait <= 0f && this._fireAngle < 90f)
			{
				//_fireAngle += 3f;
			}
			if (this._aimWait > 0f)
			{
				this._aimWait -= 0.9f;
			}
			if ((double)this._cooldown > 0.0)
			{
				this._cooldown -= 0.1f;
			}
			else
			{
				this._cooldown = 0f;
			}
			if (this.owner != null)
			{
				this._aimAngle = -Maths.DegToRad(this._fireAngle);
				if (this.offDir < 0)
				{
					this._aimAngle = -this._aimAngle;
				}
			}
			else
			{
				this._aimWait = 0f;
				this._aiming = false;
				this._aimAngle = 0f;
				this._fireAngle = 0f;
			}
			if (this._raised)
			{
				this._aimAngle = 0f;
			}
		}

		public override void OnPressAction()
		{
			if (this._cooldown == 0f)
			{
				if (this.ammo > 0)
				{
					this.Fire();
					return;
				}
				SFX.Play("click", 1f, 0f, 0f, false);
			}
		}

		public override void OnReleaseAction()
		{
			if (this._cooldown == 0f && this.ammo > 0)
			{
				this._aiming = false;
				this._cooldown = 1f;
				this.angle = 0f;
				this._fireAngle = 0f;
			}
		}

		public StateBinding _fireAngleState = new StateBinding("_fireAngle", -1, false, false);
		public StateBinding _aimAngleState = new StateBinding("_aimAngle", -1, false, false);
		public StateBinding _aimWaitState = new StateBinding("_aimWait", -1, false, false);
		public StateBinding _aimingState = new StateBinding("_aiming", -1, false, false);
		public StateBinding _cooldownState = new StateBinding("_cooldown", -1, false, false);
		public float _fireAngle;
		public float _aimAngle;
		public float _aimWait;
		public bool _aiming;
		public float _cooldown;
	}
}

