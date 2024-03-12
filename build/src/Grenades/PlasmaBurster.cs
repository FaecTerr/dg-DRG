using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.DeepDuckGalactic
{
	[EditorGroup("Faecterr's|Guns|Grenades")]
	public class PlasmaBurster : Gun
	{
		int bounces = 4;
		float cooldown;
		bool pin;
		Vec2 prevPosition;
		int stuck;
		public PlasmaBurster(float x, float y) : base(x, y)
		{
			ammo = 1;
			graphic = new SpriteMap(GetPath("sprites/grenades/PlasmBurster.png"), 12, 12);

			center = new Vec2(6, 6);

			collisionSize = new Vec2(10, 10);
			collisionOffset = new Vec2(-5, -5);
			bouncy = 0;

		}

		public override void Update()
		{
			base.Update();
			if(owner == null && prevOwner != null && pin)
            {
				canPickUp = false;
				if (bouncy == 0)
				{
					bouncy = 1;
					cooldown = 0.05f;
				}
			}
			if(cooldown > 0)
            {
				cooldown -= 0.01f;
            }
			if(bounces <= 0)
            {
				Level.Remove(this);
            }

			if (pin)
			{
				if (prevPosition == position)
				{
					stuck++;
					if (stuck >= 10)
					{
						vSpeed = -4;
						hSpeed = offDir * 4;
					}
				}
                else
                {
					stuck = 0;
                }
				prevPosition = position;
			}
		}

		public void Explosion()
		{
			float cx = position.x;
			float cy = position.y - 2f;
			Level.Add(new ExplosionPartPurp(cx, cy, true));
			int num = 6;
			if (Graphics.effectsLevel < 2)
			{
				num = 3;
			}
			for (int i = 0; i < num; i++)
			{
				float dir = (float)i * 60f + Rando.Float(-10f, 10f);
				float dist = Rando.Float(12f, 20f);
				Level.Add(new ExplosionPartPurp(cx + (float)(Math.Cos(Maths.DegToRad(dir)) * dist), cy - (float)(Math.Sin(Maths.DegToRad(dir)) * dist), true) { });
			}
			//SFX.Play("explode", 1f, 0f, 0f, false);
		}

		public override void OnSolidImpact(MaterialThing with, ImpactedFrom from)
		{
			base.OnSolidImpact(with, from);
			if (with != null && prevOwner != null)
			{
				if (cooldown <= 0 && pin)
				{
					SFX.Play(GetPath("sfx/grenades/BouncyPlasmaGrenade.wav"));
					for (int i = 0; i < 20; i++)
					{
						float dir = i * 18f - 5f + Rando.Float(10f);
						ATShrapnel shrap = new ATShrapnel();
						shrap.range = 60f + Rando.Float(18f);
						Bullet bullet = new Bullet(position.x + (float)(Math.Cos(Maths.DegToRad(dir)) * 6.0), position.y - (float)(Math.Sin(Maths.DegToRad(dir)) * 6.0), shrap, dir, null, false, -1f, false, true);
						bullet.firedFrom = this;
						firedBullets.Add(bullet);
						Level.Add(bullet);
					}
					if (Math.Sign(hSpeed) != 0)
					{
						float refraction = vSpeed * 0.5f;
						hSpeed += refraction * Math.Sign(hSpeed);
						vSpeed -= refraction * 0.5f;
					}
                    else
                    {
						float refraction = vSpeed * 0.8f;
						hSpeed += refraction * offDir;
						vSpeed -= refraction * 0.5f;
					}
					Explosion();
					bounces--;
					cooldown = 0.1f;
				}
			}
		}

		public override void Fire()
		{
            if (!pin)
            {
				pin = true;
				frame = 1;
            }
		}
	}

	public class ExplosionPartPurp : Thing
	{
		public ExplosionPartPurp(float xpos, float ypos, bool doWait = true) : base(xpos, ypos, null)
		{
			this._sprite = new SpriteMap(GetPath("sprites/grenades/explosion.png"), 64, 64, false);
			int i = Rando.ChooseInt(new int[]
			{
				0,
				1,
				2
			});
			if (i == 0)
			{
				this._sprite.AddAnimation("explode", 1f, false, new int[]
				{
					0,
					0,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9,
					10
				});
			}
			else if (i == 1)
			{
				this._sprite.AddAnimation("explode", 1.2f, false, new int[]
				{
					0,
					1,
					2,
					3,
					4,
					5,
					6,
					7,
					8,
					9
				});
			}
			else if (i == 2)
			{
				this._sprite.AddAnimation("explode", 0.9f, false, new int[]
				{
					3,
					4,
					5,
					6,
					7,
					8,
					9
				});
			}
			this._sprite.SetAnimation("explode");
			this.graphic = this._sprite;
			this._sprite.speed = 0.4f + Rando.Float(0.2f);
			base.xscale = 0.5f + Rando.Float(0.5f);
			base.yscale = base.xscale;
			this.center = new Vec2(32f, 32f);
			this._wait = Rando.Float(1f);
			this._smokeFrame = Rando.Int(1, 3);
			base.depth = 1f;
			this.vSpeed = Rando.Float(-0.2f, -0.4f);
			if (!doWait)
			{
				this._wait = 0f;
			}
		}

		public override void Initialize()
		{
		}

		public override void Update()
		{
			if (!this._created)
			{
				this._created = true;
			}
			if (this._sprite.frame > this._smokeFrame && !this._smoked)
			{
				int num = (Graphics.effectsLevel == 2) ? Rando.Int(1, 4) : 1;
				for (int i = 0; i < num; i++)
				{
					SmallSmoke smallSmoke = SmallSmoke.New(base.x + Rando.Float(-5f, 5f), base.y + Rando.Float(-5f, 5f));
					smallSmoke.vSpeed = Rando.Float(0f, -0.5f);
					smallSmoke.xscale = (smallSmoke.yscale = Rando.Float(0.2f, 0.7f));
					Level.Add(smallSmoke);
				}
				this._smoked = true;
			}
			if (this._wait <= 0f)
			{
				base.y += this.vSpeed;
			}
			if (this._sprite.finished)
			{
				Level.Remove(this);
			}
		}

		public override void Draw()
		{
			if (this._wait > 0f)
			{
				this._wait -= 0.2f;
				return;
			}
			base.Draw();
		}

		private bool _created;

		private SpriteMap _sprite;

		private float _wait;
		private int _smokeFrame;
		private bool _smoked;
	}
}



