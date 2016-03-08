﻿using System;
using numl.Model;
using numl.Utils;
using System.Linq;
using numl.Tests.Data;
using NUnit.Framework;
using System.Collections.Generic;
using numl.Serialization;

namespace numl.Tests.SerializationTests.DescriptorSerialization
{
    [TestFixture, Category("Serialization")]
    public class DescriptorSerializationTests : BaseSerialization
    {
        public static IEnumerable<string> WordStrings
        {
            get
            {
                yield return "TH@e QUi#CK bRow!N fox 12";
                yield return "s@upeR B#roWN BEaR";
                yield return "1829! UGlY FoX";
                yield return "ThE QuICk beAr";
            }
        }

        [TestFixtureSetUp]
        public void RegisterTypes()
        {
            // Need to register external assemblies
            Register.Assembly(GetType().Assembly);
        }

        [Test]
        public void Descriptor_Save_And_load()
        {
            var data = Iris.Load();
            var description = Descriptor.Create<Iris>();
            // to populate dictionaries
            var examples = description.ToExamples(data);

            Serialize(description);
            var d = Deserialize<Descriptor>();

            Assert.AreEqual(description.Type, d.Type);
            Assert.AreEqual(description.Features.Length, d.Features.Length);
            for (int i = 0; i < description.Features.Length; i++)
            {
                Assert.AreEqual(description.Features[i].Type, d.Features[i].Type);
                Assert.AreEqual(description.Features[i].Name, d.Features[i].Name);
                Assert.AreEqual(description.Features[i].Start, d.Features[i].Start);
            }

            Assert.AreEqual(description.Label.Type, d.Label.Type);
            Assert.AreEqual(description.Label.Name, d.Label.Name);
            Assert.AreEqual(description.Label.Start, d.Label.Start);
        }

        [Test]
        public void Complex_Descriptor_Save_And_Load_No_Label()
        {
            var data = Generic.GetRows(100).ToArray();
            var description = Descriptor.Create<Generic>();
            // to populate dictionaries
            var examples = description.ToExamples(data);

            Serialize(description);
            var d = Deserialize<Descriptor>();

            Assert.AreEqual(description.Type, d.Type);
            Assert.AreEqual(description.Features.Length, d.Features.Length);
            for (int i = 0; i < description.Features.Length; i++)
            {
                Assert.AreEqual(description.Features[i].Type, d.Features[i].Type);
                Assert.AreEqual(description.Features[i].Name, d.Features[i].Name);
                Assert.AreEqual(description.Features[i].Start, d.Features[i].Start);
            }

            Assert.IsTrue(d.Label == null);
        }


        [Test]
        public void Descriptor_Dictionary_Serialization_Test()
        {
            Descriptor description = new Descriptor();

            var dictionary = StringHelpers.BuildWordDictionary(WordStrings)
                                          .Select(k => k.Key)
                                          .ToArray();

            description.Features = new Property[]
            {
                new Property { Name = "Age", Type = typeof(int) },
                new Property { Name = "Height", Type = typeof(decimal) },
                new StringProperty { Name = "Words", Dictionary = dictionary },
                new Property { Name = "Weight", Type = typeof(double) },
                new Property { Name = "Good", Type = typeof(bool) },
                
            };

            Serialize(description);
            var d = Deserialize<Descriptor>();

            Assert.AreEqual(description, d);
        }

        [Test]
        public void Descriptor_Full_Serialization_Test()
        {
            var dictionary = StringHelpers.BuildWordDictionary(WordStrings)
                                          .Select(k => k.Key)
                                          .ToArray();

            Descriptor description = Descriptor.New("MyDescriptor")
                                     .With("NumberProp").As(typeof(double))
                                     .With("EnumerableProp").AsEnumerable(5)
                                     .With("DateTimeProp").AsDateTime(DatePortion.Date | DatePortion.Time)
                                     .With("StringProp").AsString(StringSplitType.Word)
                                     .With("StringEnumProp").AsStringEnum()
                                     .Learn("TargetProp").As(typeof(bool));

            ((StringProperty)description["StringProp"]).Dictionary = dictionary;
            ((StringProperty)description["StringEnumProp"]).Dictionary = dictionary;

            Serialize(description);
            var d = Deserialize<Descriptor>();

            Assert.AreEqual(description, d);
        }

        [Test]
        public void Descriptor_With_Class_Serialization_Test()
        {
            var description = Descriptor.Create<House>();
            Serialize(description);
            var d = Deserialize<Descriptor>();
            Assert.AreEqual(description, d);
        }
    }
}