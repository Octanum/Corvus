﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorvEngine.Components {

	/// <summary>
	/// Provides a component that causes an Entity to follow a set path repeatedly.
	/// </summary>
	[Serializable]
	public class PathComponent : Component {

		// TODO: Important to figure out what to do when an Entity gets stuck. Reverse to start of path maybe? Cheat and fly?

		// TODO: This needs to be heavily re-done to allow for SceneGeometry.
		// For one thing, should just determine MovementActions to send to MovementComponent (also needs to be redone).
		// MovementComponent should either be able to handle all basic MovementActions (up, down, left, right, jump) or be abstract.


		private float _JumpDelay = 200;
		private bool _AllowMultiJump = false;
		private IList<Vector2> _Nodes = new List<Vector2>();
		private Vector2 _CurrentNode;
		private int _NodeIndex = 0;
		private DateTime _LastJump = DateTime.Now;
		private int _StepSize = 1;
		private bool _ReverseOnCompletion = true;
        private bool _PathingEnabled = true;

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
		/// Gets or sets a value indicating whether this Entity should reverse it's path once the path is complete, as opposed to going to the beginning node.
		/// </summary>
		public bool ReverseOnCompletion {
			get { return _ReverseOnCompletion; }
			set { _ReverseOnCompletion = value; }
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
        /// Gets or sets a value indicating whether to allow this entity to follow the path.
        /// </summary>
        public bool PathingEnabled
        {
            get { return _PathingEnabled; }
            set { _PathingEnabled = value; }
        }

		/// <summary>
		/// Add a new node to the list of nodes.
		/// </summary>
		/// <param name="node"></param>
		public void AddNode(Vector2 node) {
			if(Nodes.Count == 0) {
				_CurrentNode = node;
				_NodeIndex = 0;
			}
			Nodes.Add(node);
		}

		/// <summary>
		/// Sets the current node to the next node, and if it's last one, it loops over to the start.
		/// </summary>
		public void AdvanceNode() {
			if(Nodes != null) {
				if(_ReverseOnCompletion) {
					if(_StepSize > 0 && _NodeIndex >= Nodes.Count - _StepSize)
						_StepSize *= -1;
					if(_StepSize < 0 && _NodeIndex < Math.Abs(_StepSize))
						_StepSize *= -1;
					_NodeIndex += _StepSize;
				} else {
					_NodeIndex++;
					if(_NodeIndex >= Nodes.Count)
						_NodeIndex = 0;
				}
				_CurrentNode = Nodes[_NodeIndex];
			}
		}

		/// <summary>
		/// Decides which direction the AI should walk to follow the path. Simple x value check.
		/// </summary>
		/// <param name="Time"></param>
		protected override void OnUpdate(GameTime Time) {
			if(Nodes != null && PathingEnabled) {
				var entity = this.Parent;
				var mc = entity.GetComponent<MovementComponent>();
				var pc = entity.GetComponent<PhysicsComponent>();
				var ps = Scene.GetSystem<PhysicsSystem>();
				//Formerly Vector2.Distance(entity.Position, CurrentNode) for Y stuff, but not needed.
                if(entity.Location.Contains((int)CurrentNode.X, (int)CurrentNode.Y)) {
                    if(!pc.IsGrounded)
                        return; // Do nothing, just wait for us to fall on our location.
                    AdvanceNode();
                } else {
                    bool MissingHorizontally = CurrentNode.X > entity.Location.Right || CurrentNode.X < entity.Location.Left;
                    if(entity.Location.Top > CurrentNode.Y && /*(DateTime.Now - _LastJump).TotalMilliseconds > JumpDelay &&*/ !MissingHorizontally) {
                        mc.Jump(AllowMultiJump);
                        _LastJump = DateTime.Now;
                    }
                    if (MissingHorizontally)
                    {
                        if (entity.Location.Left > CurrentNode.X)
                        {
                            mc.BeginWalking(Direction.Left);
                            // Check if we'll need to jump this frame.
                            if (!ps.IsLocationSolid(new Vector2(entity.Location.Left - pc.VelocityX * Time.GetTimeScalar(), entity.Location.Bottom + 5)))
                                mc.Jump(AllowMultiJump);
                        }
                        else if (entity.Location.Right < CurrentNode.X)
                        {
                            mc.BeginWalking(Direction.Right);
                            if (!ps.IsLocationSolid(new Vector2(entity.Location.Right + pc.VelocityX * Time.GetTimeScalar(), entity.Location.Bottom + 5)))
                                mc.Jump(AllowMultiJump);
                        }
                    }
                    else
                        mc.StopWalking(); 
                }
			}
			base.OnUpdate(Time);
		}

        /// <summary>
        /// Set the variable that allows us to follow a path to true, meaning we will continue following the path.
        /// </summary>
        public void StartFollowing()
        {
            PathingEnabled = true;
        }

        /// <summary>
        /// Set the variable that allows us to follow a path to false, meaning we will stop following the path.
        /// </summary>
        public void StopFollowing()
        {
            var mc = this.Parent.GetComponent<MovementComponent>();
            mc.StopWalking();
            PathingEnabled = false;
        }
	}
}
