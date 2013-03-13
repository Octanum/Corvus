﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CorvEngine.Components;
using CorvEngine.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Corvus.Components.Gameplay;

namespace Corvus.Components{
    /// <summary>
    /// Handles the damage events for this entity.
    /// </summary>
    public class DamageComponent : Component{
        private AttributesComponent AttributesComponent;
        private CombatComponent CombatComponent;
        private FloatingTextList _FloatingTexts = new FloatingTextList();

        /// <summary>
        /// Applies damage with only normal rule.
        /// </summary>
        public void TakeDamage(float incomingDamage){
            float blockMultipler = BlockDamageReduction();
            float damageTaken = NormalDamageFormula(AttributesComponent.Defense, incomingDamage);
            float overallDamage = damageTaken * blockMultipler;
            AttributesComponent.CurrentHealth -= overallDamage;

            _FloatingTexts.AddFloatingTexts(overallDamage, Color.White);
        }

        /// <summary>
        /// Applies damage, with the normal rules, based on the attacker's attributes.
        /// </summary>
        public void TakeDamage(AttributesComponent attacker){
            float blockMultipler = BlockDamageReduction();
            float damageTaken = NormalDamageFormula(AttributesComponent.Defense, attacker.Attack);
            float criticalMultiplier = CriticalDamageChance(attacker.CriticalChance, attacker.CriticalDamage);
            float elementalMultiplier = ElementalDamage(AttributesComponent.ResistantElements, attacker.AttackingElements);
            float overallDamage = damageTaken * criticalMultiplier * blockMultipler * elementalMultiplier;
            AttributesComponent.CurrentHealth -= overallDamage;

            if (criticalMultiplier != 1) //critical hit.
                _FloatingTexts.AddFloatingTexts(overallDamage, Color.Orange);
            else if (elementalMultiplier > 1) //entity is weak to that element
                _FloatingTexts.AddFloatingTexts(overallDamage, Color.Crimson);
            else if (elementalMultiplier < 1) //entity is resistant to that element
                _FloatingTexts.AddFloatingTexts(overallDamage, Color.Navy);
            else
                _FloatingTexts.AddFloatingTexts(overallDamage, Color.White);
        }
        
        private float BlockDamageReduction(){
            return (CombatComponent.IsBlocking) ? AttributesComponent.BlockDamageReduction : 1f;
        }

        private float NormalDamageFormula(float myDefense, float incomingDamage){
            return Math.Max(incomingDamage - (myDefense * 0.70f), 1);
        }

        private float CriticalDamageChance(float critChance, float critDamage){
            Random rand = new Random();
            return (rand.NextDouble() <= critChance) ? critDamage : 1f;
        }

        /// <summary>
        /// Basically. Fire < Water < Earth < Wind < Fire.... Physical reduces Phyiscal by 75%.
        /// </summary>
        private float ElementalDamage(Elements res, Elements att)
        {
            if (res == Elements.None || att == Elements.None)
                return 1f;
            
            float multiplier = 1f;
            if (res == Elements.Physical && att == Elements.Physical)
                multiplier -= 0.75f;
            if ((res == Elements.Fire && att == Elements.Water) ||
                (res == Elements.Water && att == Elements.Earth) ||
                (res == Elements.Earth && att == Elements.Wind) ||
                (res == Elements.Wind && att == Elements.Fire))
                multiplier += 0.5f;

            if ((att == Elements.Fire && res == Elements.Water)||
                (att == Elements.Water && res == Elements.Earth)||
                (att == Elements.Earth && res == Elements.Wind) ||
                (att == Elements.Wind && res == Elements.Fire))
                multiplier -= 0.5f;

            return multiplier;
        }

        protected override void OnInitialize(){
            base.OnInitialize();
            AttributesComponent = this.GetDependency<AttributesComponent>();
            CombatComponent = Parent.GetComponent<CombatComponent>();
        }

        protected override void OnUpdate(GameTime Time){
            base.OnUpdate(Time);
            if (_FloatingTexts.Count == 0)
                return;
            var width = Parent.Size.X;
            var position = Parent.Position + new Vector2(width / 2, 0);
            var ToScreen = Camera.Active.ScreenToWorld(position);
            _FloatingTexts.Update(Time, ToScreen);
        }

        protected override void OnDraw(){
            base.OnDraw();
            if (_FloatingTexts.Count == 0)
                return;
            _FloatingTexts.Draw();
        }
    }

    
}