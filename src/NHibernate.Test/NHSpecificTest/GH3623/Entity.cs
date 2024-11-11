using System;
using System.Collections.Generic;

namespace NHibernate.Test.NHSpecificTest.GH3623
{
	public class Entity1
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public IList<ComponentListEntry> Entries { get; set; } = new List<ComponentListEntry>();
	}

	public class Entity2
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public IList<ComponentListEntry> Entries { get; set; } = new List<ComponentListEntry>();
	}

	public class ComponentListEntry
	{
		public string DummyString { get; set; }
		public string Property1 { get; set; }
		public string Property2 { get; set; }
	}
}
