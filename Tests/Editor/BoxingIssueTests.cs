using System;
using System.Linq;
using NUnit.Framework;
using Unity.ProjectAuditor.Editor;

namespace UnityEditor.ProjectAuditor.EditorTests
{
    class BoxingIssueTests
    {
        TempAsset m_TempAssetBoxingFloat;
        TempAsset m_TempAssetBoxingGeneric;
        TempAsset m_TempAssetBoxingGenericRefType;
        TempAsset m_TempAssetBoxingInt;

        [OneTimeSetUp]
        public void SetUp()
        {
            m_TempAssetBoxingInt = new TempAsset("BoxingIntTest.cs",
                "using System; class BoxingIntTest { Object Dummy() { return 666; } }");
            m_TempAssetBoxingFloat = new TempAsset("BoxingFloatTest.cs",
                "using System; class BoxingFloatTest { Object Dummy() { return 666.0f; } }");
            m_TempAssetBoxingGenericRefType = new TempAsset("BoxingGenericRefType.cs",
                "class SomeClass {}; class BoxingGenericRefType<T> where T : SomeClass { T refToGenericType; void Dummy() { if (refToGenericType == null){} } }");
            m_TempAssetBoxingGeneric = new TempAsset("BoxingGeneric.cs",
                "class BoxingGeneric<T> { T refToGenericType; void Dummy() { if (refToGenericType == null){} } }");
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            TempAsset.Cleanup();
        }

        [Test]
        public void BoxingIntValueIsReported()
        {
            var issues = ScriptIssueTestHelper.AnalyzeAndFindScriptIssues(m_TempAssetBoxingInt);

            Assert.AreEqual(1, issues.Count());

            var boxingInt = issues.FirstOrDefault();

            // check issue
            Assert.NotNull(boxingInt);
            Assert.True(boxingInt.name.Equals("BoxingIntTest.Dummy"));
            Assert.True(boxingInt.filename.Equals(m_TempAssetBoxingInt.scriptName));
            Assert.True(boxingInt.description.Equals("Conversion from value type 'Int32' to ref type"));
            Assert.True(boxingInt.callingMethod.Equals("System.Object BoxingIntTest::Dummy()"));
            Assert.AreEqual(1, boxingInt.line);
            Assert.AreEqual(IssueCategory.Code, boxingInt.category);

            // check descriptor
            Assert.NotNull(boxingInt.descriptor);
            Assert.AreEqual(Rule.Action.Default, boxingInt.descriptor.action);
            Assert.AreEqual(102000, boxingInt.descriptor.id);
            Assert.True(string.IsNullOrEmpty(boxingInt.descriptor.type));
            Assert.True(string.IsNullOrEmpty(boxingInt.descriptor.method));
            Assert.False(string.IsNullOrEmpty(boxingInt.descriptor.description));
            Assert.True(boxingInt.descriptor.description.Equals("Boxing Allocation"));
        }

        [Test]
        public void BoxingFloatValueIsReported()
        {
            var issues = ScriptIssueTestHelper.AnalyzeAndFindScriptIssues(m_TempAssetBoxingFloat);

            Assert.AreEqual(1, issues.Count());

            var boxingFloat = issues.FirstOrDefault();

            // check issue
            Assert.NotNull(boxingFloat);
            Assert.True(boxingFloat.name.Equals("BoxingFloatTest.Dummy"));
            Assert.True(boxingFloat.filename.Equals(m_TempAssetBoxingFloat.scriptName));
            Assert.True(boxingFloat.description.Equals("Conversion from value type 'float' to ref type"));
            Assert.True(boxingFloat.callingMethod.Equals("System.Object BoxingFloatTest::Dummy()"));
            Assert.AreEqual(1, boxingFloat.line);
            Assert.AreEqual(IssueCategory.Code, boxingFloat.category);

            // check descriptor
            Assert.NotNull(boxingFloat.descriptor);
            Assert.AreEqual(Rule.Action.Default, boxingFloat.descriptor.action);
            Assert.AreEqual(102000, boxingFloat.descriptor.id);
            Assert.True(string.IsNullOrEmpty(boxingFloat.descriptor.type));
            Assert.True(string.IsNullOrEmpty(boxingFloat.descriptor.method));
            Assert.False(string.IsNullOrEmpty(boxingFloat.descriptor.description));
            Assert.True(boxingFloat.descriptor.description.Equals("Boxing Allocation"));
        }

        [Test]
        public void BoxingGenericIsReported()
        {
            var issues = ScriptIssueTestHelper.AnalyzeAndFindScriptIssues(m_TempAssetBoxingGeneric);

            Assert.AreEqual(1, issues.Count());
        }

        [Test]
        public void BoxingGenericRefTypeIsNotReported()
        {
            var issues = ScriptIssueTestHelper.AnalyzeAndFindScriptIssues(m_TempAssetBoxingGenericRefType);

            Assert.Zero(issues.Count());
        }
    }
}
