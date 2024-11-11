using System;
using System.Linq;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Mapping.ByCode;
using NHibernate.Util;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.GH3623
{
	[TestFixture]
	public class ComponentsListFixture : TestCaseMappingByCode
	{
		private Guid _id1;
		private Guid _id2;

		protected override void OnSetUp()
		{
			using var session = OpenSession();
			using var tr = session.BeginTransaction();
			var root1 = new Entity1();
			root1.Entries.Add(new ComponentListEntry { DummyString = "one", Property1 = "first", Property2 = "second" });

			var root2 = new Entity2();
			root2.Entries.Add(new ComponentListEntry { DummyString = "two", Property1 = "third", Property2 = "fourth" });

			session.Save(root1);
			session.Save(root2);
			tr.Commit();
			_id1 = root1.Id;
			_id2 = root2.Id;
		}

		[Test]
		public void Loading()
		{
			using var newSession = OpenSession();
			var entity1 = newSession.Get<Entity1>(_id1);
			var entity1Entry = entity1.Entries.First();
			var entity2 = newSession.Get<Entity2>(_id2);
			var entity2Entry = entity2.Entries.First();

			Assert.That(entity1Entry.DummyString, Is.EqualTo("one"));
			Assert.That(entity2Entry.DummyString, Is.EqualTo("two"));
			Assert.That(entity1Entry.Property1, Is.EqualTo("first"));
			Assert.That(entity2Entry.Property2, Is.EqualTo("fourth"));

			Assert.That(entity1Entry.Property2, Is.Null);
			Assert.That(entity2Entry.Property1, Is.Null);
		}

		protected override void OnTearDown()
		{
			using var session = OpenSession();
			using var transaction = session.BeginTransaction();
			session.Delete("from System.Object");
			transaction.Commit();
		}

		protected override HbmMapping GetMappings()
		{
			var mapper = new ModelMapper();

			mapper.Class<Entity1>(rc =>
			{
				rc.Table("Entity1");
				rc.Id(x => x.Id, map => map.Generator(Generators.GuidComb));
				rc.Lazy(false);
				rc.Property(x => x.Name);

				rc.Bag(
					x => x.Entries,
					v =>
					{
						v.Table("Entity1Entry");
						v.Key(x => x.Column("Entity1Id"));
					},
					h => h.Component(cmp =>
					{
						cmp.Property(x => x.DummyString);
						cmp.Property(x => x.Property1);
					}));
			});

			mapper.Class<Entity2>(rc =>
			{
				rc.Table("Entity2");
				rc.Id(x => x.Id, map => map.Generator(Generators.GuidComb));
				rc.Lazy(false);
				rc.Property(x => x.Name);

				rc.Bag(
					x => x.Entries,
					v =>
					{
						v.Table("Entity2Entry");
						v.Key(x => x.Column("Entity2Id"));
					},
					h => h.Component(cmp =>
					{
						cmp.Property(x => x.DummyString);
						cmp.Property(x => x.Property2);
					}));
			});

			return mapper.CompileMappingForAllExplicitlyAddedEntities();
		}
	}
}
