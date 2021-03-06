﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CorvEngine.Graphics;
using CorvEngine;

namespace Corvus.Components
{
    public class EnemySpawnedEvent : EventArgs
    {
        public Entity Entity { get; set; }
        public EnemySpawnedEvent(Entity entity)
            : base()
        {
            Entity = entity;
        }
    }

    public class SpawnerComponent : Component
    {
        /// <summary>
        /// Gets or sets an id. Used to determine what should be spawned from this spawner.
        /// </summary>
        public string SpawnID
        {
            get { return _SpawnID; }
            set { _SpawnID = value; }
        }

        /// <summary>
        /// Gets or sets the list of entities this spawner should spawn.
        /// </summary>
        public List<string> EntitiesToSpawn
        {
            get { return _EntitiesToSpawn; }
            set { _EntitiesToSpawn = value; }
        }

        /// <summary>
        /// Gets or sets the size of the entity when it spawns.
        /// </summary>
        public Vector2 EntitySize
        {
            get { return _EntitySize; }
            set { _EntitySize = value; }
        }

        /// <summary>
        /// Gets or sets the difficulty modifier.
        /// </summary>
        public float DifficultyModifier
        {
            get { return _DifficultyModifier; }
            set { _DifficultyModifier = Math.Max(value, 1f); }
        }

        /// <summary>
        /// Gets or sets the min range of enemies to sawpn.
        /// </summary>
        public int SpawnRangeMin
        {
            get { return _SpawnRangeMin; }
            set { _SpawnRangeMin = (int)MathHelper.Clamp(value, 0, EntitiesToSpawn.Count - 1); }
        }

        /// <summary>
        /// Gets or sets the max range of entities to spawn.
        /// </summary>
        public int SpawnRangeMax
        {
            get { return _SpawnRangeMax; }
            set { _SpawnRangeMax = (int)MathHelper.Clamp(value, 0, EntitiesToSpawn.Count - 1); }
        }

        public DateTime LastSpawn
        {
            get { return _LastSpawn; }
            set { _LastSpawn = value; }
        }

        public int TotalEntitiesSpawned
        {
            get { return _TotalEntitiesSpawned; }
            set { _TotalEntitiesSpawned = value; }
        }

        public int TotalEntitiesToSpawn
        {
            get { return _TotalEntitiesToSpawn; }
            set { _TotalEntitiesToSpawn = value; }
        }

        public bool SpawnerEnabled
        {
            get { return _SpawnerEnabled; }
            set { _SpawnerEnabled = value; }
        }
        
        public float SpawnDelay
        {
            get { return _SpawnDelay; }
            set { _SpawnDelay = value; }
        }

        public bool IsOnEnemySpawnRegistered { get { return OnEnemySpawn != null; } }

        public event EventHandler<EnemySpawnedEvent> OnEnemySpawn;

        private string _SpawnID = "";
        private List<string> _EntitiesToSpawn = new List<string>();
        private Vector2 _EntitySize = new Vector2();
        private float _DifficultyModifier = 1f;
        private bool _SpawnerEnabled = false;
        private DateTime _LastSpawn;
        private int _TotalEntitiesSpawned;
        private int _TotalEntitiesToSpawn;
        private float _SpawnDelay;
        private int _SpawnRangeMin = 0;
        private int _SpawnRangeMax = 0;

        public void Reset()
        {
            _SpawnRangeMin = 0;
            _SpawnRangeMax = 0;
            _DifficultyModifier = 1f;
            _TotalEntitiesSpawned = 0;
            SpawnerEnabled = false;
        }

        public Entity Spawn()
        {
            int index = RandomNumber(SpawnRangeMin, SpawnRangeMax + 1);//EntitiesToSpawn.Count());
            Entity entity = CorvEngine.Components.Blueprints.EntityBlueprint.GetBlueprint(EntitiesToSpawn[index]).CreateEntity();
            entity.Position = this.Parent.Position;
            entity.Size = EntitySize;
            Scene.AddEntity(entity);

            if (OnEnemySpawn != null)
                OnEnemySpawn(this, new EnemySpawnedEvent(entity));
            return entity;
        }

        protected override void OnUpdate(GameTime Time)
        {
            if ((DateTime.Now - LastSpawn).TotalSeconds > SpawnDelay && TotalEntitiesSpawned != TotalEntitiesToSpawn && SpawnerEnabled)
            {
                LastSpawn = DateTime.Now;
                Entity entity = Spawn();
                TotalEntitiesSpawned++;

                var ac = entity.GetComponent<AttributesComponent>();
                ac.MaxHealth *= DifficultyModifier;
                ac.StrModifier *= DifficultyModifier;
                ac.AttackSpeedModifier /= DifficultyModifier;
                ac.KnockbackModifier = 0f; //Disabled Knockback. Totally a cheap kill in this mode.
            }

            base.OnUpdate(Time);
        }

        //Function to get random number
        //Apparently the max range is exclusive, meaning not included in the returned set.
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        public static int RandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(min, max);
            }
        }
    }
}
