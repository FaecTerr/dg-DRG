using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuckGame.DeepDuckGalactic
{
    public class Warthog : Gun
	{
		public sbyte _loadProgress = 100;
		public float _loadAnimation = 1f;
		public StateBinding _loadProgressBinding = new StateBinding("_loadProgress", -1, false, false);
		protected SpriteMap _loaderSprite;
		public Warthog(float x, float y) : base(x, y)
        {
            ammo = 6;
            _ammoType = new ATShotgun();
            _ammoType.range *= 2;
            wideBarrel = true;
            _type = "gun";
            graphic = new Sprite("shotgun", 0f, 0f);
            center = new Vec2(16f, 16f);
            collisionOffset = new Vec2(-8f, -3f);
            collisionSize = new Vec2(16f, 8f);
            _barrelOffsetTL = new Vec2(30f, 14f);
            _fireSound = "shotgunFire2";
            _kickForce = 4f;
            _fireRumble = RumbleIntensity.Light;
            _numBulletsPerFire = 6;
            _loaderSprite = new SpriteMap("shotgunLoader", 8, 8, false);
            _loaderSprite.center = new Vec2(4f, 4f);
            editorTooltip = "It's...a shotgun. I don't really have anything more to say about it.";
        }
        public override void OnPressAction()
        {
            if (this.loaded)
            {
                base.OnPressAction();
                this._loadProgress = -1;
                this._loadAnimation = -0.01f;
                return;
            }
            if (this._loadProgress == -1)
            {
                this._loadProgress = 0;
                this._loadAnimation = -1f;
            }
        }
		public override void Update()
		{
			base.Update();
			if (this._loadAnimation == -1f)
			{
				SFX.Play("shotgunLoad", 1f, 0f, 0f, false);
				this._loadAnimation = 0f;
			}
			if (this._loadAnimation >= 0f)
			{
				if (this._loadAnimation == 0.5f && this.ammo != 0)
				{
					base.PopShell(false);
				}
				if (this._loadAnimation < 1f)
				{
					this._loadAnimation += 0.1f;
				}
				else
				{
					this._loadAnimation = 1f;
				}
			}
			if (this._loadProgress >= 0)
			{
				if (this._loadProgress == 50)
				{
					this.Reload(false);
				}
				if (this._loadProgress < 100)
				{
					this._loadProgress += 10;
					return;
				}
				this._loadProgress = 100;
			}
		}
		public override void Draw()
		{
			base.Draw();
			Vec2 bOffset = new Vec2(13f, -2f);
			float offset = (float)Math.Sin((double)(this._loadAnimation * 3.14f)) * 3f;
			base.Draw(this._loaderSprite, new Vec2(bOffset.x - 8f - offset, bOffset.y + 4f), 1);
		}
	}
}
