﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine;
using CorvEngine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Corvus {
	public class TestState : GameState {
		public override bool BlocksUpdate {
			get { return true; }
		}

		public override bool BlocksDraw {
			get { return true; }
		}

		public TestState() {
			this.AddComponent(new TestComponent(this));

		}

		private class TestComponent : GameStateComponent {

			private SpriteBatch Batch = new SpriteBatch(CorvBase.Instance.GraphicsDevice);
			private static Random rnd = new Random();
			private Color Color = new Color((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble());
			private Texture2D Texture;

			public TestComponent(GameState State) : base(State) {
				var Loader = new ContentManager(CorvBase.Instance.Services, "Content");
				this.Texture = Loader.Load<Texture2D>("TestTexture");
			}

			protected override void OnDraw(GameTime Time) {
				Batch.Begin();
				Batch.Draw(Texture, new Rectangle(0, 0, 800, 600), Color);
				Batch.End();
			}

			protected override void OnUpdate(GameTime Time) {
				
			}
		}
	}
}
