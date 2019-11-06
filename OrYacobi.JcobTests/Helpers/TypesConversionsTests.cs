using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrYacobi.Jcob.Helpers;

namespace OrYacobi.JsonWithBinaryTests.Helpers
{
    [TestClass()]
    public class TypesConversionsTests
    {
        [TestMethod()]
        public void SizeOfTest()
        {
            Dictionary<Type, int> sizes = new Dictionary<Type, int>
            {
                {typeof(sbyte), sizeof(sbyte)},
                {typeof(byte), sizeof(byte)},
                {typeof(short), sizeof(short)},
                {typeof(ushort), sizeof(ushort)},
                {typeof(int), sizeof(int)},
                {typeof(uint), sizeof(uint)},
                {typeof(long), sizeof(long)},
                {typeof(ulong), sizeof(ulong)},
                {typeof(char), sizeof(char)},
                {typeof(float), sizeof(float)},
                {typeof(double), sizeof(double)},
                {typeof(decimal), sizeof(decimal)},
                {typeof(bool), sizeof(bool)},
            };

            foreach (KeyValuePair<Type, int> pair in sizes)
            {
                if (PrimitiveSizeHelper.SizeOf(pair.Key) != pair.Value)
                {
                    Assert.Fail();
                }
            }
        }
    }
}