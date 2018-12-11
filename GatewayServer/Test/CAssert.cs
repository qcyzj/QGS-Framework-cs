using System.Diagnostics;

namespace GatewayServer.Test
{
    public static class CAssert
    {
        public static void AreEqual(object left, object right)
        {
            Debug.Assert(left.Equals(right));
        }

        public static void AreEqual(byte[] left, byte[] right)
        {
            Debug.Assert(left.Length == right.Length);

            for (int i = 0; i < left.Length; ++i)
            {
                Debug.Assert(left[i] == right[i]);
            }
        }

        public static void AreNotEqual(object left, object right)
        {
            Debug.Assert(!left.Equals(right));
        }

        public static void IsNull(object obj)
        {
            Debug.Assert(null == obj);
        }

        public static void IsNotNull(object obj)
        {
            Debug.Assert(null != obj);
        }

        public static void IsFalse(bool result)
        {
            Debug.Assert(!result);

        }

        public static void IsTrue(bool result)
        {
            Debug.Assert(result);
        }

        public static void Fail()
        {
            Debug.Assert(false);
        }
    }
}
