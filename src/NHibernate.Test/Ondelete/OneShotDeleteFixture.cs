namespace NHibernate.Test.Ondelete
{
	using System.Collections;
	using Cfg;
	using NUnit.Framework;

	[TestFixture]
	public class OneShotDeleteFixture : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override IList Mappings
		{
			get { return new string[] { "Ondelete.OneShot.hbm.xml" }; }
		}

		protected override void Configure(Configuration cfg)
		{
			cfg.SetProperty(Environment.GenerateStatistics, "true");
		}

		[Test]
		public void OneShotDeleteCascade()
		{
			var statistics = sessions.Statistics;

			using (var session = OpenSession())
			using (var tx = session.BeginTransaction()) {
				var parent = new Parent ("Bob");
				parent.Children.Add(new Child(parent, "Jim"));
				parent.Children.Add(new Child(parent, "Tim"));
				session.Save (parent);
				tx.Commit();
			}

			using (var session = OpenSession())
			using (var tx = session.BeginTransaction()) {
				var parent = session.Get<Parent> ("Bob");

				statistics.Clear();
				session.Delete (parent);
				tx.Commit ();
			}

			Assert.AreEqual(2, statistics.PrepareStatementCount);
		}
	}
}
