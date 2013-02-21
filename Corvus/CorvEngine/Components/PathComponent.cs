﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorvEngine.Components {

	/// <summary>
	/// Provides a component that causes an Entity to follow a set path repeatedly.
	/// </summary>
	class PathComponent : Component {
		
		private float _JumpDelay = 200;
		private bool _AllowMultiJump = true;
		private IList<Vector2> _Nodes;
		private Vector2 _CurrentNode;
		private float _ArrivalDistance = 25;
		private int _NodeIndex = 0;
		private DateTime _LastJump = DateTime.Now;

		/// <summary>
		/// Gets the nodes that this entity paths to.
		/// This is in world coordinates.
		/// </summary>
		public IList<Vector2> Nodes {
			get { return _Nodes; }
			private set {
				_Nodes = value;
				_CurrentNode = _Nodes.FirstOrDefault();
			}
		}

		/// <summary>
		/// Gets the current node that this Entity is going to.
		/// </summary>
		public Vector2 CurrentNode {
			get { return _CurrentNode; }
		}

		/// <summary>
		/// Gets or sets the delay between jumps, in milliseconds, for this entity when it's below the target path location.
		/// </summary>
		public float JumpDelay {
			get { return _JumpDelay; }
			set { _JumpDelay = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether to allow this entity to jump multiple times.
		/// </summary>
		public bool AllowMultiJump {
			get { return _AllowMultiJump; }
			set { _AllowMultiJump = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating the minimum distance this Entity must be from it's node for it to consider having reached it.
		/// </summary>
		public float ArrivalDistance {
			get { return _ArrivalDistance; }
			set { _ArrivalDistance = value; }
		}

        /// <summary>
        /// Add a new node to the list of nodes.
        /// </summary>
        /// <param name="node"></param>
		public void AddNode(Vector2 node) {
			if(Nodes.Count() == 0) {
				_CurrentNode = node;
				_NodeIndex = 0;
			}
			Nodes.Add(node);
		}

        /// <summary>
        /// Sets the current node to the next node, and if it's last one, it loops over to the start.
        /// </summary>
		public void AdvanceNode() {
			if(_NodeIndex >= Nodes.Count - 1)
				_NodeIndex = 0;
			else
				_NodeIndex++;
			_CurrentNode = Nodes[_NodeIndex];
		}

        /// <summary>
        /// Decides which direction the AI should walk to follow the path. Simple x value check.
        /// </summary>
        /// <param name="Time"></param>
		protected override void OnUpdate(GameTime Time) {
			Entity entity = this.Parent;
			MovementComponent mc = entity.GetComponent<MovementComponent>();

            //Formerly Vector2.Distance(entity.Position, CurrentNode) for Y stuff, but not needed.
			if(Math.Abs(entity.Position.X - CurrentNode.X) < ArrivalDistance) {
				AdvanceNode();
			} else {
				mc.Walk(entity.X < CurrentNode.X ? Direction.Right : Direction.Left);
				if(entity.Y > CurrentNode.Y + ArrivalDistance && (DateTime.Now - _LastJump).TotalMilliseconds > JumpDelay) {
					mc.Jump(AllowMultiJump);
					_LastJump = DateTime.Now;
				}
			}
			base.OnUpdate(Time);
		}
	}
}
