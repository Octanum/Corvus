﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CorvEngine.Entities {
	/// <summary>
	/// Provides a reference to a Component, including name and properties to assign.
	/// </summary>
	public class ComponentBlueprint {

		/// <summary>
		/// Gets the type of the Component being referenced by this ComponentBlueprint.
		/// </summary>
		public Type Type {
			get { return _Type; }
		}
		
		/// <summary>
		/// Gets the name to assign to the Component when it's created.
		/// </summary>
		public string Name {
			get { return _Name; }
		}

		/// <summary>
		/// Gets the properties to give the Component when it's created.
		/// </summary>
		public List<ComponentProperty> Properties {
			get { return _Properties; } // TODO: Can make this return something else later. Hate returning modifiable collections for most things.
		}

		public ComponentBlueprint(Type Type, string Name, IEnumerable<ComponentProperty> Properties) {
			this._Properties = Properties.ToList();
			this._Name = Name;
			this._Type = Type;
		}

		private List<ComponentProperty> _Properties;
		private Type _Type;
		private string _Name;
	}
}
